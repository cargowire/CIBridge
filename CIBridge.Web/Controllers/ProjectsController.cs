using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using Cargowire.CIBridge;
using Cargowire.CIBridge.HookParsers;

namespace CIBridge.Web.Controllers
{
	public class ProjectsController : Controller
	{
		public string CCNetUrl { get { return ConfigurationManager.AppSettings["CCNetUrl"]; } }
		public string CCNetServer { get { return ConfigurationManager.AppSettings["CCNetServer"]; } }
		public string CCNetUser { get { return ConfigurationManager.AppSettings["CCNetUser"]; } }
		public string CCNetSource { get { return ConfigurationManager.AppSettings["CCNetSource"]; } }
		public string CCNetServerUsername { get { return ConfigurationManager.AppSettings["CCNetServerUsername"]; } }
		public string CCNetServerPassword { get { return ConfigurationManager.AppSettings["CCNetServerPassword"]; } }

		public IBuildEngine GetBuildEngine()
		{
			IBuildEngine engine;
			if (!string.IsNullOrEmpty(CCNetServerUsername))
				//engine = new CCNet(CCNetUrl, CCNetServer, CCNetUser, CCNetSource, new System.Net.NetworkCredential(CCNetServerUsername, CCNetServerPassword));
				engine = new ManualCCNet(CCNetUrl, CCNetServer, CCNetUser, CCNetSource, new System.Net.NetworkCredential(CCNetServerUsername, CCNetServerPassword));
			else
				//engine = new CCNet(CCNetUrl, CCNetUser, CCNetSource);
				engine = new ManualCCNet(CCNetUrl, CCNetServer, CCNetUser, CCNetSource);
				
			return engine;
		}

		public ActionResult Index()
		{
			IBuildEngine engine = GetBuildEngine();
			IEnumerable<ProjectStatus> statuses = engine.GetStatus();
			return View(statuses);
		}

		/// <param name="name">Project name/identifier (if supplied assumed to be the repo to build regardless of other request data)</param>
		[HttpPost]
		public ActionResult Build(string name)
		{
			string values = Request.Form.ToString();
			try
			{
				Log(string.Concat("Form: ", Request.Form.ToString()));
				IEnumerable<HookInfo> hi;
				if (!string.IsNullOrEmpty(name))
					hi = new List<HookInfo> { new HookInfo{ Repository = new Repository { Name = name }, User = null } };
				else
				{
					var parser = new CodebaseHqHookParser();
					hi = parser.Parse(Request.Form);
				}
				if (hi.Count() > 0)
				{
					IBuildEngine engine = GetBuildEngine();
					Log(string.Concat("Repository: ", hi.First().Repository.Name, "(", (hi.First().Branch ?? new Branch()).Name, ")"));
					engine.ForceBuild(hi.First().Repository.Name, hi.First().Branch);
				}
				Log(Environment.NewLine);
			}
			catch (Exception ex)
			{
				Log(string.Concat("Error:", ex.Message));
				throw;
			}
			
			return (string.IsNullOrEmpty(name)) ? View() : (ActionResult)RedirectToAction("index");
		}

		private void Log(string log)
		{
			using(StreamWriter sw = new StreamWriter(HttpContext.Server.MapPath("~/log.txt"),true))
			{
				sw.WriteLine(string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ": ", log));
				sw.Close();
			}
		}
	}
}
