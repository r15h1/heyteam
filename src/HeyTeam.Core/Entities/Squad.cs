using System;

namespace HeyTeam.Core.Entities {
    public class Squad {
        public Squad(Club club, Guid? guid = null) {
            if(club == null) throw new ArgumentNullException();
            Club = club;
            Guid = guid.HasValue && guid.Value != Guid.Empty ? guid.Value : Guid.NewGuid();
        }
        public Club Club { get; private set; }
        public long? SquadId { get; set; }
        public Guid Guid { get; private set; }
        public string Name { get; set; }
    }
}