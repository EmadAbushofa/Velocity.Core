using System;

namespace Velocity.Core
{
    public class ServerErrorException : Exception
    {
        public ServerErrorException(string message) : base(message)
        {
        }
    }
}
