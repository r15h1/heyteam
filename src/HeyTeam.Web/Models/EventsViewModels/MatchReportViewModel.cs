﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeyTeam.Web.Models.EventsViewModels {
	public class MatchReportViewModel
    {
		[Required]
		public Guid EventId { get; set; }

		[Required]
		public string Opponent { get; set; }

		[Required]
		[Display(Name = "Goals scored by our team")]
		public byte? GoalsScored { get; set; }

		[Required]
		[Display(Name = "Goals scored by opponent")]
		public byte? GoalsConceeded { get; set; }

		public string Scorers { get; set; }

		[Display(Name = "Coach's Remarks")]
		public string CoachsRemarks { get; set; }
		public bool ReportExists { get; set; } = false;
		public string EventTitle { get; set; }
		public string EventDetails { get; set; }
		public List<SelectListItem> Players { get; internal set; }
	}
}
