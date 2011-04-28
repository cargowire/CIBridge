using System;

namespace Cargowire.CIBridge
{
	public class Commit
	{
		public string Id { get; set; }
		public DateTime DateTime { get; set; }
		public Uri Uri { get; set; }
		public User Author { get; set; }
	}
}