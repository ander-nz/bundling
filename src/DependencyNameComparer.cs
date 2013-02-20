using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Arraybracket.Bundling {
	public sealed class DependencyNameComparer : IEqualityComparer<string> {
		public bool Equals(string x, string y) {
			return StringComparer.Ordinal.Equals(this._Normalize(x), this._Normalize(y));
		}

		public int GetHashCode(string obj) {
			return StringComparer.Ordinal.GetHashCode(this._Normalize(obj));
		}

		private string _Normalize(string value) {
			value = value.ToLowerInvariant();
			value = value.Replace("-", ".");
			value = value.Replace(".d.ts", ".js");
			value = value.Replace(".min.", ".");
			value = value.Replace(".pack.", ".");
			value = value.Replace(".custom.", ".");
			value = value.Replace(".intellisense.", ".");
			value = value.Replace(".vsdoc.", ".");
			value = Regex.Replace(value, @"\.(([0-9]*|[A-Za-z])\.)+", ".");
			return value;
		}
	}
}