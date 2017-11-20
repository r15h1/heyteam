using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Services {
    public class Response {        
        public Response(){}
        public Response(Exception exception) => this.Exception = exception;
        public List<string> Errors{ get; } = new List<string>();
        public Exception Exception { get; }
        public bool RequestIsFulfilled { get => Exception == null && Errors.Count == 0; }
        public void AddError(string error) {
            if(!string.IsNullOrEmpty(error)) 
                Errors.Add(error);
        }        
        public static Response CreateResponse(IEnumerable<string> errors) => BuildResponse(errors);
        public static Response CreateResponse(Exception ex) => BuildResponse(new List<string> {ex.Message}, ex);
        private static Response BuildResponse(IEnumerable<string> errors, Exception ex = null)
        {
            var response = new Response(ex);
            foreach(var error in errors) response.AddError(error);
            return response;
        }
    }    
}