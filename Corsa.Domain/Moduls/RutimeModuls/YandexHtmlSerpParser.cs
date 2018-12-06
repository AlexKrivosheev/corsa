using Corsa.Domain.Exceptions;
using Corsa.Domain.Moduls;
using Corsa.Domain.Processing.Serp;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{
    public class CaptchaException : UserException
    {
        public string Key { get; set; }

        public string ImageUrl { get; set; }

        public string Retpath { get; set; }

        public CaptchaException(string message, string key, string imageUrl, string retpath) : base(message, null)
        {
            this.Key = key;
            this.ImageUrl = imageUrl;
            this.Retpath = retpath;
        }
    }

    public class YandexHtmlSerpItem
    {
        public int Postion { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
    }

    public class YandexHtmlSerpParser : Module, IRuntimeModule<Stream,List<YandexHtmlSerpItem>>
    {
        public override int Code
        {
            get
            {
                return 3001;
            }
        }

        public override string Name => this.Context.Localizer["Yandex Html Serp Parser"];

        public YandexHtmlSerpParser()
        {            
        }

        private List<YandexHtmlSerpItem> ParseContent(HtmlDocument document)
        {
            List<YandexHtmlSerpItem> result = new List<YandexHtmlSerpItem>();

            var serpItemNodes = document.DocumentNode.SelectNodes("//li[@class='serp-item']");

            if (serpItemNodes != null)
            {
                foreach (var serpItemNode in serpItemNodes)
                {
                    YandexHtmlSerpItem serpPage = new YandexHtmlSerpItem();
                    serpPage.Postion = serpItemNode.GetAttributeValue("data-cid", -1);

                    var urlNode = serpItemNode.SelectSingleNode(".//a");

                    if (urlNode == null)
                    {
                        this.Context.LogWarning(this.Context.Localizer[$"Serp item {serpPage.Postion} url not found"]);
                        continue;
                    }

                    serpPage.Href = urlNode.GetAttributeValue("href", string.Empty);

                    if (urlNode == null)
                    {
                        this.Context.LogWarning(this.Context.Localizer[$"Serp item {serpPage.Postion} url not found"]);
                        continue;
                    }

                    serpPage.Href = urlNode.GetAttributeValue("href", string.Empty);

                    result.Add(serpPage);
                }
            }

            return result;
        }

 

        private void ValidateCaptha(HtmlDocument document)
        {
            var captchaFormNode = document.DocumentNode.SelectSingleNode("//form[@action='/checkcaptcha']");

            if (captchaFormNode != null)
            {
                string key = string.Empty;
                
                var keyNode = document.DocumentNode.SelectSingleNode("//input[@name='key']");
                if (keyNode!=null)
                {
                    key = keyNode.GetAttributeValue("value", string.Empty);
                }

                string retPath = string.Empty;

                var retPathNode = document.DocumentNode.SelectSingleNode("//input[@name='retpath']");
                if (retPathNode != null)
                {
                    retPath = HttpUtility.HtmlDecode(retPathNode.GetAttributeValue("value", string.Empty));
                }

                string imageUrl = string.Empty;
                var imageNode = document.DocumentNode.SelectSingleNode("//img[@class='image form__captcha']");
                if (imageNode != null)
                {
                    imageUrl = imageNode.GetAttributeValue("src", string.Empty);
                }


                    
                throw new CaptchaException("Captha", key, imageUrl, retPath);
            }
        }

        public List<YandexHtmlSerpItem> Run(Stream config)
        {
            List<YandexHtmlSerpItem> result = new List<YandexHtmlSerpItem>();

            this.Context.LogInform(this.Context.Localizer["Loading xml source..."]);

            HtmlDocument document = new HtmlDocument();
            document.Load(config, Encoding.UTF8);

            ValidateCaptha(document);

            return ParseContent(document);
        }
    }    
}
