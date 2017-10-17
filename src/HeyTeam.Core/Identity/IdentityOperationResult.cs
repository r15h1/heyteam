using System.Collections.Generic;
using HeyTeam.Util;

namespace HeyTeam.Core.Identity {
    public class IdentityOperationResult {
        private List<string> errors;
        public IdentityOperationResult(bool success) {
            this.Succeeded = success;
            errors = new List<string>();
        }

        public bool Succeeded { get; }
        
        public IEnumerable<string> Errors { 
            get => errors;
        }

        public void AddError(string error) {
            if(!error.IsEmpty())
                errors.Add(error);
        }
    }
}