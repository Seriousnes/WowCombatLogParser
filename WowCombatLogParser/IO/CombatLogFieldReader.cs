using System.IO;

namespace WoWCombatLogParser.IO;

internal static class CombatLogFieldReader
{
    private static readonly HashSet<char> openingBrackets = ['(', '[', '{'];
    private static readonly char[] delimiters = [','];
    private static readonly bool hasFieldsEnclosedInQuotes = true;
    private static readonly char SPACE = (char)0x20;

    private static List<ICombatLogDataField> ReadFields(StringReader sr)
    {
        var content = new List<ICombatLogDataField>();
        ICombatLogDataField? currentField = null;
        int character;
        char last = '\0';
        while ((character = sr.Read()) != -1)
        {            
            char c = (char)character;

            // two spaces in a row is the delimiter between timestamp and event type
            if (c == SPACE && sr.Peek() == SPACE && sr.Read() > 0)
                c = ',';

            if (currentField is QuotedCombatLogTextField quotedField)
            {
                int next;
                if (c == '"' && ((next = sr.Peek()) == -1 || ((char)next).In(delimiters)))
                {
                    quotedField.Finalise();
                    currentField = quotedField.Parent;
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
                    currentField = AddFieldToResults<QuotedCombatLogTextField>(currentField, content);
                }
                else
                {
                    // EMOTE events don't have quotes to indicate single field, but all grouped collections will begin with the sequence ,[
                    var ignoreThisBracketDelmiter = c == '[' && last != ',';

                    if (openingBrackets.Contains(c) && !ignoreThisBracketDelmiter)
                    {
                        var bracketField = AddFieldToResults<CombatLogDataFieldCollection>(currentField, content);
                        bracketField.OpeningBracket = c;
                        currentField = bracketField;
                    }
                    else if (currentField is CombatLogDataFieldCollection bracketField && bracketField.ClosingBracket == c)
                    {
                        if (currentField is CombatLogTextField textField)
                            textField.Finalise();
                        currentField = bracketField.Parent;                        
                    }
                    else if (currentField is CombatLogTextField && currentField.Parent is CombatLogDataFieldCollection textFieldParent && textFieldParent.ClosingBracket == c)
                    {
                        if (currentField is CombatLogTextField textField)
                            textField.Finalise();
                        currentField = textFieldParent.Parent;                        
                    }
                    else
                    {
                        if (c.In(delimiters))
                        {
                            if (currentField is CombatLogTextField textField)
                                textField.Finalise();
                            if (currentField != null && currentField is not CombatLogDataFieldCollection)
                                currentField = currentField.Parent;
                        }
                        else
                        {
                            if (currentField is CombatLogDataFieldCollection || currentField is null)
                            {
                                currentField = AddFieldToResults<CombatLogTextField>(currentField, content);
                            }

                            ((CombatLogTextField)currentField).Append(c);
                        }
                    }
                }
            }

            last = c;
        }
        if (currentField is CombatLogTextField finalField && !finalField.IsFinalised)
        {
            finalField.Finalise();
            if (!content.Contains(finalField))
                content.Add(finalField);
        }

        return content;
    }

    public static CombatLogLineData ReadFields(string line)
    {
        using var sr = new StringReader(line);
        return new CombatLogLineData(ReadFields(sr));
    }

    private static T AddFieldToResults<T>(ICombatLogDataField? parent, List<ICombatLogDataField> results) where T : ICombatLogDataField, new()
    {
        T field = new();
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
}