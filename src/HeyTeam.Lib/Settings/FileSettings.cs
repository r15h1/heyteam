using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Settings
{
    public class FileConfiguration
    {
		public FileSettings FileSettings { get; set; }
    }

	public class FileSettings {
		public string Directory { get; set; }
		public string RootUrl { get; set; }
	}
}
