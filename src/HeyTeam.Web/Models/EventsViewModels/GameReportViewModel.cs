using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EventsViewModels {
	public class GameReportViewModel
    {
		[Required]
		public Guid EventId { get; set; }

		[Required]
		public string Opponent { get; set; }

		[Required]
		public string Result { get; set; }

		[Required]
		[Display(Name = "Final Score")]
		public string FinalScore { get; set; }
		
		public IEnumerable<string> Scorers { get; set; }

		[Display(Name = "Coach's Remarks")]
		public string Remarks { get; set; }
	}
}
