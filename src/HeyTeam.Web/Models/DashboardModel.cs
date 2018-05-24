using HeyTeam.Core;
using System;

namespace HeyTeam.Web.Models {
	public class DashboardModel
    {
		public Guid MemberId { get; set; }	
		public Membership Membership{ get; set; }
		public string MembershipArea { 
			get{
				return Membership == Membership.Coach ? "coaches" : "players";
			} 
		}
    }
}
