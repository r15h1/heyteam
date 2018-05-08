namespace HeyTeam.Core.Repositories {
	public interface ICoachRepository 
	{		
		void AddCoach(Coach coach);		
		void UpdateCoach(Coach coach);
		void DeleteCoach(Coach coach);
	}	
}
