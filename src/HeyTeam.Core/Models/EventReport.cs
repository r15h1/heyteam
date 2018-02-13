using System;
using System.Xml;
using HeyTeam.Util;

namespace HeyTeam.Core.Models {
	public class EventReport
    {
		public EventReport(Guid eventId){
			if (eventId.IsEmpty())
				throw new ArgumentNullException();

			EventId = eventId;
		}

		public Guid EventId { get; }
		public XmlDocument Report { get; set; }
		public DateTime LastUpdatedOn { get; set; }
	}
}
