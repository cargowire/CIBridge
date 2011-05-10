using System.Collections.Generic;
using System.Net;

using ThoughtWorks.CruiseControl.Remote;

namespace Cargowire.CIBridge
{
	public abstract class CCNetBase : ICCNet
	{
		public string Url { get; set; }
		public string Server { get; set; }
		public string User { get; set; }
		public string Source { get; set; }
		public NetworkCredential Credentials { get; set; }

		public CCNetBase(string url, string server, string user, string source)
		{
			this.Url = url;
			this.Server = server;
			this.User = user;
			this.Source = source;
		}
		public CCNetBase(string url, string server, string user, string source, NetworkCredential credentials)
			: this(url, server, user, source)
		{
			this.Credentials = credentials;
		}

		protected string NegotiateProjectName(string projectName, Branch branch)
		{
			if(branch != null && !branch.IsMaster && !string.IsNullOrEmpty(branch.Name))
				return string.Concat(projectName, "/", branch.Name);
			else
				return projectName;
		}

		public abstract void ForceBuild(string projectName);
		public abstract void ForceBuild(string projectName, Branch branch);
		public abstract IEnumerable<ProjectStatus> GetStatus();
	}
}