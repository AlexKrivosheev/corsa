using System.Runtime.Serialization;

namespace Corsa.Domain.Moduls.RutimeModuls.Antigate
{
    [DataContract]
    public class AntigateTaskResult : ModuleData
    {
        [DataMember(Name = "errorId")]
        public int ErrorId { get; set; }

        [DataMember(Name = "errorCode")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "errorDescription")]
        public string ErrorDescription { get; set; }

        [DataMember(Name = "taskId")]
        public int TaskId { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "cost")]
        public double Cost { get; set; }

        [DataMember(Name = "ip")]
        public string Ip { get; set; }

        [DataMember(Name = "createTime")]
        public int CreateTime { get; set; }

        [DataMember(Name = "endTime")]
        public int EndTime { get; set; }

        [DataMember(Name = "solveCount")]
        public int SolveCount { get; set; }

        public AntigateTaskConfig Task { get; set; }
    }

    [DataContract]
    public class AntigateTaskResult<T> : AntigateTaskResult
    {
        [DataMember(Name = "solution")]
        public T Solution { get; set; }
    }
}
