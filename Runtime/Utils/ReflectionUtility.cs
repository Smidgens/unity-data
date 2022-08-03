// smidgens @ github

namespace Smidgenomics.Unity.Variables
{
	using System;
	using System.Linq;
	using System.Reflection;
	using BFlags = System.Reflection.BindingFlags;

	/// <summary>
	/// System.Reflection helpers
	/// </summary>
	internal static class ReflectionUtility
	{
		public const BFlags
		INSTANCE_METHOD = BFlags.Public | BFlags.Instance;

		// find getter methods in type
		public static MethodInfo[] FindMethods(Type t, Type rt, params Type[] ptypes)
		{
			return t
			.GetMethods(INSTANCE_METHOD)
			.Where(x => MatchSignature(x, rt, ptypes))
			.OrderByDescending(x => x.IsSpecialName)
			.ToArray();
		}

		/// <summary>
		/// Finds getter method by name and call signature in type
		/// </summary>
		public static MethodInfo FindMethod(string name, Type t, Type rt, params Type[] ptypes)
		{
			return
			FindMethods(t, rt, ptypes)
			.FirstOrDefault(x => x.Name == name);
		}

		/// <summary>
		/// Checks if method is getter with call signature
		/// </summary>
		public static bool MatchSignature(MethodInfo m, Type rt, params Type[] ptypes)
		{
			if (rt != m.ReturnType) { return false; }
			if (!MatchArgTypes(m, ptypes)) { return false; }
			return true;
		}

		private static bool MatchArgTypes(MethodInfo m, Type[] types)
		{
			var parr = m.GetParameters();
			if (parr.Length != types.Length) { return false; }
			for (var i = 0; i < parr.Length; i++)
			{
				if (parr[i].ParameterType != types[i]) { return false; }
			}
			return true;
		}
	}
}
