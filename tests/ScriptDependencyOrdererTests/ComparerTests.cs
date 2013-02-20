using NUnit.Framework;

namespace Arraybracket.Bundling.Tests.ScriptDependencyOrdererTests {
	[TestFixture]
	public sealed class ComparerTests : TestBase {
		[Test]
		public void EqualNamesShouldBeConsideredEqual() {
			this._AssertNameComparison(true, "lib.js", "lib.js");
			this._AssertNameComparison(true, "helpers.js", "helpers.js");
		}

		[Test]
		public void DifferentNamesShouldNotBeConsideredEqual() {
			this._AssertNameComparison(false, "lib.js", "helpers.js");
			this._AssertNameComparison(false, "lib.js", "app.js");
			this._AssertNameComparison(false, "lib.core.js", "libcore.js");
		}

		[Test]
		public void NamesThatDifferInCaseShouldBeConsideredEqual() {
			this._AssertNameComparison(true, "lib.JS", "Lib.js");
			this._AssertNameComparison(true, "TEST.JS", "TEST.JS");
		}

		[Test]
		public void DeclarationFilesShouldBeConsideredEqual() {
			this._AssertNameComparison(true, "lib.js", "lib.d.ts");
			this._AssertNameComparison(true, "lib.js", "lib.vsdoc.js");
			this._AssertNameComparison(true, "lib.js", "lib.intellisense.js");
			this._AssertNameComparison(true, "lib.js", "lib-d.ts");
			this._AssertNameComparison(true, "lib.js", "lib-vsdoc.js");
			this._AssertNameComparison(true, "lib.js", "lib-intellisense.js");
		}

		[Test]
		public void MinifiedAndCustomFilesShouldBeConsideredEqual() {
			this._AssertNameComparison(true, "lib.js", "lib.min.js");
			this._AssertNameComparison(true, "lib.js", "lib.pack.js");
			this._AssertNameComparison(true, "lib.js", "lib.custom.min.js");
			this._AssertNameComparison(true, "lib.js", "lib.custom.pack.js");
			this._AssertNameComparison(true, "lib.js", "lib.min.custom.js");
			this._AssertNameComparison(true, "lib.js", "lib.pack.custom.js");
		}

		[Test]
		public void VersionedFilesShouldBeConsideredEqual() {
			this._AssertNameComparison(true, "lib.js", "lib.1.9.0.js");
			this._AssertNameComparison(true, "lib.js", "lib.1.9.js");
			this._AssertNameComparison(true, "lib.js", "lib.1.js");
			this._AssertNameComparison(true, "lib.js", "lib-1.9.0.js");
			this._AssertNameComparison(true, "lib.js", "lib-1.9.js");
			this._AssertNameComparison(true, "lib.js", "lib-1.js");
			this._AssertNameComparison(true, "lib.js", "lib-2.3.a.1.js");
		}

		[Test]
		public void ExtensionsShouldNotBeConsideredEqual() {
			this._AssertNameComparison(false, "lib.js", "lib.validation.js");
			this._AssertNameComparison(false, "lib.js", "lib-validation.js");
		}

		[Test]
		public void CombinationsShouldStack() {
			this._AssertNameComparison(true, "LIB-1.9.0.custom.min.JS", "Lib.d.tS");
			this._AssertNameComparison(true, "LIB.intellisense.1.9.JS", "Lib-1.9.pack.jS");
			this._AssertNameComparison(false, "LIB.validation.1.9.JS", "Lib-1.9.pack.jS");
		}
	}
}