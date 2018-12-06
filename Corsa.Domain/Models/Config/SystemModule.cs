using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Models.SystemConfig
{
    [Table("SystemModules")]
    public class SystemModule
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedTime { get; set; }        
    }
}
