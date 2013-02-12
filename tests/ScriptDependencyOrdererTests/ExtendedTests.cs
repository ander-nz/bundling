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
		public void UnspecifiedUnknownDependenciesShouldCauseAnError() {
			var app = this._WriteFile("app.js", @"
/// <reference path=""lib.js"" />
alert('2');
");
			
			this._ExecuteOrderingFor(app);
		}

		[TestMethod]
		public void SpecifiedUnknownDependenciesShouldBeIgnored() {
			var app = this._WriteFile("app.js", @"
/// <reference path=""libs/modernizr.js"" />
alert('2');
");

			this._AssertOrderingFor(app).Expect(app).Complete();
		}

		[TestMethod]
		public void ATransitiveDependencyPathShouldBeRelativeToTheIncludingDependency() {
			var deepLib = this._WriteFile("libs/helpers/lib.js", @"
alert('1');
");

			var shallowLib = this._WriteFile("libs/lib.js", @"
/// <reference path=""helpers/lib.js"" />
alert('2');
");

			var rootLib = this._WriteFile("lib.js", @"
/// <reference path=""libs/lib.js"" />
alert('3');
");

			this._AssertOrderingFor(deepLib, shallowLib, rootLib).Expect(deepLib).Expect(shallowLib).Expect(rootLib).Complete();
			this._AssertOrderingFor(rootLib, shallowLib, deepLib).Expect(deepLib).Expect(shallowLib).Expect(rootLib).Complete();
		}

		[TestMethod]
		public void ADependencyReferenceShouldBeInsensitiveToCaseOrSuffix() {
			var lib = this._WriteFile("lib-1.9.1.CUSTOM.PACK.JS", @"
alert('1');
");

			var app = this._WriteFile("app.ts", @"
/// <reference path=""Lib.d.ts"" />
alert('2');
");

			this._AssertOrderingFor(lib, app).Expect(lib).Expect(app).Complete();
			this._AssertOrderingFor(app, lib).Expect(lib).Expect(app).Complete();
		}

		[TestMethod]
		public void ADuplicateDependencyThatDiffersByRelativePathOrCaseOrSuffixShouldNotBeRepeated() {
			var lib1 = this._WriteFile("libs/lib1-1.9.1.CUSTOM.PACK.JS", @"
alert('1');
");

			var lib2 = this._WriteFile("libs/lib2.js", @"
/// <reference path=""LIB1-vsdoc.js"" />
alert('2');
");

			var app = this._WriteFile("app.ts", @"
/// <reference path=""libs/lib1.d.ts"" />
/// <reference path=""libs/lib2.d.ts"" />
alert('3');
");

			this._AssertOrderingFor(lib1, lib2, app).Expect(lib1).Expect(lib2).Expect(app).Complete();
			this._AssertOrderingFor(app, lib2, lib1).Expect(lib1).Expect(lib2).Expect(app).Complete();
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void CollidingDependenciesShouldCauseAnError() {
			var lib1 = this._WriteFile("lib-vsdoc.js", @"
alert('1');
");

			var lib2 = this._WriteFile("lib-intellisense.js", @"
alert('2');
");

			var app = this._WriteFile("app.js", @"
/// <reference path=""lib-vsdoc.js"" />
/// <reference path=""lib-intellisense.js"" />
");

			this._ExecuteOrderingFor(lib1, lib2, app);
		}
	}
}