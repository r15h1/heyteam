using System;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Core.Entities {
    public class Player : TeamMember, ISessionEvaluator {
        public enum Foot {
            RIGHT,
            LEFT
        }                
        public string Nationality { get;set; }
        public string SquadNumber {get; set; }
        public Foot DominantFoot { get; set; } = Foot.RIGHT;
        public void EvaluateSession(Session session, Evaluation evaluation) { 
            if(session == null) 
                throw new ArgumentNullException(); 

            session.AddEvaluation(evaluation); 
        }
    }
}