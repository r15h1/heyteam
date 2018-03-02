using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories
{
    public class ReportDesignerRepository : IReportDesignerRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public ReportDesignerRepository(IDbConnectionFactory factory)
        {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
        }

        public void AddReportDesign(ReportDesign reportDesign)
        {
            string sql = @"INSERT INTO ReportCardDesigns(Guid, Name, ClubId) 
                    VALUES( @ReportCardDesignGuid, @Name, 
                            (SELECT ClubId FROM Clubs WHERE Guid = @ClubGuid))";

            DynamicParameters p = new DynamicParameters();
            p.Add("@ReportCardDesignGuid", reportDesign.Guid.ToString());
            p.Add("@Name", reportDesign.Name);
            p.Add("@ClubGuid", reportDesign.ClubId.ToString());

            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                connection.Execute(sql, p);
            }
        }
    }
}
