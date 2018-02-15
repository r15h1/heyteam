using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EventsViewModels {
	public class EmailReportViewModel
    {
		[Required]
		public Guid EventId { get; set; }

		[Required]
		public List<string> EmailAddress { get; set; }

		public bool? SendMeACopy { get; set; }
	}
}
