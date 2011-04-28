using System;
using System.Collections.Generic;

namespace Cargowire.CIBridge
{
	public class Repository
	{
		public string Name { get; set; }
		public Uri Uri { get; set; }
		public IEnumerable<Commit> Commits { get; set; }
	}
}