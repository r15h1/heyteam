using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models.Mini
{
	public class AssignmentDetails : AssignmentSummary {
		public AssignmentDetails(Guid clubId, Guid assignmentId) : base(clubId, assignmentId) {
		}

		public ICollection<MiniTrainingMaterial> TrainingMaterials { get; set; } = new List<MiniTrainingMaterial>();
		public ICollection<MiniModel> Players { get; set; } = new List<MiniModel>();
	}
}