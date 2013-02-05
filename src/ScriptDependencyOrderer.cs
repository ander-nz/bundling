using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Optimization;

namespace Arraybracket.Bundling {
	public sealed class ScriptDependencyOrderer : IBundleOrderer {
		public IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files) {
			throw new NotImplementedException();
		}
	}
}