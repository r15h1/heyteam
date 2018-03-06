using HeyTeam.Core;
using HeyTeam.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.EvaluationViewModels
{
    public class PlayerReportCardModel
    {
        public string PlayerName { get; set; }
        public Guid PlayerId { get; set; }
        public string SquadName { get; set; }
        public Guid SquadId { get; set; }
        public string TermTitle { get; set; }
        public Guid TermId { get; set; }
        public PlayerReportCard ReportCard { get; set; }
    }
}
