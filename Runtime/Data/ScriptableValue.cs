// smidgens @ github

// suppress warning about unused _description field
#pragma warning disable 0414

namespace Smidgenomics.Unity.Variables
{
	using UnityEngine;
	using System;

	/// <summary>
	/// Root variable
	/// </summary>
	public abstract class ScriptableValue : ScriptableObject
	{
		/// <summary>
		/// Type of variable value
		/// </summary>
		public abstract Type ValueType { get; }
	}

	/// <summary>
	/// Base typed variable
	/// </summary>
	/// <typeparam name="T">Variable type</typeparam>
	public abstract class ScriptableValue<T> : ScriptableValue
	{
		public override Type ValueType => typeof(T);

		/// <summary>
		/// Value getter
		/// </summary>
		public T Value => _value;

		// serialized value
		[SerializeField] private T _value = default;

		// editor only description (documentation)
		#if UNITY_EDITOR
		[TextArea(2, 10)]
		[SerializeField] private string _notes = default;
		#endif

	}
}
