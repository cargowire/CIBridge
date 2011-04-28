using System;

namespace Cargowire.CIBridge
{
	public class ProjectStatus
	{
		public DateTime LastBuildDate { get; set; }
		public string Name { get; set; }
		public string LastBuildStatus { get; set; }
		public string Status { get; set; }
		public string Activity { get; set; }
	}
}
