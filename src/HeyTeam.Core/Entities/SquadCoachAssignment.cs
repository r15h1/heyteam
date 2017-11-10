using HeyTeam.Core.Events;
using HeyTeam.Core.Exceptions;
using HeyTeam.Util;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core.Entities {
	public class SquadCoachAssignment {
		public delegate void CoachChanged(SquadCoachChanged e);

		public event CoachChanged OnCoachAdded;
		public event CoachChanged OnCoachRemoved;

		public SquadCoachAssignment(Squad squad, IList<Coach> coaches) {
			Ensure.ArgumentNotNull(squad);
			Squad = squad;
			Coaches = coaches ?? new List<Coach>();
		}

		public Squad Squad { get; }
		public IList<Coach> Coaches { get; }

		public void AddCoach(Coach coach) {
			if (Coaches.Any(c => c.Guid == coach.Guid))
				throw new DuplicateEntryException("Coach is already assigned to this squad");

			OnCoachAdded?.Invoke(new SquadCoachChanged(Squad.Guid, coach.Guid));
		}

		public void RemoveCoach(Coach coach) {
			if (!Coaches.Any(c => c.Guid == coach.Guid))
				throw new EntityNotFoundException("Coach is not assigned to this squad");

			OnCoachRemoved?.Invoke(new SquadCoachChanged(Squad.Guid, coach.Guid));
		}
	}	
}