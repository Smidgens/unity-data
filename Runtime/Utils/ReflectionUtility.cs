// smidgens @ github

namespace Smidgenomics.Unity.Variables
{
	using System;
	using System.Reflection;
	using System.Linq;
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
		/// Returns a string with type and assembly info for object
		/// </summary>
		public static string GetStringifiedType(object o, string dv = "")
		{
			if (o == null) { return dv; }
			var t = o.GetType();
			var tn = t.FullName;
			var nn = t.Assembly.GetName().Name;
			return $"{tn}, {nn}";
		}

		public static bool IsGetter(MethodInfo m, Type vtype)
		{
			return MatchSignature(m, vtype);
		}

		public static bool IsSetter(MethodInfo m, Type vtype)
		{
			return MatchSignature(m, typeof(void), vtype);
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

		/// <summary>
		/// Prettify method name
		/// </summary>
		public static string FormatMethodName(MethodInfo m, in bool includeParens = true)
		{
			var n = m.Name;
			if (IsPropertyName(m.Name))
			{
				return n.Substring(4);
			}

			return n;


			//var ptypes = m.GetParameters();
			//var n = m.Name;
			//if (IsPropertyName(n))
			//{
			//	if(m.ReturnType == typeof(void))
			//	{
			//		return FormatTypeName(ptypes[0].ParameterType) + " " + n.Substring(4);
			//	}

			//	return FormatTypeName(m.ReturnType) + " "  + n.Substring(4);
			//}

			//if (!includeParens) { return n; }

			//var p = new System.Text.StringBuilder(n);

			//p.Append(' ');
			//p.Append('(');

			
			//int i = 0;
			//foreach (var pi in ptypes)
			//{

			//	p.Append(FormatTypeName(pi.ParameterType));

			//	//if (pi.ParameterType.IsPrimitive)
			//	//{
			//	//	p.Append(pi.ParameterType.Name.ToLower());
			//	//}
			//	//else
			//	//{
			//	//	p.Append(pi.ParameterType.Name);
			//	//}
			//	if(i < ptypes.Length - 1)
			//	{
			//		p.Append(',');
			//	}
			//}
			//p.Append(')');

			//return p.ToString();
		}

		/// <summary>
		/// Prettify method name
		/// </summary>
		public static string FormatMethodName(in string n, in bool includeParens = true)
		{
			if (IsPropertyName(n)) { return n.Substring(4); } // trim property prefix
			return includeParens ? n + " ()" : n;
		}

		public static string GetDisplayName(Type t)
		{
			if(t == typeof(int)) { return "int"; }
			if(t == typeof(string)) { return "string"; }
			if(t == typeof(double)) { return "double"; }
			if(t == typeof(float)) { return "float"; }
			if(t == typeof(bool)) { return "bool"; }
			if (t.IsPrimitive) { return t.Name.ToLower(); }
			return t.Name;
		}

		private static bool IsPropertyName(in string n)
		{
			if(n.Length < 5) { return false; } // get_[x] or set_[x]
			return
			(n[0] == 'g' || n[0] == 's')
			&& n[1] == 'e'
			&& n[2] == 't'
			&& n[3] == '_';
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
