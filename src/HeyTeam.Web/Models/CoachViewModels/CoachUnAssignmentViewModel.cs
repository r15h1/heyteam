using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.CoachViewModels
{
    public class CoachUnAssignmentViewModel {

		[Required]
		public Guid? CoachId { get; set; }

		[Required]
		public Guid? SquadId { get; set; }
    }
}
