using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arraybracket.Bundling.Tests.ScriptDependencyOrdererTests {
	[TestClass]
	public sealed class ExtendedTests : TestBase {
		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void CyclicDependenciesShouldCauseAnError() {
			var lib1 = this._WriteFile("lib1.js", @"
/// <reference path=""lib2.js"" />
alert('1');
");

			var lib2 = this._WriteFile("lib2.js", @"
/// <reference path=""lib1.js"" />
alert('2');
");

			this._ExecuteOrderingFor(lib1, lib2);
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void UnknownDependenciesShouldCauseAnError() {
			var app = this._WriteFile("app.js", @"
/// <reference path=""lib.js"" />
alert('2');
");
			
			this._ExecuteOrderingFor(app);
		}

		[TestMethod]
		public void ADependencyShouldBeMatchedInACaseInsensitiveManner() {
			throw new NotImplementedException();
		}

		[TestMethod]
		public void ADependencyOnADeclarationFileShouldCauseTheRelatedFileToPrecedeItsUsage() {
			throw new NotImplementedException();
		}

		[TestMethod]
		public void ADependencyOnAnIntellisenseFileShouldCauseTheRelatedFileToPrecedeItsUsage() {
			throw new NotImplementedException();
		}

		[TestMethod]
		public void ATransitiveDependencyPathShouldBeRelativeToTheIncludingDependency() {
			throw new NotImplementedException();
		}

		[TestMethod]
		public void ADuplicateDependencyThatDiffersByCaseOrSuffixOrRelativePathShouldNotBeRepeated() {
			throw new NotImplementedException();
		}
	}
}