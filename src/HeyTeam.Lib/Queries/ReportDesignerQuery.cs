using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Queries
{
    public class ReportDesignerQuery : IReportDesignerQuery
    {
        private readonly IDbConnectionFactory connectionFactory;

        public ReportDesignerQuery(IDbConnectionFactory factory)
        {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
        }

        public ReportCardDesign GetReportCardDesign(Guid reportDesignId)
        {
            string sql = $@"SELECT R.Guid AS DesignGuid, C.Guid AS ClubGuid, R.Name
                            FROM ReportCardDesigns R 
                            INNER JOIN Clubs C ON R.ClubId = C.ClubId
                            WHERE (R.Deleted IS NULL OR R.Deleted = 0) AND R.Guid = @ReportDesignGuid";

            DynamicParameters p = new DynamicParameters();
            p.Add("@ReportDesignGuid", reportDesignId.ToString());
                        
            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var design = reader.Select<dynamic, ReportCardDesign>(
                        row => new ReportCardDesign(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.DesignGuid.ToString()))
                        {
                            DesignName = row.Name
                        }).SingleOrDefault();

                return design;
            }
        }

        public IEnumerable<ReportCardDesign> GetReportCardDesigns(Guid clubId, string name = null)
        {
            string sql = $@"SELECT R.Guid AS DesignGuid, C.Guid AS ClubGuid, R.Name
                            FROM ReportCardDesigns R 
                            INNER JOIN Clubs C ON R.ClubId = C.ClubId AND C.Guid = @ClubGuid
                            WHERE (R.Deleted IS NULL OR R.Deleted = 0)
                                {(name.IsEmpty() ? "" : " AND LOWER(R.Name) = @Name")}";

            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", clubId.ToString());

            if(!name.IsEmpty())
                p.Add("@Name", name);

            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var designs = reader.Select<dynamic, ReportCardDesign>(
                        row => new ReportCardDesign(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.DesignGuid.ToString()))
                        {
                            DesignName = row.Name
                        }).ToList();

                return designs;
            }
        }
    }
}
