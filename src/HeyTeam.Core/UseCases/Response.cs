using System;
using System.Collections.Generic;

namespace HeyTeam.Core.UseCases {
    public class Response<T> {        
        public Response(){}
        public Response(T result) { this.Result = result; }
        public Response(Exception exception) => this.Exception = exception;
        public List<string> Errors{ get; } = new List<string>();
        public Exception Exception { get; }
        public bool WasRequestFulfilled { get => Exception == null && Errors.Count == 0; }
        public void AddError(string error) {
            if(!string.IsNullOrEmpty(error)) 
                Errors.Add(error);
        }
        public T Result{ get; }
        public static Response<T> CreateResponse(IEnumerable<string> errors) => BuildResponse(errors);
        public static Response<T> CreateResponse(Exception ex) => BuildResponse(new List<string> {ex.Message}, ex);
        private static Response<T> BuildResponse(IEnumerable<string> errors, Exception ex = null)
        {
            var response = new Response<T>(ex);
            foreach(var error in errors) response.AddError(error);
            return response;
        }
    }    
}