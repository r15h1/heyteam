using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Settings
{
	public class SmtpConfiguration {
		public SmtpSettings SmtpSettings { get; set; }
	}
	public class SmtpSettings
    {
		public string FromEmailAddress { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
