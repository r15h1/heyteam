using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.FeedbackViewModels
{
    public class NewCommentModel
    {
        [Required]
        public Guid FeedbackId { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
