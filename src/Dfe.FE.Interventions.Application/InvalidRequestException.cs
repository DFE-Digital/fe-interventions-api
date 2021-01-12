using System;

namespace Dfe.FE.Interventions.Application
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message)
            : base(message)
        {
            
        }
    }
}