using System;

namespace Velocity.Core
{
    public class FactoryMakerException : Exception
    {
        public FactoryMakerException(string message, Exception exception)
            : base(message, exception)
        {

        }
    }
}
