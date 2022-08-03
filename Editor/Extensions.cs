// smidgens @ github

namespace Smidgenomics.Unity.Variables.Editor
{
	using UnityEngine;

	/// <summary>
	/// UnityEngine.Rect helpers
	/// </summary>
	internal static class Rect_
	{
		public static Rect[] SplitHorizontally(this Rect pos, double pad, params float[] widths)
		{
			var r = new Rect[widths.Length];
			if (widths.Length == 0) { return r; }
			var padding = GetSplitPadding(widths.Length, pos.width, pad);
			var totalSize = pos.width - padding.total;
			var w = totalSize.Split(widths);
			var offset = 0f;
			for (var i = 0; i < w.Length; i++)
			{
				r[i] = pos;
				r[i].x += offset;
				r[i].width = w[i];
				offset += w[i] + padding.offset;
			}
			return r;
		}

		private static SplitPad GetSplitPadding(int n, float v, double p)
		{
			if (n < 2) { return default; }
			var o = System.Convert.ToSingle(p);
			// ratio
			if (o < 1) { o = o * v; }
			return new SplitPad { offset = o, total = o * (n - 1) };
		}
		private struct SplitPad { public float offset, total; }
	}
}

namespace Smidgenomics.Unity.Variables.Editor
{
	using System.Linq;

	/// <summary>
	/// float helpers
	/// </summary>
	internal static class Float_
	{
		public static float[] Split(this float v, params float[] weights)
		{
			if (weights.Length == 0) { return new float[0]; }
			var flex = v;
			// absolute weights, >1
			foreach (var w in weights) { if (w > 1f) { flex -= w; } }
			return weights.Select((w, i) => w > 1f ? w : w * flex).ToArray();
		}
	}
}


namespace Smidgenomics.Unity.Variables.Editor
{
	using System;
	using System.Reflection;

	internal static class Reflection_
	{
		public static string GetStringifiedType(this object o, string defaultValue = "")
		{
			return EditorReflection.GetStringifiedType(o, defaultValue);
		}

		public static Type GetFirstGenericType(this FieldInfo fi)
		{
			var t = fi.FieldType;
			if (t.IsArray)
			{
				t = t.GetElementType();
			}
			var args = t.GetGenericArguments();
			return args.Length > 0 ? args[0] : null; ;
		}
	}
}