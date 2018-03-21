using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models
{
    public class TrainingMaterialView
    {
		public TrainingMaterialView(MiniTrainingMaterial trainingMaterial)
		{
			this.TrainingMaterial = trainingMaterial;
		}
		public MiniTrainingMaterial TrainingMaterial { get; }
		public IEnumerable<MiniMember> ViewedBy { get; } = new List<MiniMember>();
		public void AddViewer(MiniMember member) => ((ICollection<MiniMember>)ViewedBy).Add(member);
	}
}