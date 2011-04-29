using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using System.Xml.Linq;

namespace Cargowire.CIBridge
{
	/// <summary>Implementation of IBuildEngine that uses the web dashboard as an api</summary>
	public class ManualCCNet : CCNetBase
	{
		public ManualCCNet(string url, string server, string user, string source)
			: base(url, server, user, source)
		{
		}
		public ManualCCNet(string url, string server, string user, string source, NetworkCredential credentials)
			: base(url, server, user, source, credentials)
		{
		}

		public override void ForceBuild(string projectName)
		{
			HttpWebRequest wr = WebRequest.Create(string.Concat(Url,"/ViewFarmReport.aspx")) as HttpWebRequest;
			wr.Method = "post";
			wr.ContentType = "application/x-www-form-urlencoded";
			if (this.Credentials != null)
				wr.Credentials = this.Credentials;
				
			string body = string.Format("ForceBuild=true&projectName={0}&serverName={1}", projectName, this.Server);
			byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
			wr.ContentLength = bodyBytes.Length;
			using (var dataStream = wr.GetRequestStream())
			{
				dataStream.Write(bodyBytes, 0, bodyBytes.Length);
				dataStream.Close();
			}
			wr.GetResponse();
		}

		public override IEnumerable<ProjectStatus> GetStatus()
		{
			WebRequest wr = WebRequest.Create(string.Concat(Url, "/XmlServerReport.aspx"));
			if (this.Credentials != null)
				wr.Credentials = this.Credentials;
			var response = wr.GetResponse();
			
			XDocument xmlReport = XDocument.Load(response.GetResponseStream());
			response.Close();
			return from projects in xmlReport.Descendants("Project")
				   select new ProjectStatus 
				   { 
					   Name = projects.Attribute("name").Value,
					   LastBuildDate = DateTime.Parse(projects.Attribute("lastBuildTime").Value),
					   LastBuildStatus = projects.Attribute("lastBuildStatus").Value,
					   Status = projects.Attribute("status").Value,
					   Activity = projects.Attribute("activity").Value
				   };
		}
	}
}
