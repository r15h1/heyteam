using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Settings
{
	public class CryptographicConfiguration {
		public CryptographicSettings CryptographicSettings { get; set; }
	}
	public class CryptographicSettings
    {
		public string RegistrationPurposeKey { get; set; }
		public string KeysPersistencePathToFileSystem { get; set; }
	}
}
