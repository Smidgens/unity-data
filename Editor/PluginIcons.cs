// smidgens @ github

namespace Smidgenomics.Unity.Data.Editor
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;

	internal enum PIcon
	{
		// row 1
		Int, Float, Bool, String,
		// row 2
		Vector2, Vector3, Color, Curve,
		// row 3
		RefType, ValueType,
		// row 4
		Arrow = 12, Variable = 13, Search = 14,
	}

	internal static class PluginIcons
	{
		public const byte CELLS = 4; // 4x4 grid

		/// <summary>
		/// Finds appropriate atlas icon for given type
		/// </summary>
		public static PIcon Find(Type type)
		{
			if(_TYPE_MAP.TryGetValue(type, out var ic)) { return ic; }
			if (type.IsValueType) { return PIcon.ValueType; }
			return PIcon.RefType;
		}

		/// <summary>
		/// Draw icon
		/// </summary>
		public static void Draw(in Rect pos, in PIcon n)
		{
			var (x, y) = GetCell(n);
			DrawCell(pos, x, y);
		}

		private static (int,int) GetCell(in PIcon ico)
		{
			var i = (byte)ico;
			var x = i % CELLS;
			var y = i / CELLS;
			return (x, y);
		}

		// image atlas
		private static readonly Lazy<Texture>
		_ATLAS = new Lazy<Texture>(() => Resources.Load<Texture>(Config.ResourcePath.ICON_ATLAS));

		private static void DrawCell(in Rect pos, in int x, in int y)
		{
			using (new GUI.ClipScope(pos))
			{
				var size = pos.size * CELLS;
				var cellSize = size.y / CELLS;
				var roffset = new Vector2(x, y) * cellSize;
				var icoRect = new Rect(-roffset, size);
				GUI.DrawTexture(icoRect, _ATLAS.Value);
			}
		}

		// map types to indices
		private static Dictionary<Type, PIcon> _TYPE_MAP = new Dictionary<Type, PIcon>()
		{
			{ typeof(int), PIcon.Int },
			{ typeof(float), PIcon.Float },
			{ typeof(string), PIcon.String },
			{ typeof(bool), PIcon.Bool },
			{ typeof(Vector2), PIcon.Vector2 },
			{ typeof(Vector3), PIcon.Vector3 },
			{ typeof(Color), PIcon.Color },
			{ typeof(AnimationCurve), PIcon.Curve },
		};
	}
}