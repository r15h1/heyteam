using System;

namespace HeyTeam.Core.Exceptions {
    public class IllegalOperationException : Exception {
        public IllegalOperationException(string message):base(message){}
    }
}