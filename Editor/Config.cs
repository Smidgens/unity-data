// smidgens @ github

namespace Smidgenomics.Unity.Variables.Editor
{
	/// <summary>
	/// magic constants, lt.dan
	/// </summary>
	internal static class Config
	{
		public static class Label
		{
			public const string NO_FUNCTION_SET = "No Function";
		}

		public static class WrappedMethod
		{
			public const float FIXED_BREAKPOINT = 300f;
			public const double PADDING = 2.0;

			// [target][method]
			public static readonly float[]
			SIZES_FLUID = { 0.4f, 0.6f },
			SIZES_FIXED = { 120f, 1f };
		}
	}
}