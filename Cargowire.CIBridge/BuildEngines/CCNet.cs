using System.Collections.Generic;
using System.Linq;
using System.Net;

using ThoughtWorks.CruiseControl.Remote;

namespace Cargowire.CIBridge
{
	/// <summary>Implementation of IBuildEngine that intends to use the ThoughtWorks libraries</summary>
	public class CCNet : CCNetBase
	{
		public CCNet(string url, string server, string user, string source)
			: base(url, server, user, source) {	}
		public CCNet(string url, string server, string user, string source, NetworkCredential credentials)
			: base(url, server, user, source, credentials) {	}

		public override void ForceBuild(string projectName)
		{
			ForceBuild(projectName, null);
		}
		public override void ForceBuild(string projectName, Branch branch)
		{
			projectName = NegotiateProjectName(projectName, branch);
			Open("/ViewFarmReport.aspx").Request(projectName, new IntegrationRequest(BuildCondition.ForceBuild, Source, User));
		}

		public override IEnumerable<ProjectStatus> GetStatus()
		{
			return Open().GetProjectStatus().ToList().Select(ps => new ProjectStatus { Name = ps.Name, Activity = ps.Activity.ToString(), Status = ps.Status.ToString(), LastBuildDate = ps.LastBuildDate, LastBuildStatus = ps.BuildStatus.ToString() });
		}

		private CruiseServerHttpClient Open()
		{
			return Open("/");
		}
		private CruiseServerHttpClient Open(string path)
		{
			if (this.Credentials == null)
			{
				return new CruiseServerHttpClient(string.Concat(Url, path));
			}
			else
			{
				var wc = new WebClient();
				wc.Credentials = this.Credentials;
				return new CruiseServerHttpClient(string.Concat(Url, path), wc);
			}
		}
	}
}