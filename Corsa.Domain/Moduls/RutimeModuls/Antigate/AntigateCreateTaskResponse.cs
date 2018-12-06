using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Corsa.Domain.Moduls.RutimeModuls.Antigate
{
    [DataContract]
    public class AntigateCreateTaskResponse
    {
        [DataMember(Name = "errorId")]
        public int ErrorId { get; set; }

        [DataMember(Name = "errorCode")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "errorDescription")]
        public string ErrorDescription { get; set; }

        [DataMember(Name = "taskId")]
        public int TaskId { get; set; }
    }
}
