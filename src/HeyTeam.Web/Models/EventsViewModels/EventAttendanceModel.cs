﻿using HeyTeam.Core.Models;
using System;
using System.Collections.Generic;

namespace HeyTeam.Web.Models
{
    public class EventAttendanceModel
    {
		public Guid EventId { get; set; }
		public string Title { get; set; }
		public string Location { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public IEnumerable<EventPlayer> EventPlayers { get; set; }
		public string EventType { get; set; }
	}
}