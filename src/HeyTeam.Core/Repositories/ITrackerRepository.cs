using HeyTeam.Core.Services;

namespace HeyTeam.Core.Repositories
{
    public interface ITrackerRepository
    {
        void Track(EventTrainingMaterialViewRequest request);
		void Track(AssignmentTrainingMaterialViewRequest request);
	}
}
