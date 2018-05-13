using System;
using HeyTeam.Util;

namespace HeyTeam.Core.Models.Mini
{
    public class MiniFeedback
    {
        public MiniFeedback(Guid guid)
        {
            if (!guid.IsEmpty()) {
                this.Guid = guid;
            }
        }

        public Guid? Guid { get; }
        public MiniModel Player { get; set; }
        public string WeeklyNotes { get; set; }
        public string LatestComment { get; set; }
        public DateTime? PublishedOn { get; set; }
        public string FormmattedPublishedDate {
            get {
                if (PublishedOn.HasValue)
                    return PublishedOn.Value.ToString("dd-MMM-yyyy");
                
                return string.Empty;
            }
        }
    }
}
