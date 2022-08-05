// smidgens @ github

namespace Smidgenomics.Unity.Data
{
	using UnityEngine;

	public interface IReadableValue<T>
	{
		public T Value { get; }
	}

	/// <summary>
	/// Base variable, used for project search
	/// </summary>
	public abstract class ScriptableValue : ScriptableObject { }

	/// <summary>
	/// Typed variable
	/// </summary>
	public abstract class ScriptableValue<T> : ScriptableValue, IReadableValue<T>
	{
		public T Value => _value;
		[SerializeField] private T _value = default;
	}
}
