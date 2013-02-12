using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Arraybracket.Bundling.Tests {
	internal static class ShouldExtensions {
		public static void Should(this object actual, IResolveConstraint expression) {
			Assert.That(actual, expression);
		}

		public static void ShouldEqual(this object actual, object expected) {
			actual.Should(Be.EqualTo(expected));
		}
	}

	internal class Be : Is {
	}

	internal class Have : Has {
	}

	internal class Contain : Contains {
	}
}