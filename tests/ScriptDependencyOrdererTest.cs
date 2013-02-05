using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arraybracket.Bundling.Tests {
	[TestClass]
	public sealed class ScriptDependencyOrdererTest : ScriptDependencyOrdererTestBase {
		[TestMethod]
		public void TestASingleDirectDependency() {
			var lib = this._WriteFile("lib.js", @"
alert('1');
");

			var app = this._WriteFile("app.js", @"
/// <reference path=""lib.js"" />
alert('2');
");

			this._AssertOrderingFor(lib, app).Expect(lib).Expect(app).Complete();
			this._AssertOrderingFor(app, lib).Expect(lib).Expect(app).Complete();
		}

		[TestMethod]
		public void TestTwoDirectDependencies() {
			var lib1 = this._WriteFile("lib1.js", @"
alert('1');
");

			var lib2 = this._WriteFile("lib2.js", @"
alert('2');
");

			var app = this._WriteFile("app.js", @"
/// <reference path=""lib1.js"" />
/// <reference path=""lib2.js"" />
alert('3');
");

			this._AssertOrderingFor(lib1, lib2, app).Expect(lib1, lib2).Expect(app).Complete();
			this._AssertOrderingFor(app, lib2, lib1).Expect(lib1, lib2).Expect(app).Complete();

			app = this._WriteFile("app.js", @"
/// <reference path=""lib2.js"" />
/// <reference path=""lib1.js"" />
alert('3');
");

			this._AssertOrderingFor(lib1, lib2, app).Expect(lib1, lib2).Expect(app).Complete();
			this._AssertOrderingFor(app, lib2, lib1).Expect(lib1, lib2).Expect(app).Complete();
		}

		[TestMethod]
		public void TestAnIndirectDependency() {
			var libcore = this._WriteFile("libcore.js", @"
alert('1');
");

			var libaddons = this._WriteFile("libaddons.js", @"
/// <reference path=""libcore.js"" />
alert('2');
");

			var app = this._WriteFile("app.js", @"
/// <reference path=""libaddons.js"" />
alert('3');
");

			this._AssertOrderingFor(libcore, libaddons, app).Expect(libcore).Expect(libaddons).Expect(app).Complete();
			this._AssertOrderingFor(app, libaddons, libcore).Expect(libcore).Expect(libaddons).Expect(app).Complete();
		}

		[TestMethod]
		public void TestADuplicateDependency() {
			var libcore = this._WriteFile("libcore.js", @"
alert('1');
");

			var libaddons = this._WriteFile("libaddons.js", @"
/// <reference path=""libcore.js"" />
alert('2');
");

			var app = this._WriteFile("app.js", @"
/// <reference path=""libcore.js"" />
/// <reference path=""libaddons.js"" />
alert('3');
");

			this._AssertOrderingFor(libcore, libaddons, app).Expect(libcore).Expect(libaddons).Expect(app).Complete();
			this._AssertOrderingFor(app, libaddons, libcore).Expect(libcore).Expect(libaddons).Expect(app).Complete();

			app = this._WriteFile("app.js", @"
/// <reference path=""libcore.js"" />
/// <reference path=""libaddons.js"" />
alert('3');
");

			this._AssertOrderingFor(libcore, libaddons, app).Expect(libcore).Expect(libaddons).Expect(app).Complete();
			this._AssertOrderingFor(app, libaddons, libcore).Expect(libcore).Expect(libaddons).Expect(app).Complete();
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void TestThatACyclicDependencyCausesAnError() {
			var lib1 = this._WriteFile("lib1.js", @"
/// <reference path=""lib2.js"" />
alert('1');
");

			var lib2 = this._WriteFile("lib2.js", @"
/// <reference path=""lib1.js"" />
alert('2');
");

			var inputFiles = new[] { lib1, lib2 }.Select(i => new FileInfo(i)).ToArray();
			var actualFiles = new ScriptDependencyOrderer().OrderFiles(null, inputFiles).ToArray();
			TextWriter.Null.Write(actualFiles);
		}
	}
}