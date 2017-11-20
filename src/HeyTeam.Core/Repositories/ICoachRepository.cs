using HeyTeam.Core.Entities;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Repositories {
	public interface ICoachRepository 
	{		
		void AddCoach(Coach coach);		
		void UpdateCoach(Coach coach);
	}	
}
