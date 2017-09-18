using System;

namespace HeyTeam.Core.Exceptions {
    public class PolicyViolationException : Exception {
        public PolicyViolationException(string message):base(message){}
    }
}