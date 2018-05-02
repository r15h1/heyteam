using System;

namespace HeyTeam.Core {
	public class Club {
        public Club (Guid? guid = null) {
            Guid = guid.HasValue ? guid.Value : System.Guid.NewGuid();
        }
        public Guid Guid { get; }        
        public string Name { get; set; }
        public string Url { get; set; }
		public string LogoUrl { get; set; }
	}
}