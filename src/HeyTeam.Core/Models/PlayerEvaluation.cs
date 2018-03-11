using HeyTeam.Core.Models.Mini;

namespace HeyTeam.Core.Models
{
    public class PlayerEvaluation
    {
        public MiniModel Club { get; set; }        
        public MiniTerm Term { get; set; }
        public MiniModel Player { get; set; }
        public ReportCard ReportCard { get; set; }
    }
}
