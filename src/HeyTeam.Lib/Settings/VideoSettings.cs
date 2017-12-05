using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Settings
{
    public class VideoConfiguration
    {
		public VideoSettings VideoSettings { get; set; }
    }

	public class VideoSettings {
		public string Token { get; set; }
		public string Password { get; set; }
	}
}
