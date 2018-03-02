using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.EvaluationViewModels
{
    public class ReportCardDesignViewModel
    {        
        [Required]
        [MaxLength(100)]
        public string ReporCardDesignName { get; set; }

        [Required]
        public Guid TermId { get; set; }
    }
}
