using HeyTeam.Core;
using HeyTeam.Core.Models.Mini;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.Assignments
{
    public class AssignmentDetailsViewModel
    {
        public Guid? Guid { get; set; }
        public DateTime? SubmittedOn { get; set; }

        [Required]
        public int CoachId { get; set; }

        public DateTime? DateDue { get; set; }

        [Required]
        public string Notes { get; set; }

        public IEnumerable<TrainingMaterial> TrainingMaterials { get; set; }

        public IEnumerable<MiniModel> Squads { get; set; }

        public IEnumerable<MiniModel> Players { get; set; }
        public IEnumerable<SelectListItem> SquadList { get; internal set; }
    }
}