using System.Collections.Generic;
using System.IO;

namespace WoWCombatLogParser;

public static class CombatLogFieldReader
{
    private static readonly HashSet<char> openingBrackets = new() { '(', '[', '{' };
    private static char[] delimiters = new[] { ',' };
    private static bool hasFieldsEnclosedInQuotes = true;
    private static char SPACE = (char)0x20;

    private static IList<ICombatLogDataField> ReadFields(StringReader sr)
    {
        var content = new List<ICombatLogDataField>();
        ICombatLogDataField field = null;
        int character;
        int index = 0;
        while ((character = sr.Read()) != -1)
        {
            char c = (char)character;

            if (c == SPACE && sr.Peek() == SPACE)
            {
                sr.Read();
                c = ',';
            }

            if (field is QuotedCombatLogTextField quotedField)
            {
                int next;
                if (c == '"' && ((next = sr.Peek()) == -1 || ((char)next).In(delimiters)))
                {
                    quotedField.Range.End = index - 1;
                    field = quotedField.Parent;
                }
                else
                {
                    quotedField.Append(c);
                }
            }
            else
            {
                if (c == '"' && hasFieldsEnclosedInQuotes)
                {
                    field = AddFieldToResults<QuotedCombatLogTextField>(index + 1, field, content);
                }
                else
                {
                    if (openingBrackets.Contains(c))
                    {
                        var bracketField = AddFieldToResults<CombatLogDataFieldCollection>(index, field, content);
                        bracketField.OpeningBracket = c;
                        field = bracketField;
                    }
                    else if (field is CombatLogDataFieldCollection bracketField && bracketField.ClosingBracket == c)
                    {
                        field = bracketField.Parent;
                        bracketField.Range.End = index;
                    }
                    else if (field is CombatLogTextField && field.Parent is CombatLogDataFieldCollection textFieldParent && textFieldParent.ClosingBracket == c)
                    {
                        field.Range.End = index - 1;
                        field.Parent.Range.End = index;
                        field = textFieldParent.Parent;
                    }
                    else
                    {
                        if (c.In(delimiters))
                        {
                            if (field != null && field is not CombatLogDataFieldCollection)
                            {
                                field.Range.End = index - 1;
                                field = field.Parent;
                            }
                        }
                        else
                        {
                            if (field is CombatLogDataFieldCollection || field is null)
                            {
                                field = AddFieldToResults<CombatLogTextField>(index, field, content);
                            }

                            ((CombatLogTextField)field).Append(c);
                        }
                    }
                }
            }

            index++;
        }

        return content;
    }

    public static CombatLogLineData ReadFields(string line)
    {
        using var sr = new StringReader(line);
        return new CombatLogLineData(ReadFields(sr));
    }

    private static T AddFieldToResults<T>(int startIndex, ICombatLogDataField parent, IList<ICombatLogDataField> results) where T : ICombatLogDataField, new()
    {
        T field = new() { Range = new Range(startIndex, 0) };
        if (parent is CombatLogDataFieldCollection bracketField)
        {
            bracketField.AddChild(field);
        }
        else
        {
            results.Add(field);
        }

        return field;
    }

    public class CombatLogLineData
    {
        public CombatLogLineData(IList<ICombatLogDataField> data)
        {
            EventType = data[1].ToString();
            data.RemoveAt(1);
            Data = data;
        }

        public string EventType { get; set; }
        public IList<ICombatLogDataField> Data { get; set; }
    }
}