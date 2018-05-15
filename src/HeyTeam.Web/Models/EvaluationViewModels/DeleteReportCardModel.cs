using HeyTeam.Web.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.EvaluationViewModels
{
    public class DeleteReportCardModel
    {
        [Required]
        [NotEmpty(ErrorMessage = "SquadId cannot be an empty guid")]
        public Guid SquadId { get; set; }

        [Required]
        [NotEmpty(ErrorMessage = "ReportCardId cannot be an empty guid")]
        public Guid ReportCardId { get; set; }

        [Required]
        [NotEmpty(ErrorMessage = "PlayerId cannot be an empty guid")]
        public Guid PlayerId { get; set; }

        [Required]
        [NotEmpty(ErrorMessage = "TermId cannot be an empty guid")]
        public Guid TermId { get; set; }
    }
}
