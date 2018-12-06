using Corsa.Domain.Moduls;
using Corsa.Domain.Processing.Serp;
using System;
using System.Linq;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{
    public class LexicalTextAnalyzerConfig
    {
        public SerpWebPage Page { get; set; }
        public Token[] Tokens { get; set; }
        public Predicate<Token> Filter { get; set; }
    }

    public class LexicalTextAnalyzer :Module, IRuntimeModule<LexicalTextAnalyzerConfig,LexicalTextAnalyzerStatistics>
    {       
        public override string Name => this.Context.Localizer["Lexical Frequency Analyser"];

        public override int Code
        {
            get { return 1; }
        }

        public LexicalTextAnalyzer()
        {
            
        }

        public LexicalTextAnalyzerStatistics Run(LexicalTextAnalyzerConfig config)
        {
            LexicalTextAnalyzerStatistics result = new LexicalTextAnalyzerStatistics() { Page = config.Page };
            int totalWords = 0;

            var wordStatistics = from token in config.Tokens
                                 where config.Filter(token) && token.Type == TokenType.Word
                                 group token by token.Value;

            foreach (var word in wordStatistics)
            {
                int numberWords = word.Count();
                result.WordStatistics.Add(word.Key, numberWords);
                totalWords += numberWords;
            }

            result.TotalWords = totalWords;

            return result;
        }
    }
}
