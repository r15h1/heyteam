using System;

namespace HeyTeam.Core.Exceptions {
    public class EntityNotFoundException : Exception {
        public EntityNotFoundException():base("The specified item was not found"){}
		public EntityNotFoundException(string message) : base(message) { }
	}
}