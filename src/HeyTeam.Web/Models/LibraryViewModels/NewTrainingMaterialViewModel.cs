using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HeyTeam.Util;

namespace HeyTeam.Web.Models.LibraryViewModels {
	public class NewTrainingMaterialViewModel
    {
		public Guid? Guid { get; set; }

		[Required]
		[MaxLength(250)]
		public string Title { get; set; }	
		
		[MaxLength(1000)]
		public string Description { get; set; }	
		
		[Required(ErrorMessage = "Please select a file")]		
		public IFormFile File { get; set; }

		public string ContentType {
			get {
				if (File != null) 
					return File.ContentType;
				
				return string.Empty;
			}
		}

		public string FileName {
			get {
				if (File != null)
					return File.FileName;

				return string.Empty;
			}
		}
	}
}