using System;
using System.Runtime.Serialization;

namespace Corsa.Domain.Moduls.RutimeModuls.Antigate
{
    public class AntigateCustomTask<TResult> : AntigateTaskConfig
    {
        public override string Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override Type ResultType => throw new NotImplementedException();
    }


    [DataContract]
    public class ImageToTextTask : AntigateTaskConfig
    {        
        public override string Type
        {
            get { return "ImageToTextTask"; }
            set { }
        }

        public override Type ResultType
        {
            get { return typeof(AntigateTaskResult<ImageToTextTaskSolution>); }
        }

        [DataMember(Name = "numeric", EmitDefaultValue = false)]
        public int Numeric { get; set; }

        [DataMember(Name = "minLength", EmitDefaultValue = false)]
        public int MinLength { get; set; }

        [DataMember(Name = "maxLength", EmitDefaultValue = false)]
        public int MaxLength { get; set; }

        [DataMember(Name = "phrase", EmitDefaultValue = false)]
        public bool Phrase { get; set; }

        [DataMember(Name = "case", EmitDefaultValue = false)]
        public bool Case { get; set; }

        [DataMember(Name = "math", EmitDefaultValue = false)]
        public bool Math { get; set; }

        [DataMember(Name = "comment", EmitDefaultValue = false)]
        public string Comment { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }       
    }

    [DataContract]
    public class ImageToTextTaskSolution : AntigateSolutionBase
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }        
    }
}
