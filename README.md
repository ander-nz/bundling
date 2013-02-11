Arraybracket.Bundling
=====================

<code>ScriptDependencyOrderer</code> is an implementation of <code>IBundleOrderer</code> for *.js and *.ts files that sorted any dependencies specified through <code>/// &lt; reference /></code> tags.

Visual Studio's JavaScript editor uses <code>/// &lt; reference /> tags</code> to assist intellisense. Similarly, TypeScript uses the same tags to specify dependencies (for both intellisense and code verification). <code>ScriptDependencyOrderer</code> will scan all the included files in a <code>ScriptBundle</code> and ensure that the specified dependencies are sorted appropriately.

Some JavaScript libraries have separate *.intellisense.js or *.d.ts files, and <code>ScriptDependencyOrderer</code> will resolve those by ensuring that the similarly-named *.js file is sorted first.

Usage
=====

When constructing a <code>ScriptBundle</code>, set the bundle's <code>Orderer</code> property to an instance of <code>ScriptDependencyOrderer</code>. The below example highlights the specified change.

```csharp
var scriptBundle = new ScriptBundle();
scriptBundle.Include("~/scripts/libs/\*.js");
scriptBundle.Include("~/scripts/\*.ts");
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
