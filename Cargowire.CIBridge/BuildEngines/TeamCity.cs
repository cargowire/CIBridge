using System;
using System.Collections.Generic;
using System.Net;
using Cargowire.CIBridge.Configuration;
using TeamCitySharp;
using TeamCitySharp.DomainEntities;

namespace Cargowire.CIBridge
{
	/// <summary>Implementation of IBuildEngine using Paul Stacks TeamCitySharp</summary>
	public class TeamCity : IBuildEngine
	{
		public Uri Url { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public TeamCity(Uri url, string username, string password)
		{
			Url = url;
			Username = username;
			Password = password;
		}

		public void ForceBuild(string projectName)
		{
			ForceBuild(projectName, null);
		}
		public void ForceBuild(string projectName, Branch branch)
		{
			projectName = NegotiateProjectName(projectName, null);

			var client = Open();
			BuildConfig config = client.BuildConfigByConfigurationName(projectName);

			HttpWebRequest wr = WebRequest.Create(string.Concat(Url, string.Format("/httpAuth/action.html?add2Queue={0}&branch_project3={1}", config.Id, (branch.IsMaster) ? "<default>" : branch.FullName))) as HttpWebRequest;
			wr.Method = "post";
			wr.ContentType = "application/x-www-form-urlencoded";
			wr.Credentials = new NetworkCredential(this.Username, this.Password);

			wr.GetResponse();
		}

		public IEnumerable<ProjectStatus> GetStatus()
		{
			throw new NotImplementedException();
		}

		protected string NegotiateProjectName(string projectName, Branch branch)
		{
			return ProjectMapSection.Instance.GetBuildName(projectName);
		}

		private TeamCityClient Open()
		{
			return Open(this.Url);
		}

		private TeamCityClient Open(Uri uri)
		{
			var client = new TeamCityClient(string.Concat(uri.Host,":", uri.Port), (string.Compare(uri.Scheme, "https")==0));
			client.Connect(this.Username, this.Password);

			return client;
		}
	}
}