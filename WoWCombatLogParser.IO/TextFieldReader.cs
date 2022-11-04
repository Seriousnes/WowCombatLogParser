using System;
using System.Collections.Generic;
using System.IO;

namespace WoWCombatLogParser
{
    public static class TextFieldReader
    {
        private static readonly HashSet<char> openingBrackets = new HashSet<char>() { '(', '[', '{' };

        private static IList<IField> ReadFields(StringReader sr, TextFieldReaderOptions options)
        {
            var content = new List<IField>();
            IField field = null;
            int character;
            int index = 0;
            while ((character = sr.Read()) != -1)
            {
                char c = (char)character;

                if (field is QuotedTextField quotedField)
                {
                    int next;
                    if (c == '"' && ((next = sr.Peek()) == -1 || ((char)next).In(options.Delimiters)))
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
                    if (c == '"' && options.HasFieldsEnclosedInQuotes)
                    {
                        field = AddFieldToResults<QuotedTextField>(index + 1, field, content);
                    }
                    else
                    {
                        if (openingBrackets.Contains(c))
                        {
                            var bracketField = AddFieldToResults<GroupField>(index, field, content);
                            bracketField.OpeningBracket = c;
                            field = bracketField;
                        }
                        else if (field is GroupField bracketField && bracketField.ClosingBracket == c)
                        {
                            field = bracketField.Parent;
                            bracketField.Range.End = index;
                        }
                        else if (field is TextField && field.Parent is GroupField textFieldParent && textFieldParent.ClosingBracket == c)
                        {
                            field.Range.End = index - 1;
                            field.Parent.Range.End = index;
                            field = textFieldParent.Parent;
                        }
                        else
                        {
                            if (c.In(options.Delimiters))
                            {
                                if (field != null && !(field is GroupField))
                                {
                                    field.Range.End = index - 1;
                                    field = field.Parent;
                                }
                            }
                            else
                            {
                                if (field is GroupField || field is null)
                                {
                                    field = AddFieldToResults<TextField>(index, field, content);
                                }

                                ((TextField)field).Append(c);
                            }
                        }
                    }
                }

                index++;
            }

            return content;
        }

        public static IList<IField> ReadFields(string line, TextFieldReaderOptions options)
        {
            options ??= new TextFieldReaderOptions { Delimiters = new[] { ',' }, HasFieldsEnclosedInQuotes = false };
            using var sr = new StringReader(line?.Replace("  ", ","));
            var result = ReadFields(sr, options);
            return result;
        }

        private static T AddFieldToResults<T>(int startIndex, IField parent, IList<IField> results) where T : IField, new()
        {
            T field = new T { Range = new Range(startIndex, 0) };
            if (parent is GroupField bracketField)
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

    public class TextFieldReaderOptions
    {
        public char[] Delimiters { get; set; }
        public bool HasFieldsEnclosedInQuotes { get; set; }
    }
}