// smidgens @ github

namespace Smidgenomics.Unity.ScriptableData
{
	using System;
	using UnityEngine;

	[AttributeUsage(AttributeTargets.Field)]
	internal class GenericAssetAttribute : PropertyAttribute
	{
		public Type BaseType { get; }

		public GenericAssetAttribute(Type t)
		{
			BaseType = t;
		}
	}
}