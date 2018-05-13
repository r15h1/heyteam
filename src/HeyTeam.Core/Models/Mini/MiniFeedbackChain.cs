using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models.Mini
{
    public class MiniFeedbackChain
    {
        public MiniFeedback Feedback { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public List<string> Comments { get; set; }
    }
}