using Corsa.Domain.Exceptions;
using Corsa.Domain.Moduls;
using Corsa.Domain.Processing.Serp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{
    public class XmlYandexSerpParser :Module, IRuntimeModule<Stream,List<SerpWebPage>>
    {
        public override int Code
        {
            get
            {
                return 3001;
            }
        }

        public override string Name => this.Context.Localizer["Xml Yandex Serp Parser"];

        public XmlYandexSerpParser()
        {            
        }

        public List<SerpWebPage> Run(Stream config)
        {
            this.Context.LogInform(this.Context.Localizer["Loading xml source..."]);

            XDocument xmlDocument = XDocument.Load(config);

            this.Context.LogInform(this.Context.Localizer["Xml source was loaded successfully:"]);

            this.Context.LogInform(this.Context.Localizer["Analysis of xml source..."]);

            var error = xmlDocument.Root.Descendants("error").FirstOrDefault();
            if (error != null)
            {
                throw new UserException(error.Value);
            }

            int count = 0;
            var result = from g in xmlDocument.Root.Descendants("group")
                         let index = count++
                         let docGroup = from d in g.Descendants("doc") select new { Url = d.Element("url").Value, Title = d.Element("title").Value, Position = index }
                         from doc in docGroup
                         select new SerpWebPage() { Url = doc.Url, Title = doc.Title, Position = doc.Position };

            this.Context.LogInform(this.Context.Localizer[$"Analysis of xml source was completed. Number of Serp Pages : {result.Count()}"]);

            return result.ToList();
        }
    }    
}
