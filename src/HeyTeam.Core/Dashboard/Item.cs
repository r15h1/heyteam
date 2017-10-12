using System.Collections.Generic;

namespace HeyTeam.Core.Dashboard {
    public class Item {
        public IDictionary<string, string> Cells { get;set; } = new Dictionary<string, string>();
    }
}