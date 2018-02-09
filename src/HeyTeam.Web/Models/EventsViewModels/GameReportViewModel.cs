using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EventsViewModels {
	public class GameReportViewModel
    {
		public IEnumerable<string> MyProperty { get; set; }

		[Required]
		public string Opponent { get; set; }

		[Required]
		public string Result { get; set; }

		[Required]
		public string FinalScore { get; set; }
		
		public IEnumerable<string> Scorers { get; set; }
	}
}
