using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.AccountViewModels
{
    public class SendInvitationModel
    {
		public string Email { get; set; }
		public DateTime Expiry { get; set; }
		public string Club { get; set; }
	}
}
