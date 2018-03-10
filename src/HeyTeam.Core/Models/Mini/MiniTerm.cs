using System;

namespace HeyTeam.Core.Models.Mini
{
    public class MiniTerm : MiniModel
    {
        public MiniTerm(Guid guid, string name) : base(guid, name) {}

        public string Status { get; set; }
    }
}
