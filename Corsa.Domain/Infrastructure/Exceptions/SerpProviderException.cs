using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Processing
{
    public class SerpProviderException : Exception
    {
        public SerpProviderException(string message): base(message)
        {

        }
    }
}
