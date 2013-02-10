using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arraybracket.Bundling.Tests.ScriptDependencyOrdererTests {
	public abstract class TestBase : IDisposable {
		private readonly DependencyNameComparer _Comparer = new DependencyNameComparer();
		private readonly string _ContentPath = Path.Combine(Path.GetTempPath(), "Arraybracket.Bundling.Tests");
		
		public void Dispose() {
			if (Directory.Exists(this._ContentPath))
				Directory.Delete(this._ContentPath, true);
		}

		protected string _WriteFile(string name, string content) {
			var path = Path.GetFullPath(Path.Combine(this._ContentPath, name));
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, content);
			return path;
		}
		
		/// <remarks>This method is marked as <see cref="PureAttribute">Pure</see> so that code analysis ensures that its return value is consumed.</remarks>
		[Pure]
		protected _AssertOrderingHelper _AssertOrderingFor(params string[] inputFileNames) {
			var inputFiles = inputFileNames.Select(i => new FileInfo(i)).ToArray();
			var actualFiles = new ScriptDependencyOrderer().OrderFiles(null, inputFiles).ToArray();
			return new _AssertOrderingHelper(actualFiles.Select(a => a.FullName).ToArray());
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
			var inputFiles = inputFileNames.Select(i => new FileInfo(i)).ToArray();
			var actualFiles = new ScriptDependencyOrderer().OrderFiles(null, inputFiles).ToArray();
			TextWriter.Null.Write(actualFiles);
		}

		protected void _AssertNameComparison(bool expected, string name1, string name2) {
			Assert.AreEqual(expected, this._Comparer.Equals(name1, name2));
			if (expected)
				Assert.AreEqual(this._Comparer.GetHashCode(name1), this._Comparer.GetHashCode(name2));
		}
	}
}