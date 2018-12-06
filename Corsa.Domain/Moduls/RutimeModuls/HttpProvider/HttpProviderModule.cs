using Corsa.Domain.Models.Actions;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{
    public class HttpProviderModule :Module, IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData>, IConfigModule<HttpProviderConfig>
    {
        public override string Name
        {
            get { return Configuration?.ProjectModule?.Name; }
        }

        public HttpProviderModule()
        {

        }

        public HttpProviderModule(int id)
        {
            this.Id = id;
        }

        public override int Code
        {
            get
            {
                return ModuleCode;
            }
        }

        public int Id { get; set; }

        public static int ModuleCode = 1005;
        
        public HttpProviderConfig Configuration { get; set; }
   
        public async Task<HttpProviderData> LoadAsync(HttpProviderRuntimeConfig config)
        {
            var httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = config.AllowAutoRedirect
            };

            if (!string.IsNullOrEmpty(Configuration.ProxyHost) &&(!string.IsNullOrEmpty(Configuration.ProxyPort)))
            {
                httpClientHandler.UseProxy = true;                
                httpClientHandler.ServerCertificateCustomValidationCallback = delegate { return true; };

                UriBuilder proxyUrl = new UriBuilder();
                proxyUrl.Host = Configuration.ProxyHost;
                proxyUrl.Port = int.Parse(Configuration.ProxyPort);

                httpClientHandler.Proxy = new WebProxy()
                {
                    Address = proxyUrl.Uri,
                    UseDefaultCredentials = true
                };

                if (!string.IsNullOrEmpty(Configuration.ProxyUser) && !string.IsNullOrEmpty(Configuration.ProxyPassword))
                {                    
                    httpClientHandler.UseDefaultCredentials = false;
                    httpClientHandler.Proxy.Credentials = new NetworkCredential(userName: Configuration.ProxyUser, password: Configuration.ProxyPassword);
                }

                httpClientHandler.UseCookies = true;
            }

            this.Context.LogInform(this.Context.Localizer[$"Request execution '{config.Query}'"]);

            using (HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept - Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept - Language", "en - US,en; q = 0.9");                
                client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep - alive");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade - Insecure - Requests", "1");                
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
                
                HttpProviderData data = new HttpProviderData();

                data.Response = config.IsPost ? await client.PostAsync(new Uri(config.Query), config.Content) : data.Response = await client.GetAsync(new Uri(config.Query));

                this.Context.LogInform(this.Context.Localizer[$"Request execution was completed"]);

                return data;
            }
        }

        public bool CreateAndSave(HttpProviderConfig config)
        {
            config.ProjectModule.Code = ModuleCode;
            config.ProjectModule.CreatedTime = DateTime.Now;
            config.ProjectModule.ProjectId = config.ProjectModule.ProjectId;

            return Context.Repository.AddModule(config.ProjectModule) & Context.Repository.AddHttpProviderConfig(config);
        }

        public UserActionDetails GetDetails(int id)
        {
            return Context.Repository.GetActionStatistic(id);
        }

        public bool SaveConfig(HttpProviderConfig config)
        {
            var projectModule = Context.Repository.GetModule(Id);
            projectModule.Name = config.ProjectModule.Name;
            projectModule.Description = config.ProjectModule.Description;

            Configuration.ProxyHost = config.ProxyHost;
            Configuration.ProxyPort = config.ProxyPort;
            Configuration.ProxyUser = config.ProxyUser;
            Configuration.ProxyPassword = config.ProxyPassword;
            
            return Context.Repository.UpdateModule(projectModule) & Context.Repository.UpdateHttpProviderConfig(Configuration);
        }

        public bool LoadConfig()
        {
            Configuration = Context.Repository.GetHttpProviderConfig(Id) ?? new HttpProviderConfig();
            return true;
        }

        public bool Perform()
        {
            return false;
        }

        public bool Load()
        {
            return LoadConfig();
        }

        public bool Drop()
        {
            return Context.Repository.DropModule(Id);
        }

        public HttpProviderData Run(HttpProviderRuntimeConfig config)
        {
            var task = LoadAsync(config);
            return task.Result;
        }
    }
}
