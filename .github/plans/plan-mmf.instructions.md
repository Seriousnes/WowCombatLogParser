# Execution Plan: High-Performance Zero-Allocation Log Parser (.NET 10)

## **1. Project Context & Objective**

We are refactoring a World of Warcraft combat log parser to handle large files (>1GB) with maximum throughput and minimal memory footprint. The goal is to move away from stream-based parsing to a **Memory Mapped File (MMF)** architecture with **Zero-Allocation** principles.

**Target Architecture:**

* **Phase 1 (Indexing):** A single-threaded, SIMD-accelerated scan to identify "Section" boundaries (e.g., `ENCOUNTER_START` to `ENCOUNTER_END`).
* **Phase 2 (Parallel Parsing):** Concurrent processing of identified sections using `Parallel.ForEach`.
* **Phase 3 (JIT Parsing):** Inside each thread, lines and tokens are parsed strictly using `Span<byte>` on the stack.

**Tech Stack:** C# (.NET 10), `System.IO.MemoryMappedFiles`, `System.Buffers`, `System.Runtime.CompilerServices` (Unsafe).

---

## **2. Implementation Steps**

### **Step 1: The MMF Wrapper (Infrastructure)**

Create a class `MemoryMappedLogReader` that manages the file lifecycle.

* **Action:** Open the file using `MemoryMappedFile.CreateFromFile`.
* **Access Pattern:** Create a `MemoryMappedViewAccessor` for the entire file.
* **Optimization:** Acquire a raw `byte*` pointer using `SafeMemoryMappedViewHandle.AcquirePointer`.
* **Constraints:**
* Implement `IDisposable` to correctly release the pointer and dispose of the accessor/file.
* Store the `FileLength` (long) to prevent out-of-bounds access.



### **Step 2: The Section Indexer (Phase 1)**

Create a method `IndexSections()` that scans the file to build a `List<LogSection>`.

* **Data Structure:**
```csharp
public readonly struct LogSection {
    public readonly long StartOffset; // Absolute offset in file
    public readonly int Length;       // Length of section (fits in int)
}

```


* **Algorithm:**
* Use `System.Buffers.SearchValues.Create(byte[])` to create a vectorized searcher for the start byte of your marker (e.g., 'E' for "ENCOUNTER").
* Scan using `Span<byte>.IndexOfAny`.
* **Crucial:** Because `Span<byte>` is limited to `int.MaxValue` (2GB), you cannot create a span for the whole file. You must implement a **Sliding Window** approach (e.g., 1GB views) using pointer arithmetic (`byte* currentPtr = basePtr + offset`).
* When a candidate byte is found, perform a sequence check (e.g., `SequenceEqual`) for the full markers `ENCOUNTER_START` and `ENCOUNTER_END`.


* **Output:** A lightweight list of `LogSection` structs.

### **Step 3: The Parallel Engine (Phase 2)**

Create the main processing loop using `Parallel.ForEach`.

* **Input:** The `List<LogSection>` from Step 2.
* **Concurrency:** Use `ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }`.
* **Logic:**
* Inside the loop, create a **local** `ReadOnlySpan<byte>` for the specific section:
`new ReadOnlySpan<byte>(basePtr + section.StartOffset, section.Length)`.
* Pass this Span to the `ParseEncounter` method.



### **Step 4: The Stack-Based Parser (Phase 3)**

Implement the `ParseEncounter(ReadOnlySpan<byte> data)` method.

* **Line Splitting:**
* Do **not** use `string.Split` or `StreamReader`.
* Iterate through the `data` Span looking for `\n` (newline).
* Slice the Span to get the current line: `ReadOnlySpan<byte> line = data.Slice(start, length)`.


* **Token Parsing:**
* Iterate through the `line` Span looking for delimiters (`,`).
* **Zero-Alloc String Check:** To check if a token equals a specific keyword (e.g., "SPELL_DAMAGE"), compare the byte sequence directly. Do not materialize a string.
* **Materialization:** Only allocate a `string` (using `Encoding.UTF8.GetString`) when you absolutely need to store a value (e.g., a Character Name) in the final result object. Numerical values should be parsed directly from the Span using `Utf8Parser.TryParse`.



---

## **3. Technical Guardrails & Requirements**

1. **NO** `FileStream` or `StreamReader` usage.
2. **NO** `string` allocations during the scanning/splitting phase.
3. **NO** LINQ on the hot path (e.g., avoid `Skip`, `Take`, `Select` on byte arrays).
4. **Unsafe Pointers:** Use `unsafe` context for pointer arithmetic (`byte*`) to bypass array bounds checking and handle >2GB file offsets.
5. **Span Limits:** explicit handling of the 2GB `Span` length limit. The Indexer must manage `long` offsets, but individual `LogSection` lengths can be assumed to be `< Int32.MaxValue`.

## **4. Code Snippet for AI Reference**

*Provide this snippet to the agent to demonstrate the desired "Sliding Window" pattern for the Indexer:*

```csharp
// Pattern for scanning >2GB files with Spans
long cursor = 0;
while (cursor < fileLength)
{
    long remaining = fileLength - cursor;
    int windowSize = (int)Math.Min(remaining, 1024 * 1024 * 1024); // 1GB Window
    
    // Create view into memory map at cursor
    var window = new ReadOnlySpan<byte>(basePtr + cursor, windowSize);
    
    // Search logic here...
    // If a marker is found, calculate absolute position: cursor + localIndex
    
    // Advance cursor
    cursor += processedBytes;
}

```