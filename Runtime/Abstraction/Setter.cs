// smidgens @ github

namespace Smidgenomics.Unity.ScriptableData
{
	using UnityEngine;
	using System;
	using System.Reflection;

	/// <summary>
	/// Reflection assisted reference to setter function
	/// </summary>
	[Serializable]
	internal class Setter<VT>
	{
		public void Invoke(VT v)
		{
			if (!Application.isEditor) { SetValueCached(v); }
			else { SetValueUncached(v); }
		}

		[SerializeField] private MethodRef _ref = default;

		// method references cached outside editor
		private Action<VT> _cachedFn = null;
		private MethodInfo _cachedInfo = null;

		// loads getter using reflection
		private MethodInfo LoadMethod()
		{
			if (!_ref.target) { return null; }
			var tt = _ref.target.GetType();
			return ReflectionUtility.FindMethod(_ref.method, tt, typeof(void), typeof(VT));
		}

		// loads method anew before value
		private void SetValueUncached(VT v)
		{
			var m = LoadMethod();
			if(m != null)
			{
				m.Invoke(_ref.target, new object[] { v });
			}
		}

		// loads and caches method info before value
		private void SetValueCached(VT v)
		{
			if (_cachedFn == null)
			{
				_cachedInfo = LoadMethod();
				if (_cachedInfo != null) { _cachedFn = InvokeSet; }
				else { _cachedFn = NoOp; }
			}
			_cachedFn.Invoke(v);
		}

		private void NoOp(VT v) { }

		private void InvokeSet(VT v)
		{
			_cachedInfo.Invoke(_ref.target, new object[] { v });
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

namespace Smidgenomics.Unity.ScriptableData.Editor
{
	internal static partial class SPHelper
	{
		public static class WrappedMethod
		{
			public const string
			TARGET = "_ref.target",
			TYPE = "_ref.type",
			METHOD = "_ref.method";
		}
	}
}

#endif