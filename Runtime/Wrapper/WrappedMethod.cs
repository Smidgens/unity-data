//// smidgens @ github

//namespace Smidgenomics.Unity.Variables
//{
//	using UnityEngine;
//	using System;
//	using System.Reflection;

//	/// <summary>
//	/// Reflection assisted reference to getter
//	/// </summary>
//	[Serializable]
//	public class WrappedMethod<RT, VT>
//	{
//		public VT Value => GetValue();

//		[SerializeField] private MethodRef _ref = default;

//		// method references cached outside editor
//		private Func<VT> _cachedGet = null;
//		private Action<VT> _cachedSet = null;

//		private MethodInfo _cachedInfo = null;


//		private VT GetValue()
//		{
//			if (!Application.isEditor) { return GetValueCached(); }
//			return GetValueUncached();
//		}

//		// loads getter using reflection
//		private MethodInfo LoadMethod()
//		{
//			if (!_ref.target) { return null; }
//			var tt = _ref.target.GetType();
//			return ReflectionUtility.FindMethod(_ref.method, tt, typeof(VT));
//		}

//		// loads method anew before value
//		private VT GetValueUncached()
//		{
//			var m = LoadMethod();
//			return m != null ? (VT)m.Invoke(_ref.target, new object[0]) : default;
//		}

//		// loads and caches method info before value
//		private VT GetValueCached()
//		{
//			if (_cachedGet == null)
//			{
//				_cachedInfo = LoadMethod();
//				if (_cachedInfo != null) { _cachedGet = GetMethodValue; }
//				else { _cachedGet = GetDefaultValue; }
//			}
//			return _cachedGet.Invoke();
//		}

//		private VT GetDefaultValue() => default;

//		private VT GetMethodValue()
//		{
//			return (VT)_cachedInfo.Invoke(_ref.target, new object[0]);
//		}

//		[Serializable]
//		internal struct MethodRef
//		{
//			public UnityEngine.Object target;
//			public string type, method;
//		}
//	}
//}

//#if UNITY_EDITOR

//namespace Smidgenomics.Unity.Variables.Editor
//{
//	internal static partial class SPHelper
//	{
	
//	}
//}

//#endif