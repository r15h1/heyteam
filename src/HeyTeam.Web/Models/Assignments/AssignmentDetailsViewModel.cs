using HeyTeam.Core.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.Assignments {
	public class AssignmentDetailsViewModel
    {
        public Guid? Guid { get; set; }
        public DateTime? SubmittedOn { get; set; }
        				
		public DateTime? DateDue { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

		[Required]
		public AssignedTo? AssignedTo { get; set; }

		[Required]
        public string Instructions { get; set; }
        public IEnumerable<Guid> TrainingMaterials { get; set; }
        public IEnumerable<Guid> Squads { get; set; }
        public IEnumerable<Guid> Players { get; set; }
        public IEnumerable<SelectListItem> SquadList { get; internal set; }        
        public string SelectedTrainingMaterialList { get; internal set; }
        public string SelectedPlayerList { get; internal set; }
    }	
}