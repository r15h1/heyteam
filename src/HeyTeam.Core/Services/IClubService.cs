﻿namespace HeyTeam.Core.Services {
	public interface IClubService
    {
		Response RegisterClub(Club club);
		Response UpdateClubProfile(Club club);
	}
}
