using System;

namespace HeyTeam.Core.Entities {
    public class Evaluation {
        public Evaluation (SquadMember evaluator) {
            if (evaluator == null || !evaluator.Id.HasValue) 
                throw new ArgumentNullException("The evaluator must be registered");
                
            Evaluator = evaluator;
        }

        public long? Id { get; set; }
        public SquadMember Evaluator { get; private set; }
        public string Comments { get; set; }
    }
}