Arraybracket.Bundling
=====================

Arraybracket.Bundling contains two useful improvements for the ASP.NET Web Optimization Framework.

<code>CssUrlVersioner</code> is a post-processor for <code>StyleBundle</code> instances that computes a hash for any <code>url()</code> values, causing the images to be reloaded if they're modified.

<code>ScriptDependencyOrderer</code> is an implementation of <code>IBundleOrderer</code> for *.js and *.ts files that sorts any dependencies specified through <code>/// &lt; reference /></code> tags.

Using CssUrlVersioner
=====================

**Warning: CssUrlVersioner is currently available in pre-release form only.**

From your web project, add a NuGet package reference by running:

```
Install-Package Arraybracket.Bundling -Pre
```

Then, when constructing a <code>StyleBundle</code>, add an instance of <code>CssUrlVersioner</code> to the <code>Transforms</code> property. The below example specified the required change:

```csharp
var styleBundle = new StyleBundle("~/styled/combined");
styleBundle.Include("~/styles/libs/*.css");
styleBundle.Include("~/styles/*.less");
styleBundle.Transforms.Add(new CssTransformer());
styleBundle.Transforms.Add(new CssUrlVersioner()); // add this line
BundleTable.Bundles.Add(styleBundle);
```

Note: if you have multiple <code>Transforms</code> values specified, as in the example above, <code>CssUrlVersioner</code> should be the last one.

Using ScriptDependencyOrderer
=============================

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

More information
================

Please see the [wiki](https://github.com/arraybracket/bundling/wiki) for more information.

Contributors
============

Special thanks to the following contributors:

* [Jean-Yves Boudreau](https://github.com/jyboudreau)