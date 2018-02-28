using HeyTeam.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EvaluationViewModels {
	public class TermModel
    {
		public Guid? Guid { get; }

		[Required(ErrorMessage = "Start Date is required")]		
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "End Date is required")]
		public DateTime EndDate { get; set; }

		[Required(ErrorMessage = "Title is required")]
		[MaxLength(100, ErrorMessage = "Maximum length of Title is 100")]
		public string Title { get; set; }		
	}
}
