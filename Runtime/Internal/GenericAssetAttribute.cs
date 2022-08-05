// smidgens @ github

namespace Smidgenomics.Unity.Variables
{
	using System;
	using UnityEngine;

	[AttributeUsage(AttributeTargets.Field)]
	internal class AssetValueSearchAttribute : PropertyAttribute
	{
		public Type BaseType { get; }

		public AssetValueSearchAttribute(Type t)
		{
			BaseType = t;
		}
	}
}