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
		/// <summary>
		/// Static/default value
		/// </summary>
		Static,
		/// <summary>
		/// Use asset
		/// </summary>
		Asset,
		/// <summary>
		/// Generic getter
		/// </summary>
		[InspectorName("Function")]
		Getter,
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

		// convenience setter for simple values
		protected void SetStatic(T value)
		{
			_vStatic = value;
			_type = ValueSource.Static;
		}

		[GenericAsset(typeof(ScriptableValue))]
		[SerializeField] internal ScriptableValue<T> _vAsset = default;
		[SerializeField] private T _vStatic = default;
		[SerializeField] private WrappedGetter<T> _vGetter = default;
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
			if (!_vAsset) { return default; }
			return _vAsset.Value;
		}

		private T GetStaticValue() => _vStatic;
		private T GetGetterValue() => _vGetter.Value;
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
				"_v" + nameof(ValueSource.Static),
				"_v" + nameof(ValueSource.Asset),
				"_v" + nameof(ValueSource.Getter),
			};
		}
	}
}

#endif