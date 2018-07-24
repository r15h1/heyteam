using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models.Mini
{
    public class MiniReportCard
    {
		public MiniReportCard(Guid guid){
			this.Guid = guid;
		}

		public string Title { get; set; }
		public Guid Guid{ get; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
