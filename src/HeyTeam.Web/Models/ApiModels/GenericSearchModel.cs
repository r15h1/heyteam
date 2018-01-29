using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.ApiModels
{
    public class GenericSearchModel
    {
		[FromQuery(Name="q")]
		[Required]
		public string Query { get; set; }

		[FromQuery(Name = "page")]
		public int Page { get; set; } = 1;

		[FromQuery(Name = "limit")]
		public int Limit { get; set; } = 10;
	}
}
