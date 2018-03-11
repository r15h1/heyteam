using System;

namespace HeyTeam.Core.Models.Mini
{
    public class MiniModel
    {
        public MiniModel(Guid guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        public Guid Guid { get; }
        public string Name { get; }
    }
}
