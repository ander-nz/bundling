using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Optimization;

namespace Arraybracket.Bundling {
	public sealed class ScriptDependencyOrderer : IBundleOrderer {
		private static readonly Regex _ReferenceRegex = new Regex(@"///\s*<reference\s+path=""(?<path>[^""]*)""\s*/>");

		public List<string> ExcludedDependencies = new List<string> {
			"modernizr",
		};

		public IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files) {
			var workingItems = files.AsParallel().Select(i => new _WorkingItem {
				Path = i.FullName,
				FileInfo = i,
				Dependencies = this._GetDependencies(i.FullName),
			});

			var fileDependencies = new Dictionary<string, _WorkingItem>(new DependencyNameComparer());
			foreach (var item in workingItems) {
				_WorkingItem duplicate;
				if (fileDependencies.TryGetValue(item.Path, out duplicate))
					throw new ArgumentException(string.Format("During dependency resolution, a collision between '{0}' and '{1}' was detected. Files in a bundle must not collide with respect to the dependency name comparer.", Path.GetFileName(item.Path), Path.GetFileName(duplicate.Path)));

				fileDependencies.Add(item.Path, item);
			}

			foreach (var item in fileDependencies.Values) {
				foreach (var dependency in item.Dependencies) {
					if (!fileDependencies.ContainsKey(dependency))
						throw new ArgumentException(string.Format("Dependency '{0}' referenced by '{1}' could not found. Ensure the dependency is part of the bundle and its name can be detected by the dependency name comparer. If the dependency is not supposed to be in the bundle, add it to the list of excluded dependencies.", Path.GetFileName(dependency), Path.GetFileName(item.Path)));
				}
			}

			while (fileDependencies.Count > 0) {
				var result = fileDependencies.Values.FirstOrDefault(f => f.Dependencies.All(d => !fileDependencies.ContainsKey(d)));
				if (result == null)
					throw new ArgumentException(string.Format("During dependency resolution, a cyclic dependency was detected among the remaining dependencies {0}.", string.Join(", ", fileDependencies.Select(d => "'" + Path.GetFileName(d.Value.Path) + "'"))));
				yield return result.FileInfo;
				fileDependencies.Remove(result.Path);
			}
		}
		
		private string[] _GetDependencies(string path) {
			var dir = Path.GetDirectoryName(path);

			return _ReferenceRegex.Matches(File.ReadAllText(path)).Cast<Match>().Select(m => {
				var relativePath = m.Groups["path"].Value;
				return Path.GetFullPath(Path.Combine(dir, relativePath));
			}).Where(m => this.ExcludedDependencies.All(e => !m.Contains(@"\" + e))).ToArray();
		}

		private sealed class _WorkingItem {
			public string Path;
			public FileInfo FileInfo;
			public string[] Dependencies;
		}
	}
}