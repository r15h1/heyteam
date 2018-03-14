using HeyTeam.Web.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EvaluationViewModels
{
    public class ReportSkillModel
    {
		[Required]
		[NotEmpty(ErrorMessage ="SquadId cannot be an empty guid")]
		public Guid SquadId{ get; set; }

		[Required]
		[NotEmpty(ErrorMessage = "TermId cannot be an empty guid")]
		public Guid TermId { get; set; }

		[Required]
		[NotEmpty(ErrorMessage = "PlayerId cannot be an empty guid")]
		public Guid PlayerId { get; set; }

		public Guid? SkillId { get; set; }

        public int? ReportCardGradeId { get; set; }

		public string FacetKey{ get; set; }
		public string FacetValue { get; set; }
	}
}
