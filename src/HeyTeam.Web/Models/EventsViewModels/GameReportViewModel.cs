using Microsoft.AspNetCore.Mvc.Rendering;
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
		[Display(Name = "Goals Scored")]
		public byte GoalsScored { get; set; }

		[Required]
		[Display(Name = "Goals Conceeded")]
		public byte GoalsConceeded { get; set; }

		public IEnumerable<string> Scorers { get; set; }

		[Display(Name = "Coach's Remarks")]
		public string Remarks { get; set; }

		[Display(Name = "Send Email To")]
		public IEnumerable<string> EmailTo { get; set; }

		public IEnumerable<Guid> Recipients{ get; set; }
		public List<SelectListItem> SquadPlayers { get; set; }

		public string EventTitle { get; set; }
		public string EventDetails { get; set; }
	}
}
