Arraybracket.Bundling
=====================

<code>ScriptDependencyOrderer</code> is an implementation of <code>IBundleOrderer</code> for *.js and *.ts files that sorts any dependencies specified through <code>/// &lt; reference /></code> tags.

Visual Studio's JavaScript editor uses <code>/// &lt; reference /> tags</code> to assist intellisense. Similarly, TypeScript uses the same tags to specify dependencies (for both intellisense and code verification). <code>ScriptDependencyOrderer</code> will scan all the included files in a <code>ScriptBundle</code> and ensure that the specified dependencies are sorted appropriately.

Some JavaScript libraries have separate *.intellisense.js or *.d.ts files, and <code>ScriptDependencyOrderer</code> will resolve those by ensuring that the similarly-named *.js file is sorted first.

Usage
=====

From your web project, add a NuGet package reference by running:

```
Install-Package Arraybracket.Bundling
```

Then, when constructing a <code>ScriptBundle</code>, set the bundle's <code>Orderer</code> property to an instance of <code>ScriptDependencyOrderer</code>. The below example specified the required change:

```csharp
var scriptBundle = new ScriptBundle("~/scripts/combined");
scriptBundle.Include("~/scripts/libs/*.js");
scriptBundle.Include("~/scripts/*.ts");
scriptBundle.Transforms.Add(new JsTransformer());
scriptBundle.Orderer = new ScriptDependencyOrderer(); // add this line
BundleTable.Bundles.Add(scriptBundle);
```

Relative Paths
==============

A <code>/// &lt; reference /></code> tag, in both *.js and *.ts files, specifies a path relative to the file containing the reference. In the scenario where:

* <code>~/app.js</code> references <code>helpers/jquery.fancybox.js</code>
* <code>~/helpers/jquery.fancybox.js</code> references <code>core/jquery.js</code>

then the sorted order will be:

* <code>~/helpers/core/jquery.js</code>
* <code>~/helpers/jquery.fancybox.js</code>
* <code>~/app.js</code>

Name Matching
=============

To support Visual Studio's JavaScript intellisense and TypeScript correctly, certain names are treated as the same for the purposes of detecting dependencies. Different extensions, different suffixes, or optional version numbers are excluded in the comparison. For example:

* <code>jquery-1.9.0.js</code> matches <code>jquery-vsdoc.js</code>
* <code>jquery-1.9.0.js</code> matches <code>jquery-1.9.0-vsdoc.js</code>
* <code>jquery-1.9.0.js</code> matches <code>jquery.intellisense.js</code>
* <code>jquery.min.js</code> matches <code>jquery-1.9.d.ts</code>
* <code>jquery-ui-1.10.custom.js</code> matches <code>jquery-ui.d.ts</code>

Excluded Dependencies
=====================

To ensure that name matching doesn't miss any references, <code>ScriptDependencyOrderer</code> will throw an exception if a dependency is referenced but cannot be found in the bundle. Since certain libraries have to be included separately from the bundle (such as Modernizr), but may still be referenced as a dependency (i.e. modernizr.d.ts), you can specify an excluded dependency by setting <code>ExcludedDependencies</code> on the orderer.

```csharp
var orderer = new ScriptDependencyOrderer();
orderer.ExcludedDependencies.Add("modernizr"); // excludes any files whose file name starts with "modernizr"
scriptBundle.Orderer = orderer;
```

Note that, by default, modernizr is already an excluded dependency.

Contributors
============

Special thanks to the following contributors:

* [Jean-Yves Boudreau](https://github.com/jyboudreau)