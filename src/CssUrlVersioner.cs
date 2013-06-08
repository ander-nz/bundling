using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

namespace Arraybracket.Bundling {
	public sealed class CssUrlVersioner : IBundleTransform {
		private static readonly Regex _UrlRegex = new Regex(@"url\((?<url>[^\)]+)\)", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public void Process(BundleContext context, BundleResponse response) {
			var httpServer = context.HttpContext.Server;
			var httpResponse = context.HttpContext.Response;
			var baseDir = Path.GetDirectoryName(httpServer.MapPath(context.BundleVirtualPath));

			using (var hashAlgorithm = this._CreateHashAlgorithm()) {
				response.Content = _UrlRegex.Replace(response.Content, match => {
					var url = match.Groups["url"].Value;
					url = url.Trim('\'', '"');

					if (this._IsExternal(url))
						return match.Value;

					var path = this._MapPath(httpServer, baseDir, url);
					if (!File.Exists(path))
						return match.Value;

					httpResponse.AddFileDependency(path);
					var hashCode = this._GetHashCode(hashAlgorithm, path);
					return string.Format("url({0}?v={1})", url, hashCode);
				});
			}
		}

		private HashAlgorithm _CreateHashAlgorithm() {
			if (CryptoConfig.AllowOnlyFipsAlgorithms)
				return new SHA256CryptoServiceProvider();
			else
				return new SHA256Managed();
		}

		private bool _IsExternal(string url) {
			return url.Contains("//");
		}

		private string _MapPath(HttpServerUtilityBase httpServer, string baseDir, string url) {
			if (url.StartsWith("/"))
				return httpServer.MapPath(url);
			else
				return Path.GetFullPath(Path.Combine(baseDir, url.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar)));
		}

		private string _GetHashCode(HashAlgorithm hashAlgorithm, string filePath) {
			return HttpServerUtility.UrlTokenEncode(hashAlgorithm.ComputeHash(File.ReadAllBytes(filePath)));
		}
	}
}