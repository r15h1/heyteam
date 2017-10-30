using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HeyTeam.Core.Dashboard;

namespace HeyTeam.Web.Models.SquadViewModels {
    public class SquadViewModel {        

        [Required]
        [MaxLength(100)]
        public string SquadName { get; set; }
    }
}