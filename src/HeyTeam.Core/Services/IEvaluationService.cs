using System;

namespace HeyTeam.Core.Services {
	public interface IEvaluationService 
	{
		Response CreateTerm(TermSetupRequest request);
		Response UpdateTerm(TermSetupRequest request);
		Response DeleteTerm(TermDeleteRequest request);
		Response UpdateStatus(TermStatusUpdateRequest request);
	}

	public class TermStatusUpdateRequest {
		public Guid ClubId { get; set; }
		public Guid TermId { get; set; }
		public TermStatus TermStatus { get; set; }
	}

	public class TermDeleteRequest {
		public Guid ClubId{ get; set; }
		public Guid TermId { get; set; }
	}

	public class TermSetupRequest 
	{
		public Guid ClubId{ get; set; }
		public Guid? TermId{ get; set; }
		public string Title { get; set; }
		public TermPeriod PeriodStart { get; set; }
		public TermPeriod PeriodEnd { get; set; }
	}
}