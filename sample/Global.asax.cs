using System;
using System.Web;
using System.Web.Optimization;
using BundleTransformer.Core.Transformers;

namespace Arraybracket.Bundling.Sample {
	public class Global : HttpApplication {
		protected void Application_Start(object sender, EventArgs args) {
			var scriptBundle = new ScriptBundle("~/Scripts/combined");
			scriptBundle.Include("~/Scripts/libs/*.js");
			scriptBundle.Include("~/Scripts/*.ts");
			scriptBundle.Transforms.Clear();
			scriptBundle.Transforms.Add(new JsTransformer());
			scriptBundle.Orderer = new ScriptDependencyOrderer();
			BundleTable.Bundles.Add(scriptBundle);

			var styleBundle = new StyleBundle("~/Styles/combined");
			styleBundle.Include("~/Styles/*.less");
			styleBundle.Transforms.Clear();
			styleBundle.Transforms.Add(new CssTransformer());
			styleBundle.Transforms.Add(new CssUrlVersioner());
			BundleTable.Bundles.Add(styleBundle);
		}
	}
}