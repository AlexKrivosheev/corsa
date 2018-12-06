using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Exceptions
{
    public class UserException :Exception
    {
        public UserException(string message,Exception innerException): base(message, innerException)
        {
            
        }

        public UserException(string message) : base(message, null)
        {

        }

    }
}
