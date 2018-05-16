using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.FeedbackViewModels
{
    public class FeedbackChainModel
    {
		public MiniFeedbackChain FeedbackChain { get; set; }
		public bool IsMember { get; set; } = false;
	}
}
