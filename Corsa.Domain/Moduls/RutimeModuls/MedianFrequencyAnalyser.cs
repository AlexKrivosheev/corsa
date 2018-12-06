using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Models.SearchEngines;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Processing.Serp;
using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{
    public class MedianFrequencyAnalyserConfig
    {
        public List<SerpWebPage> Pages { get; set; } = new List<SerpWebPage>();

        public Request Request { get; set; }

        public IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> HttpModule { get; set; }

    }

    public class MedianFrequencyAnalyser :Module, IRuntimeModule<MedianFrequencyAnalyserConfig,LexModuleStats>
    {
        public MedianFrequencyAnalyser()
        {            
        }

        public override int Code
        {
            get { return 2001; }
        }

        public override string Name => this.Context.Localizer["Median Frequency Analyser"];
  
        public LexModuleStats Analyze( List<LexicalTextAnalyzerStatistics> statistics)
        {
            var stats = new LexModuleStats();

            List<SerpWebPage> serpPages = new List<SerpWebPage>();
            Dictionary<string, List<int>> mutualWordStatistics = new Dictionary<string, List<int>>();

            foreach (var item in statistics)
            {
                item.Page.TotalWords = item.TotalWords;
                item.Page.Stats = stats;
                serpPages.Add(item.Page);

                foreach (var word in item.WordStatistics)
                {
                    if (!mutualWordStatistics.ContainsKey(word.Key))
                    {
                        mutualWordStatistics.Add(word.Key, new List<int>());
                    }
                    mutualWordStatistics[word.Key].Add(word.Value);
                }
            }

            List<RequestWord> medianFrequency = new List<RequestWord>();
            foreach (var word in mutualWordStatistics)
            {
                var sequance = new int[statistics.Count];
                word.Value.CopyTo(sequance);
                int value = CalcMedian(sequance);
                medianFrequency.Add(new RequestWord() { Name = word.Key, MedianValue = value, Stats = stats });
            }

            medianFrequency.Sort((arg1, arg2) => { return arg2.MedianValue.CompareTo(arg1.MedianValue); });

            medianFrequency = medianFrequency.Take(100).ToList();

            List<LexModuleFrequencyData> frequencyStatistics = new List<LexModuleFrequencyData>();

            foreach (var word in medianFrequency)
            {
                foreach (var statistic in statistics)
                {
                    int value = 0;
                    statistic.WordStatistics.TryGetValue(word.Name, out value);

                    frequencyStatistics.Add(new LexModuleFrequencyData() { Page = statistic.Page, Word = word, Value = value, Stats = stats });
                }
            }

            stats.SerpPages = serpPages;
            stats.Words = medianFrequency;
            stats.FrequencyData = frequencyStatistics;
            
            return stats;
        }

        private int CalcMedian(int[] sequance)
        {
            var result = sequance.OrderByDescending(v => v).ToArray();

            int value = 0;

            if (result.Length == 0)
            {
                return 0;
            }

            if (result.Length == 1)
            {
                return result[0];
            }

            if (result.Length % 2 == 0)
            {
                var arg1 = result[result.Length / 2 - 1];
                var arg2 = result[result.Length / 2];
                value = (arg1 + arg2) / 2;
            }
            else
            {
                value = result[(result.Length - 1) / 2];
            }

            return value;
        }

        public LexModuleStats Run(MedianFrequencyAnalyserConfig config)        
        {
            List<LexicalTextAnalyzerStatistics> lexTextStatistics = new List<LexicalTextAnalyzerStatistics>();
            
            var lexer = new HtmlLexer();
            var lexTextAnalyzer = new LexicalTextAnalyzer();

            foreach (var page in config.Pages.Take(10))
            {
                var httpResult = RuntimeTask.Run(Context, config.HttpModule, new HttpProviderRuntimeConfig() { Query = page.Url.ToString() });
                if (httpResult.IsSuccessfully)
                {
                    using (var stream = httpResult.Data.GetContent())
                    {
                        var lexerResult = RuntimeTask.Run(Context, lexer, stream);

                        if (lexerResult.Details.Result != ActionExecutionResult.Error)
                        {                            
                            var lexTextAnalyzerResult = RuntimeTask.Run(Context, lexTextAnalyzer, new LexicalTextAnalyzerConfig() { Page = page, Tokens = lexerResult.Data, Filter = token => { return token.Value.Count() > 3; } });

                            if (lexTextAnalyzerResult.Details.Result != ActionExecutionResult.Error)
                            {
                                lexTextStatistics.Add(lexTextAnalyzerResult.Data);
                            }
                        }
                    }                    
                }
            }

            return Analyze(lexTextStatistics);
        }
    }
}
