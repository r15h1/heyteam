using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.AccountViewModels
{
    public class UserListViewModel
    {
		public string UserId { get; set; }
		public string Email { get; set; }
		public IEnumerable<string> Names { get; set; } = new List<string>();
		public IEnumerable<string> Access { get; set; } = new List<string>();
		public bool? AccountIsLocked { get; set; }
	}
}
