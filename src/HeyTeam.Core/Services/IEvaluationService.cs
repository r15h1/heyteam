using System;

namespace HeyTeam.Core.Services {
	public interface IEvaluationService 
	{
		Response CreateTerm(TermSetupRequest request);
		Response UpdateTerm(TermSetupRequest request);
		Response DeleteTerm(TermDeleteRequest request);
		Response UpdateStatus(TermStatusUpdateRequest request);
		(Guid? Guid, Response Response) GeneratePlayerReportCard(GenerateReportCardRequest request);
        Response UpdatePlayerReportCard(UpdateReportCardRequest request);
    }

	public class GenerateReportCardRequest {
		public Guid ClubId { get; set; }
		public Guid TermId { get; set; }
		public Guid SquadId{ get; set; }
		public Guid PlayerId { get; set; }
		public Guid ReportDesignId { get; set; }
	}

    public class UpdateReportCardRequest
    {
        public Guid ClubId { get; set; }
        public Guid TermId { get; set; }
        public Guid SquadId { get; set; }
        public Guid PlayerId { get; set; }
        public Guid ReportCardId { get; set; }
        public Guid? SkillId { get; set; }
        public ReportCardGrade? ReportCardGrade { get; set; }
        public string Comments { get; set; }
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
		public DateTime	StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}    
}