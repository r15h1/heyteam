using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.ApiModels
{
    public class FeedbackSearchModel
    {
        public Guid SquadId { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
    }
}
