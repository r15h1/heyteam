using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Models.LibraryViewModels {
	public class TrainingMaterialDetailsViewModel
    {
		public Guid? Guid { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string ContentType { get; set; }
		public string ExternalId { get; set; }
		public string Url { get; set; }
		public string ThumbnailUrl { get; set; }

		public string Status { 
			get => Url.IsEmpty() ? "not available yet" : "available"; 
		}
		public bool IsVideo {
			get => !ContentType.IsEmpty() && ContentType.ToLowerInvariant().Contains("video");
		}
		public List<string> Errors { get; set; }
		public string ShortContentType { get; set; }
	}
}
