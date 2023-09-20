using System.Collections.Generic;
using System.IO;

namespace WoWCombatLogParser.IO;

public static class CombatLogFieldReader
{
    private static readonly HashSet<char> openingBrackets = new() { '(', '[', '{' };
    private static char[] delimiters = new[] { ',' };
    private static bool hasFieldsEnclosedInQuotes = true;
    private static char SPACE = (char)0x20;

    private static IList<ICombatLogDataField> ReadFields(StringReader sr)
    {
        var content = new List<ICombatLogDataField>();
        ICombatLogDataField currentField = null;
        int character;
        while ((character = sr.Read()) != -1)
        {
            char c = (char)character;

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
                    if (openingBrackets.Contains(c))
                    {
                        var bracketField = AddFieldToResults<CombatLogDataFieldCollection>(currentField, content);
                        bracketField.OpeningBracket = c;
                        currentField = bracketField;
                    }
                    else if (currentField is CombatLogDataFieldCollection bracketField && bracketField.ClosingBracket == c)
                    {
                        currentField = bracketField.Parent;
                    }
                    else if (currentField is CombatLogTextField && currentField.Parent is CombatLogDataFieldCollection textFieldParent && textFieldParent.ClosingBracket == c)
                    {
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
        }

        return content;
    }

    public static CombatLogLineData ReadFields(string line)
    {
        using var sr = new StringReader(line);
        return new CombatLogLineData(ReadFields(sr));
    }

    private static T AddFieldToResults<T>(ICombatLogDataField parent, IList<ICombatLogDataField> results) where T : ICombatLogDataField, new()
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