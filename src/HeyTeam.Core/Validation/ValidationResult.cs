using System;
using System.Collections.Generic;
using HeyTeam.Util;

namespace HeyTeam.Core.Validation {
    public class ValidationResult<T> {
        public ValidationResult (T subject) {
            Subject = subject;
        }

        public T Subject {get; private set; }

        public bool IsValid { get { return Messages.Count == 0; } }

        public List<string> Messages { get; private set; } = new List<string>();

        public void AddMessage(string message) {
            if (!message.IsEmpty()) Messages.Add(message);
        }        
    }
}