// smidgens @ github

namespace Smidgenomics.Unity.Variables
{
	using UnityEngine;
	using System;

	/// <summary>
	/// Determines where value should be pulled from
	/// </summary>
	internal enum ValueSource
	{
		Static, // fixed
		Asset, // scriptable object
		[InspectorName("Function")]
		Getter, // reflection
	}

	/// <summary>
	/// Wrapper around value, hides source
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="AT">Asset type supported</typeparam>
	[Serializable]
	public class WrappedValue<T>
	//public abstract class WrappedValue<T, AT> where AT : ScriptableValue
	{
		/// <summary>
		/// Reads value
		/// </summary>
		public T Value => GetValue();

		/// <summary>
		/// Reads and stringifies current value
		/// </summary>
		/// <returns>Stringified value</returns>
		public override string ToString() => GetValue().ToString();

		/// <summary>
		/// Implicit conversion to return type
		/// </summary>
		/// <param name="valueSource">Instance</param>
		public static implicit operator T(WrappedValue<T> valueSource)
		{
			return valueSource.Value;
		}

		[AssetValueSearch(typeof(ScriptableValue))]
		[SerializeField] internal ScriptableValue<T> _asset = default;
		[SerializeField] private WrappedGetter<T> _method = default;
		[SerializeField] private T _value = default;
		[SerializeField] private ValueSource _type = ValueSource.Static;

		// select
		private T GetValue()
		{
			switch(_type)
			{
				case ValueSource.Static: return GetStaticValue();
				case ValueSource.Asset: return GetAssetValue();
				case ValueSource.Getter: return GetGetterValue();
			}
			return default;
		}

		private T GetAssetValue()
		{
			if (!_asset) { return default; }
			return _asset.Value;
		}

		private T GetStaticValue() => _value;
		private T GetGetterValue() => _method.Invoke();
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Variables.Editor
{
	internal static partial class SPHelper
	{
		/// <summary>
		/// Editor helper for WrappedValue serialization
		/// </summary>
		public static class WrappedValue
		{
			public const string
			VALUE_TYPE = "_type";
			public static string GetTypeField(int type)
			{
				if(type < 0 || type >= _VALUE_FIELDS.Length) { return null; }
				return _VALUE_FIELDS[type];
			}

			private static readonly string[] _VALUE_FIELDS =
			{
				"_value",
				"_asset",
				"_method",
			};
		}
	}
}

#endif