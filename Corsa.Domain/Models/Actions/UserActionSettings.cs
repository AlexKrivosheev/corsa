using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Models.Actions
{
    public class UserActionSettings
    {
      public int Id { get; set; }
      public string UserId { get; set; }
      public int ActionId { get; set; }
      public int Quantity { get; set; }
      public DateTime PeriodStart { get; set; }
      public DateTime PeriodEnd { get; set; }
      public int Priority { get; set; }      
    }
}
