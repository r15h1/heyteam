﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.ApiModels
{
    public class AssignmentSearchModel
    {
		public Guid? PlayerId { get; set; }
		public Guid? SquadId { get; set; }
		public DateTime? Date{ get; set; }
	}
}