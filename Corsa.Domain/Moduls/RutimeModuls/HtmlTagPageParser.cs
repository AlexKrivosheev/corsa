using Corsa.Domain.Moduls;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{
    public class TagProperties
    {
        public string Path { get; set; }

        public HtmlNodeExtractor Extractor { get; set; }
    }

    public class HtmlTextAttributeParser : HtmlNodeExtractor<string>
    {
        public string Attribute { get; set; }
   
        public override string ExctractNodeValue(HtmlNode node)
        {
            return node.GetAttributeValue(Attribute, string.Empty);            
        }
    }

    public class HtmlTagInnerText: HtmlNodeExtractor<string>
    {
        public override string ExctractNodeValue(HtmlNode node)
        {
            return node.InnerText;
        }
    }

    public class SinglHtmlNodeSelector : HtmlNodeSelector
    {
        public override object FindValue(HtmlNode node)
        {
            var targetNode = node.SelectSingleNode(Tag.Path);
            
            if (targetNode != null)
            {
                return Tag.Extractor.ExctractValue(targetNode);
            }

            return null;
        }
    }

    public class MultiHtmlNodeSelector<TValue> : HtmlNodeSelector
    {
        public override object FindValue(HtmlNode node)
        {
            List<TValue> result = new List<TValue>(); 

            var nodes = node.SelectNodes(Tag.Path);

            if (nodes != null)
            {
                foreach (var targetNode in nodes)
                {
                    result.Add((TValue)Tag.Extractor.ExctractValue(targetNode));
                }

                return result;
            }

            return null;
        }
    }

    public abstract class HtmlNodeSelector
    {
        public TagProperties Tag { get; set; }

        public abstract object FindValue(HtmlNode node);
    }

    public abstract class HtmlNodeExtractor<TValue> : HtmlNodeExtractor
    {
        public Func<TValue, TValue> PostHandle { get; set; }

        public abstract TValue ExctractNodeValue(HtmlNode node);

        public override object ExctractValue(HtmlNode node)
        {
            if (PostHandle != null)
            {
                return PostHandle(ExctractNodeValue(node));
            }

            return ExctractNodeValue(node);            
        }
    }

    public abstract class HtmlNodeExtractor
    {
        public abstract object ExctractValue(HtmlNode node);
    }

    public class HtmlTagPageConfig
    {
        public Stream Stream { get; set; }

        public Dictionary<string, HtmlNodeSelector> Tags { get; set; } = new Dictionary<string, HtmlNodeSelector>();
    }

    public class HtmlTagPageData
    {        
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
    }

    public class HtmlTagPageParser : Module, IRuntimeModule<HtmlTagPageConfig, HtmlTagPageData>
    {
        public override int Code
        {
            get
            {
                return 3002;
            }
        }

        public override string Name => this.Context.Localizer["Tag Html Page Parser"];

        public HtmlTagPageParser()
        {            
        }

        private HtmlTagPageData ParseContent(HtmlTagPageConfig config)
        {
            HtmlTagPageData result = new HtmlTagPageData();

            this.Context.LogInform(this.Context.Localizer["Loading xml source..."]);

            HtmlDocument document = new HtmlDocument();
            document.Load(config.Stream, Encoding.Default);

            foreach (var tag in config.Tags)
            {
                var value = tag.Value.FindValue(document.DocumentNode);
                result.Values.Add(tag.Key, value);
            }

            return result;
        }

        public HtmlTagPageData Run(HtmlTagPageConfig config)
        {
            return ParseContent(config);
        }
    }    
}
