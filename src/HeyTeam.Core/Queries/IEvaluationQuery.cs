﻿using HeyTeam.Core.Models;
using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IEvaluationQuery {
		IEnumerable<PlayerReportCard> GetPlayerReportCards(Guid clubId, Guid termId, Guid squadId);
		IEnumerable<MiniReportCard> GetPlayerReportCards(Guid clubId, Guid playerId);
		PlayerReportCard GetPlayerReportCard(Guid clubId, Guid termId, Guid squadId, Guid playerId);
        PlayerReportCard GetPlayerReportCard(Guid clubId, Guid reportCardId);
        PlayerEvaluation GetPlayerReportCardDetails(Guid clubId, Guid playerReportCardId);
		IEnumerable<Term> GetTerms(Guid clubId, DateTime? startDate = null, DateTime? endDate = null, TermStatus? status = null);
		Term GetTerm(Guid termId);
    }
}
