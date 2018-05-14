using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HeyTeam.Core.Dashboard;

namespace HeyTeam.Web.Models.SquadViewModels {
    public class SquadViewModel {        

        [Required]
        [MaxLength(100)]
        public string SquadName { get; set; }

        [Required]
        [Range(1990, 2100)]
        public short YearBorn { get; set; }
    }
}