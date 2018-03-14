using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Util;

namespace HeyTeam.Core.Models
{
    public class ReportCard
    {
		private readonly IDictionary<string, string> facets = new Dictionary<string, string>() {
			{ ReportCardFacetKeys.CoachesComments, "" },
			{ ReportCardFacetKeys.OverallGrade, "" },
			{ ReportCardFacetKeys.Level, "" },
		};

		public ReportCard(Guid reportCardId)
        {
            Guid = reportCardId;
        }
        public Guid Guid { get; }
        public MiniModel Design { get; set; }
        public ICollection<MiniReportCardHeadline> Headlines { get; } = new List<MiniReportCardHeadline>();
        public void AddHeadline(MiniReportCardHeadline headline)
        {
            if (headline != null && !headline.Guid.IsEmpty() && !Headlines.Any(h => h.Guid == headline.Guid))
            {
                Headlines.Add(headline);
            }
        }
        public void AddArea(MiniReportCardArea area)
        {
            if (area != null && !area.Guid.IsEmpty() && !area.HeadlineId.IsEmpty())
            {
                var targetHeadline = Headlines.SingleOrDefault(h => h.Guid == area.HeadlineId);
                if (targetHeadline != null)
                {
                    targetHeadline.AddArea(area);
                }
            }
        }
        public void AddSkill(MiniReportCardSkill skill)
        {
            if (skill != null && !skill.Guid.IsEmpty() && !skill.AreaId.IsEmpty())
            {
                var targetArea = Headlines.SelectMany(h => h.Areas).SingleOrDefault(a => a.Guid == skill.AreaId);
                targetArea.AddSkill(skill);
            }
        }		
		public IEnumerable<ReportCardFacet> Facets { get => facets.Select(f => new ReportCardFacet { Key = f.Key, Value = f.Value }); } 
		public void AddFacet(string key, string value) {
			if(!key.IsEmpty()){
				if (facets.ContainsKey(key)) {
					facets[key] = value ?? string.Empty;					
				} else {
					facets.Add(key, value ?? string.Empty);
				}
			}
		}
    }

	public class ReportCardFacet{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public static class ReportCardFacetKeys {
		public const string CoachesComments = "CoachesComments";
		public const string OverallGrade = "OverallGrade";
		public const string Level = "Level";
	}
}
