using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Exceptions;
using HeyTeam.Util;

namespace HeyTeam.Core.Entities {
    public class Club {
        public Club (Guid? guid = null) {
            Guid = guid.HasValue ? guid.Value : System.Guid.NewGuid();
        }
        public Guid Guid { get; }        
        public string Name { get; set; }
        public string Url { get; set; }
    }
}