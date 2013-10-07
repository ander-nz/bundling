using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Optimization;
using NUnit.Framework;

namespace Arraybracket.Bundling.Tests.ScriptDependencyOrdererTests {
	public abstract class TestBase : IDisposable {
		private static readonly string _ContentPath = Path.Combine(Path.GetTempPath(), "Arraybracket.Bundling.Tests");
		private readonly DependencyNameComparer _Comparer = new DependencyNameComparer();
		
		public void Dispose() {
			if (Directory.Exists(_ContentPath))
				Directory.Delete(_ContentPath, true);
		}

		protected string _WriteFile(string name, string content) {
			var path = Path.GetFullPath(Path.Combine(_ContentPath, name));
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, content);
			return path;
		}
		
		/// <remarks>This method is marked as <see cref="PureAttribute">Pure</see> so that code analysis ensures that its return value is consumed.</remarks>
		[Pure]
		protected _AssertOrderingHelper _AssertOrderingFor(ScriptDependencyOrderer orderer, params string[] inputFileNames) {
			var inputFiles = _GetBundleFiles(inputFileNames);
			var actualFiles = orderer.OrderFiles(null, inputFiles).ToArray();
			return new _AssertOrderingHelper(actualFiles.Select(a => a.IncludedVirtualPath).ToArray());
		}

		/// <remarks>This method is marked as <see cref="PureAttribute">Pure</see> so that code analysis ensures that its return value is consumed.</remarks>
		[Pure]
		protected _AssertOrderingHelper _AssertOrderingFor(params string[] inputFileNames) {
			return this._AssertOrderingFor(new ScriptDependencyOrderer(), inputFileNames);
		}

		/// <remarks>This method is marked as <see cref="PureAttribute">Pure</see> so that code analysis ensures that its return value is consumed.</remarks>
		[Pure]
		protected _AssertOrderingHelper _AssertOrderingFor(IEqualityComparer<string> dependencyNameComparer, params string[] inputFileNames) {
			return this._AssertOrderingFor(new ScriptDependencyOrderer(dependencyNameComparer), inputFileNames);
		}

		protected sealed class _AssertOrderingHelper {
			private string[] _OrderedFileNames;
			private int _Index;

			public _AssertOrderingHelper(string[] orderedFileNames) {
				this._OrderedFileNames = orderedFileNames;
				this._Index = 0;
			}

			/// <remarks>This method is marked as <see cref="PureAttribute">Pure</see> so that code analysis ensures that the chain is terminated by <see cref="Complete"/>.</remarks>
			[Pure]
			public _AssertOrderingHelper Expect(params string[] expectedFileNames) {
				expectedFileNames = expectedFileNames.Select(_GetVirtualPath).ToArray();
				var actual = this._OrderedFileNames.Skip(this._Index).Take(expectedFileNames.Length).ToArray();
				CollectionAssert.AreEquivalent(expectedFileNames, actual);
				this._Index += expectedFileNames.Length;
				return this;
			}

			public void Complete() {
				Assert.AreEqual(this._OrderedFileNames.Length, this._Index);
			}
		}

		protected void _ExecuteOrderingFor(params string[] inputFileNames) {
			var inputFiles = _GetBundleFiles(inputFileNames);
			var actualFiles = new ScriptDependencyOrderer().OrderFiles(null, inputFiles).ToArray();
			TextWriter.Null.Write(actualFiles);
		}

		protected void _ExecuteOrderingFor(IEqualityComparer<string> dependencyNameComparer, params string[] inputFileNames) {
			var inputFiles = _GetBundleFiles(inputFileNames);
			var actualFiles = new ScriptDependencyOrderer(dependencyNameComparer).OrderFiles(null, inputFiles).ToArray();
			TextWriter.Null.Write(actualFiles);
		}

		protected void _AssertNameComparison(bool expected, string name1, string name2) {
			Assert.AreEqual(expected, this._Comparer.Equals(name1, name2));
			if (expected)
				Assert.AreEqual(this._Comparer.GetHashCode(name1), this._Comparer.GetHashCode(name2));
		}

		private static BundleFile[] _GetBundleFiles(IEnumerable<string> inputFileNames) {
			return inputFileNames.Select(fullPath => {
				var virtualPath = _GetVirtualPath(fullPath);
				return new BundleFile(virtualPath, new _StubVirtualFile(virtualPath, fullPath));
			}).ToArray();
		}

		private static string _GetVirtualPath(string fullPath) {
			return fullPath.Replace(_ContentPath, "").Replace('\\', '/');
		}

		private sealed class _StubVirtualFile : VirtualFile {
			private readonly string _FullPath;

			public _StubVirtualFile(string virtualPath, string fullPath) : base(virtualPath) {
				this._FullPath = fullPath;
			}

			public override Stream Open() {
				return File.OpenRead(this._FullPath);
			}
		}
	}
}