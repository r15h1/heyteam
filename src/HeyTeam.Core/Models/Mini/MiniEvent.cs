using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models.Mini
{
	public class MiniEvent : MiniModel {
		public MiniEvent(Guid guid, string name) : base(guid, name) {
		}

		public string Location { get; set; }
		public string StartDate { get; set; }
		public EventType? EventType { get; set; }
		public string EventTypeDescription{ get; set; }
	}
}
