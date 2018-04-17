using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.ApiModels
{
    public class UpdateDueDateModel
    {
		[Required]
		public Guid? AssignmentId { get; set; }

		[Required]
		public DateTime? DueDate { get; set; }
	}
}