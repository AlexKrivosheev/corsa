using Corsa.Domain.Moduls;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{
    public class HtmlLexer :Module, IRuntimeModule<Stream, Token[]>
    {
        public HtmlLexer()
        {

        }

        public override string Name => this.Context.Localizer["Html Lexer"];
        
        public Dictionary<string, TokenType> HTMLSymbols = new Dictionary<string, TokenType>() {
            { "&nbsp" ,TokenType.WhiteSpace},
            { "nbsp" ,TokenType.WhiteSpace},
        };

        public Dictionary<string, TokenType> PunctuationSymbols = new Dictionary<string, TokenType>()
        {
            { "~" ,TokenType.Punctuation},
            { "«" ,TokenType.Punctuation},
            { "»" ,TokenType.Punctuation},
            { "#" ,TokenType.Punctuation},
            { "$" ,TokenType.Punctuation},
            { "&" ,TokenType.Punctuation},
            { "{" ,TokenType.Punctuation},
            { "}" ,TokenType.Punctuation},
            { "(" ,TokenType.Punctuation},
            { ")" ,TokenType.Punctuation},
            { "," ,TokenType.Punctuation},
            { ";" ,TokenType.Punctuation},
            { "|" ,TokenType.Punctuation},
            { "\"" ,TokenType.Punctuation},
            { "'" ,TokenType.Punctuation},
            { "." ,TokenType.Point },
            { ":" ,TokenType.Punctuation},
            { "?" ,TokenType.Point},
            { "!" ,TokenType.Point},
            { " ", TokenType.WhiteSpace},
            { "\\n ", TokenType.WhiteSpace}
        };

        public Dictionary<string, TokenType> MathSymbols = new Dictionary<string, TokenType>()
        {
            { "+" ,TokenType.Punctuation},
            { "-" ,TokenType.Punctuation},
            { "/" ,TokenType.Punctuation},
            { "=" ,TokenType.Punctuation},
            { "<", TokenType.Punctuation},
            { ">", TokenType.Punctuation},
        };

        public override int Code
        {
            get { return 5003; }
        }

        public Token[] Analyze(Stream stream)
        {
            List<Token> result = new List<Token>();
            HtmlDocument document = new HtmlDocument();
            document.Load(stream, Encoding.Default);

            ProcessChildNodes(document.DocumentNode, node => { return !string.Equals(node.Name, "script", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(node.Name, "style", StringComparison.InvariantCultureIgnoreCase); }, ref result);

            return result.ToArray();
        }

        private void ProcessChildNodes(HtmlNode parent, Predicate<HtmlNode> predicate, ref List<Token> result)
        {
            if (predicate(parent))
            {
                List<HtmlNode> children = parent.ChildNodes.ToList();

                parent.RemoveAllChildren();
                result.AddRange(Tokenize(parent.InnerText.Trim()));

                foreach (HtmlNode node in children)
                {
                    ProcessChildNodes(node, predicate, ref result);
                }
            }
        }

        public List<Token> Tokenize(string text)
        {
            List<Token> result = new List<Token>();
            List<char> sequence = new List<char>();
            List<char> separator = new List<char>();

            if (string.IsNullOrEmpty(text))
            {
                return result;
            }

            foreach (var letter in text.Trim())
            {
                separator.Add(letter);

                switch (FindSeparator(separator.ToArray()))
                {
                    case SeparatorState.Started:
                        {


                        }
                        break;
                    case SeparatorState.NotFound:
                        {
                            sequence.Add(letter);
                            separator.Clear();
                        }
                        break;
                    case SeparatorState.Completed:
                        {
                            result.Add(Tokenize(sequence.ToArray()));
                            result.Add(Tokenize(separator.ToArray()));
                            sequence.Clear();
                            separator.Clear();
                        }
                        break;
                }
            }

            if (sequence.Count > 0)
            {
                var token = Tokenize(sequence.ToArray());
                result.Add(token);
            }

            return result;
        }

        private enum SeparatorState
        {
            Started,
            Completed,
            NotFound
        }

        private SeparatorState FindSeparator(char[] letters)
        {
            var value = new string(letters);

            var result = FindSeparator(value, PunctuationSymbols);
            if (result != SeparatorState.NotFound)
            {
                return result;
            }

            result = FindSeparator(value, HTMLSymbols);
            if (result != SeparatorState.NotFound)
            {
                return result;
            }

            result = FindSeparator(value, MathSymbols);
            if (result != SeparatorState.NotFound)
            {
                return result;
            }

            return SeparatorState.NotFound;
        }

        private SeparatorState FindSeparator(string value, Dictionary<string, TokenType> dictionary)
        {

            SeparatorState state = SeparatorState.NotFound;
            foreach (var item in PunctuationSymbols.Keys.Where(key => key.StartsWith(value)))
            {
                if (item.Length == value.Length)
                {
                    return SeparatorState.Completed;
                }
                else
                {
                    state = SeparatorState.Started;
                }

            }
            return state;
        }

        public Token Tokenize(char[] letters)
        {
            var value = new string(letters);

            TokenType type;
            if (PunctuationSymbols.TryGetValue(value, out type))
            {
                return new Token() { Type = type, Value = value };
            }

            if (HTMLSymbols.TryGetValue(value, out type))
            {
                return new Token() { Type = type, Value = value };
            }

            if (MathSymbols.TryGetValue(value, out type))
            {
                return new Token() { Type = type, Value = value };
            }

            if (value.All(char.IsDigit))
            {
                return new Token() { Type = TokenType.Numeral, Value = value };
            }

            return new Token() { Type = TokenType.Word, Value = value.ToLower() }; ;
        }
      
        public Token[] Run(Stream config)
        {
            return Analyze(config);
        }
    }
}
