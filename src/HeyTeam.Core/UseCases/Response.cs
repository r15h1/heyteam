using System;
using System.Collections.Generic;

namespace HeyTeam.Core.UseCases {
    public class Response {
        public Response() { }
        public Response(Exception exception) => this.Exception = exception;
        public List<string> Messages{ get; } = new List<string>();
        public Exception Exception { get; }
        public bool WasRequestFulfilled { get => Exception == null && Messages.Count == 0; }
        public void AddMessage(string message) {
            if(!string.IsNullOrEmpty(message)) 
                Messages.Add(message);
        }
    }
}