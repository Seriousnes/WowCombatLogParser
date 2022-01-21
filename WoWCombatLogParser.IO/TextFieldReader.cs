﻿namespace WoWCombatLogParser.IO
{
    public class TextFieldReader : IDisposable
    {
        private static readonly HashSet<char> openingBrackets = new() { '(', '[', '{' };
        private readonly StringReader sr;

        public char[] Delimiters { get; set; }
        public bool HasFieldsEnclosedInQuotes { get; init; }

        public TextFieldReader(StringReader reader)
        {
            this.sr = reader;
        }

        public TextFieldReader(string line) : this(new StringReader(line))
        {
        }

        public IList<IField> ReadFields()
        {
            var content = new List<IField>();
            IField? field = null;
            int character;
            int index = 0;
            while ((character = sr.Read()) != -1)
            {
                char c = (char)character;

                if (field is QuotedTextField quotedField)
                {
                    int next;
                    if (c == '"' && ((next = sr.Peek()) == -1 || ((char)next).In(Delimiters)))
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
                    if (c == '"' && HasFieldsEnclosedInQuotes)
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
                            if (c.In(Delimiters))
                            {
                                if (field is not null && field is not GroupField)
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

        private T AddFieldToResults<T>(int startIndex, IField? parent, IList<IField> results) where T : IField, new()
        {
            T field = new();
            field.Range = new Range(startIndex, 0);

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

        public void Dispose()
        {
            sr.Dispose();
        }
    }
}