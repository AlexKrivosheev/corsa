using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Corsa.Domain.Moduls.RutimeModuls.Antigate
{
    [DataContract]    
    public class AntigateCreateTaskRequest
    {
        public AntigateCreateTaskRequest(string clientKey, string languagePool, int softId, string callbackUrl)
        {
            ClientKey = clientKey;
            LanguagePool = languagePool;
            SoftId = softId;
            CallbackUrl = callbackUrl;
        }

        [DataMember(Name ="clientKey")]
        public string ClientKey { get; set; }

        [DataMember(Name = "languagePool", EmitDefaultValue = false)]
        public string LanguagePool { get; set; }

        [DataMember(Name = "softId", EmitDefaultValue = false)]
        public int SoftId { get; set; }

        [DataMember(Name = "callbackUrl", EmitDefaultValue = false)]
        public string CallbackUrl { get; set; }

        [DataMember(Name = "task")]
        public AntigateTaskConfig Task { get; set; }
        
    }
}
