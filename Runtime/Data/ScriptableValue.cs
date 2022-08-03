// smidgens @ github

// suppress warning about unused _description field
#pragma warning disable 0414

namespace Smidgenomics.Unity.Variables
{
	using UnityEngine;

	/// <summary>
	/// Base variable, used for project search
	/// </summary>
	public abstract class ScriptableValue : ScriptableObject { }

	/// <summary>
	/// Typed variable
	/// </summary>
	public abstract class ScriptableValue<T> : ScriptableValue
	{
		public T Value => _value;
		[SerializeField] private T _value = default;
	}
}
