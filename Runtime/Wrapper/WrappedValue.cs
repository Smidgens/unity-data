////// smidgens @ github


//namespace Smidgenomics.Unity.Variables
//{
//	using System;
//	using UVector2 = UnityEngine.Vector2;
//	using UVector3 = UnityEngine.Vector3;
//	using UColor = UnityEngine.Color;

//	/// <summary>
//	/// Concrete type refs
//	/// </summary>
//	public static partial class WrappedValue
//	{
//		[Serializable] public class Float : WrappedValue<float> { }
//		[Serializable] public class Int : WrappedValue<int> { }
//		[Serializable] public class String : WrappedValue<string> { }
//		[Serializable] public class Vector2 : WrappedValue<UVector2> { }
//		[Serializable] public class Vector3 : WrappedValue<UVector3> { }
//		[Serializable]
//		public class Color : WrappedValue<UColor>
//		{
//			public static Color White => new Color(UColor.white);
//			public static Color Black => new Color(UColor.black);
//			public static Color Clear => new Color(UColor.clear);
//			public Color(UColor staticValue) => SetStatic(staticValue);
//		}
//	}
//}

//namespace Smidgenomics.Unity.Variables
//{
//	using System;
//	using UnityCurve = UnityEngine.AnimationCurve;

//	partial class WrappedValue
//	{
//		[Serializable]
//		public class AnimationCurve : WrappedValue<UnityCurve>
//		{
//			public static AnimationCurve Linear01 => new AnimationCurve(UnityCurve.Linear(0f, 0f, 1f, 1f));
//			public static AnimationCurve EaseInOut01 => new AnimationCurve(UnityCurve.EaseInOut(0f, 0f, 1f, 1f));
//			public AnimationCurve(UnityCurve staticValue = default) => SetStatic(staticValue);
//			public static AnimationCurve Linear(float ts, float vs, float te, float ve)
//			{
//				return new AnimationCurve(UnityCurve.Linear(ts, vs, te, ve));
//			}
//			public static AnimationCurve EaseInOut(float ts, float vs, float te, float ve)
//			{
//				return new AnimationCurve(UnityCurve.EaseInOut(ts, vs, te, ve));
//			}
//		}
//	}
//}