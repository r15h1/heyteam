using System;
using HeyTeam.Core.Services;

namespace HeyTeam.Core.Repositories {
	public interface IEvaluationRepository
    {
		void AddTerm(TermSetupRequest request);
		void UpdateTerm(TermSetupRequest request);
		void DeleteTerm(TermDeleteRequest request);
		Guid GeneratePlayerReportCard(PlayerReportCardGenerationRequest request);
	}
}
