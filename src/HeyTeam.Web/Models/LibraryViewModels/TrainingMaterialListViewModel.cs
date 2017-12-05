using HeyTeam.Util;
using System;
using System.Linq;

namespace HeyTeam.Web.Models.LibraryViewModels {
	public class TrainingMaterialListViewModel
    {
		public Guid Guid { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Url { get; set; }
		public string ThumbnailUrl { get; set; }
		public string ContentType { get; set; }
		public bool IsVideo{
			get => !ContentType.IsEmpty() && ContentType.ToLowerInvariant().Contains("video");
		}
	}
}
