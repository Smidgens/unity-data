// smidgens @ github

namespace Smidgenomics.Unity.Variables
{
	using UnityEngine;
	using System;
	using System.Reflection;

	/// <summary>
	/// Reflection assisted reference to getter
	/// </summary>
	[Serializable]
	public class WrappedGetter<VT>
	{
		public VT Value => GetValue();

		[SerializeField] private MethodRef _ref = default;

		// method references cached outside editor
		private Func<VT> _cachedGetter = null;
		private MethodInfo _cachedInfo = null;

		private VT GetValue()
		{
			if(!Application.isEditor) { return GetValueCached(); }
			return GetValueUncached();
		}

		// loads getter using reflection
		private MethodInfo LoadMethod()
		{
			if (!_ref.target) { return null; }
			var tt = _ref.target.GetType();
			return ReflectionUtility.FindMethod(_ref.method, tt, typeof(VT));
		}

		// loads method anew before value
		private VT GetValueUncached()
		{
			var m = LoadMethod();
			return m != null ? (VT)m.Invoke(_ref.target, new object[0]) : default;
		}

		// loads and caches method info before value
		private VT GetValueCached()
		{
			if(_cachedGetter == null)
			{
				_cachedInfo = LoadMethod();
				if(_cachedInfo != null) { _cachedGetter = GetMethodValue; }
				else { _cachedGetter = GetDefaultValue; }
			}
			return _cachedGetter.Invoke();
		}

		private VT GetDefaultValue() => default;

		private VT GetMethodValue()
		{
			return (VT)_cachedInfo.Invoke(_ref.target, new object[0]);
		}

		[Serializable]
		internal struct MethodRef
		{
			public UnityEngine.Object target;
			public string type, method;
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Variables.Editor
{
	internal static partial class SPHelper
	{
		public static class WrappedGetter
		{
			public const string
			TARGET = "_ref.target",
			TYPE = "_ref.type",
			METHOD = "_ref.method";
		}
	}
}

#endif