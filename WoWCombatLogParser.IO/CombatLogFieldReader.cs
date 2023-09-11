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
        ICombatLogDataField field = null;
        int character;
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
                    field = AddFieldToResults<QuotedCombatLogTextField>(field, content);
                }
                else
                {
                    if (openingBrackets.Contains(c))
                    {
                        var bracketField = AddFieldToResults<CombatLogDataFieldCollection>(field, content);
                        bracketField.OpeningBracket = c;
                        field = bracketField;
                    }
                    else if (field is CombatLogDataFieldCollection bracketField && bracketField.ClosingBracket == c)
                    {
                        field = bracketField.Parent;
                    }
                    else if (field is CombatLogTextField && field.Parent is CombatLogDataFieldCollection textFieldParent && textFieldParent.ClosingBracket == c)
                    {
                        field = textFieldParent.Parent;
                    }
                    else
                    {
                        if (c.In(delimiters))
                        {
                            if (field != null && field is not CombatLogDataFieldCollection)
                            {
                                field = field.Parent;
                            }
                        }
                        else
                        {
                            if (field is CombatLogDataFieldCollection || field is null)
                            {
                                field = AddFieldToResults<CombatLogTextField>(field, content);
                            }

                            ((CombatLogTextField)field).Append(c);
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