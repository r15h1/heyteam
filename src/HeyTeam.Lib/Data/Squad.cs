using System;

namespace HeyTeam.Lib.Data {
    public class Squad {
        public Club Club { get; set ; }
        public long? ClubId { get; set; }
        public long? SquadId { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
}