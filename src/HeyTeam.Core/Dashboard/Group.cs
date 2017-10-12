using System.Collections.Generic;

namespace HeyTeam.Core.Dashboard {
    public class Group {
        public string Name { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}