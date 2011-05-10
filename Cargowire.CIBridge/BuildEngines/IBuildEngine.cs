using System.Collections.Generic;

namespace Cargowire.CIBridge
{
	public interface IBuildEngine
	{
		/// <summary>Immediately forces a build</summary>
		void ForceBuild(string projectName);

		/// <summary>Immediately forces a build</summary>
		void ForceBuild(string projectName, Branch branch);

		/// <summary>Retrives the current status</summary>
		IEnumerable<ProjectStatus> GetStatus();
	}
}