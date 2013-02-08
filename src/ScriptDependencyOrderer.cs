using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Optimization;

namespace Arraybracket.Bundling {
	public sealed class ScriptDependencyOrderer : IBundleOrderer {
		private static readonly Regex _ReferenceRegex = new Regex(@"///\s*<reference\s+path=""(?<path>[^""]*)""\s*/>");

		public IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files) {
			var fileDependencies = files.AsParallel().Select(i => new {
				Path = i.FullName,
				FileInfo = i,
				Dependencies = this._GetDependencies(i.FullName).ToArray(),
			}).ToDictionary(f => f.Path, StringComparer.OrdinalIgnoreCase);

			while (fileDependencies.Count > 0) {
				var result = fileDependencies.Values.First(f => f.Dependencies.All(d => !fileDependencies.ContainsKey(d)));
				yield return result.FileInfo;
				fileDependencies.Remove(result.Path);
			}
		}
		
		private IEnumerable<string> _GetDependencies(string path) {
			var dir = Path.GetDirectoryName(path);

			return _ReferenceRegex.Matches(File.ReadAllText(path)).Cast<Match>().Select(m => {
				var relativePath = m.Groups["path"].Value;
				return Path.GetFullPath(Path.Combine(dir, relativePath));
			});
		}
	}
}