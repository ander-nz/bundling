using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

namespace Arraybracket.Bundling {
	public sealed class CssUrlVersioner : IBundleTransform {
		private static readonly Regex _UrlRegex = new Regex(@"url\((?<url>[^\)]+)\)", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public void Process(BundleContext context, BundleResponse response) {
			var cssDir = Path.GetDirectoryName(context.HttpContext.Server.MapPath(context.BundleVirtualPath));

			response.Content = _UrlRegex.Replace(response.Content, match => {
				var url = match.Groups["url"].Value;
				url = url.Trim('\'', '"');

				if (url.Contains("//"))
					return match.Value;

				var path = url.StartsWith("/") ? context.HttpContext.Server.MapPath(url) : Path.GetFullPath(Path.Combine(cssDir, url.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar)));
				if (!File.Exists(path))
					return match.Value;

				context.HttpContext.Response.AddFileDependency(path);
				return "url(" + url + "?v=" + this.GetContentHashCode(File.ReadAllBytes(path)) + ")";
			});
		}

		internal string GetContentHashCode(byte[] content) {
			if (content.Length == 0) {
				return "";
			} else {
				using (SHA256 hashAlgorithm = CreateHashAlgorithm())
					return HttpServerUtility.UrlTokenEncode(hashAlgorithm.ComputeHash(content));
			}
		}

		private static SHA256 CreateHashAlgorithm() {
			if (CryptoConfig.AllowOnlyFipsAlgorithms)
				return (SHA256)new SHA256CryptoServiceProvider();
			else
				return (SHA256)new SHA256Managed();
		}
	}
}