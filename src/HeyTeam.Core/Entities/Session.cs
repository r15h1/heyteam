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
        public List<Evaluation> Evaluations { get; private set; } = new List<Evaluation>(); 
        public bool IsClosedForEvaluations { 
            get {
                return ClosingDateForEvaluations.HasValue && DateTime.Now > ClosingDateForEvaluations;
            }
        }
        public DateTime? ClosingDateForEvaluations { get; set; }

        internal void AddEvaluation(Evaluation evaluation) {
            Validate(evaluation);            
            Evaluations.Add(evaluation);
        }

        private void Validate(Evaluation evaluation) {
            if(!Id.HasValue) throw new IllegalOperationException ("Evaluations can be added to registered sessions only (sessions that have a valid id)");
            if(IsClosedForEvaluations) throw new IllegalOperationException("This session is closed for evaluations");
            if(evaluation == null) throw new ArgumentNullException(); 
            if(!evaluation.Evaluator.Id.HasValue) throw new IllegalOperationException ("Evaluations can be added by registered evaluators only (must have a valid id)");
            if(string.IsNullOrWhiteSpace(evaluation.Comments)) throw new IllegalOperationException ("Evaluation comments must be set");
        }

        public List<Squad> Squads { get; private set; } = new List<Squad>();
        public void AddSquad(Squad squad) {
            Validate(squad);
            Squads.Add(squad);
        }

        private void Validate(Squad squad) {
            
        }
    }
}