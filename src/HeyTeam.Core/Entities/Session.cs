using System;
using System.Collections.Generic;
using HeyTeam.Core.Exceptions;
using System.Linq;

namespace HeyTeam.Core.Entities {
    public class Session {
        public int? Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Evaluation> evaluations { get; private set; } = new List<Evaluation>();     

        internal void AddEvaluation(Evaluation evaluation) {
            Validate(evaluation);
            if(evaluation.Id.HasValue) {
                var existing = evaluations.FirstOrDefault(e => e.Evaluator.Id == evaluation.Evaluator.Id);
                if(existing != null) {
                    evaluation.Id = existing.Id;
                    evaluations.Remove(existing);                
                }
            }
            evaluations.Add(evaluation);
        }

        private void Validate(Evaluation evaluation) {
            if(evaluation == null) throw new ArgumentNullException();            
            if (!Id.HasValue) throw new IllegalOperationException ("Evaluations can be added to registered sessions only (sessions that have a valid id)");
            if (!evaluation.Evaluator.Id.HasValue) throw new IllegalOperationException ("Evaluations can be added by registered evaluators only (must have a valid id)");
            if(string.IsNullOrWhiteSpace(evaluation.Comments)) throw new IllegalOperationException ("Evaluation comments must be set");
        }
    }
}