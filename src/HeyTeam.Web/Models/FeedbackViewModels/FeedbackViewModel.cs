using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HeyTeam.Web.Models.FeedbackViewModels
{
    public class FeedbackViewModel
    {
		public List<SelectListItem> Squads { get; set; }
	}
}