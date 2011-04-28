using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Newtonsoft.Json.Linq;

namespace Cargowire.CIBridge.HookParsers
{
	public class GitHubHookParser : IHookParser
	{
		/// <summary>Translates an http get/post from the Source control solution into a generic format
		/// for use with a build engine</summary>
		public IEnumerable<HookInfo> Parse(NameValueCollection requestData)
		{
			string jsonData = requestData["payload"];
			if (!string.IsNullOrEmpty(jsonData))
			{
				string json = jsonData;
				JObject o = JObject.Parse(json);
				User u = null;
				Repository r = new Repository { Name = (string)((JObject)o["repository"])["name"], Uri = new Uri((string)((JObject)o["repository"])["url"]) };
				List<Commit> commits = new List<Commit>();
				foreach(JObject commit in (JArray)o["commits"]){
					commits.Add(new Commit { Author = u, Id = (string)commit["id"], DateTime = DateTime.Parse((string)commit["timestamp"]), Uri = new Uri((string)commit["url"]) });
				}
				r.Commits = commits;
				return new List<HookInfo>(new HookInfo[]{ new HookInfo { User = u, Repository = r } });
			}
			return null;
		}
	}
}