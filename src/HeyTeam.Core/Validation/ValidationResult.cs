using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Validation {
    public class ValidationResult<T> {
        public ValidationResult (T subject) {
            Subject = subject;
        }

        public T Subject {get; private set; }

        public bool IsValid { get { return Messages.Count == 0; } }

        public List<string> Messages { get; private set; } = new List<string>();

        public void AddMessage(string message) {
            if (!string.IsNullOrWhiteSpace(message)) Messages.Add(message);
        }        
    }
}