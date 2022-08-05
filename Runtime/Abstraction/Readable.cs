// smidgens @ github

namespace Smidgenomics.Unity.Data
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
	public class Readable<T>
	{
		/// <summary>
		/// Reads value
		/// </summary>
		public T Value => GetValue();

		/// <summary>
		/// Stringifies value
		/// </summary>
		public override string ToString() => GetValue().ToString();

		/// <summary>
		/// Implicit conversion to value
		/// </summary>
		public static implicit operator T(Readable<T> valueSource)
		{
			return valueSource.Value;
		}

		[GenericAsset(typeof(ScriptableValue))]
		[SerializeField] internal ScriptableValue<T> _asset = default;
		[SerializeField] private Getter<T> _method = default;
		[SerializeField] private T _value = default;
		[SerializeField] private ValueSource _type = ValueSource.Static;

		// select
		private T GetValue()
		{
			switch(_type)
			{
				case ValueSource.Static: return ReadStatic();
				case ValueSource.Asset: return ReadAsset();
				case ValueSource.Getter: return ReadFunction();
			}
			return default;
		}

		private T ReadStatic() => _value;

		private T ReadAsset()
		{
			if (!_asset) { return default; }
			return _asset.Value;
		}

		private T ReadFunction() => _method.Invoke();
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Data.Editor
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