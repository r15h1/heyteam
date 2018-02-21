using Dapper;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Repositories
{
    public class TrackerRepository : ITrackerRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public TrackerRepository(IDbConnectionFactory factory)
        {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
        }

        public void Track(EventTrainingMaterialViewRequest request)
        {
            string sql = @" INSERT INTO EventTrainingMaterialViews (EventId, TrainingMaterialId, PlayerId, CoachId, ViewedOn)
                            VALUES(
                                (SELECT EventId FROM Events WHERE Guid = @EventGuid),
                                (SELECT TrainingMaterialId FROM TrainingMaterials WHERE Guid = @TrainingMaterialGuid),
                                (SELECT PlayerId FROM Players WHERE Guid = @PlayerGuid),
                                (SELECT CoachId FROM Coaches WHERE Guid = @CoachGuid),
                                GetDate()
                        )";

            var parameters = new DynamicParameters();
            parameters.Add("@EventGuid", request.EventId.ToString());
            parameters.Add("@TrainingMaterialGuid", request.TrainingMaterialId.ToString());
            parameters.Add("@PlayerGuid", request.Membership == Core.Membership.Player ? request.MemberId.ToString() : null );
            parameters.Add("@CoachGuid", request.Membership == Core.Membership.Coach ? request.MemberId.ToString() : null);

            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                connection.Execute(sql, parameters);
            }
        }
    }
}
