// smidgens @ github

namespace Smidgenomics.Unity.Data.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;
	using UnityObject = UnityEngine.Object;

	/// <summary>
	/// System.Reflection helpers
	/// </summary>
	internal static class EditorReflection
	{
		// find getter methods in type
		public static MethodInfo[] FindMethods(Type t, Type rt, params Type[] ptypes)
		{
			return ReflectionUtility.FindMethods(t, rt, ptypes);
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
		public static string FormatMethodName(MethodInfo m)
		{
			var n = m.Name;
			if (IsPropertyName(m.Name))
			{
				return n.Substring(4);
			}
			return n;
		}

		public static bool DerivesFrom(Type t, Type bt)
		{
			var ot = typeof(object);
			var x = t.BaseType;
			while (x != ot && x != bt) { x = x.BaseType; }
			return x == bt;
		}

		/// <summary>
		/// Prettify method name
		/// </summary>
		public static string FormatMethodName(in string n)
		{
			return IsPropertyName(n) ? n.Substring(4) : n;
		}

		public static string GetDisplayName(Type t)
		{
			if (t == typeof(int)) { return "int"; }
			if (t == typeof(string)) { return "string"; }
			if (t == typeof(double)) { return "double"; }
			if (t == typeof(float)) { return "float"; }
			if (t == typeof(bool)) { return "bool"; }
			if (t.IsPrimitive) { return t.Name.ToLower(); }
			return t.Name;
		}

		public struct CallableMethodInfo
		{
			public string group;
			public UnityObject target;
			public string method;
			public string displayName;
			public bool isProperty;
		}

		public static CallableMethodInfo[] FindCallableMethods(UnityObject t, Type rt, params Type[] ptypes)
		{
			if (!t) { return new CallableMethodInfo[0]; }
			var tt = t.GetType();

			GameObject go = null;

			var targets = new Dictionary<string, UnityObject>();

			if (DerivesFrom(tt, typeof(Component))) { go = (t as Component).gameObject; }
			else if (tt == typeof(GameObject)) { go = t as GameObject; }

			if (go)
			{
				targets[typeof(GameObject).Name] = go;
				foreach (var c in go.GetComponents<Component>())
				{
					var tn = c.GetType().Name;
					var vk = tn;
					var i = 1;
					while (targets.ContainsKey(vk))
					{
						vk = $"{tn} ({i})";
						i++;
					}
					targets[vk] = c;
				}
			}
			else { targets[tt.Name] = t; }

			var options = new List<CallableMethodInfo>();

			foreach (var v in targets)
			{
				var target = v.Value;
				FindCallables(v.Key, target, target.GetType(), rt, options, ptypes);
			}

			return options.ToArray();
		}

		private static void FindCallables
		(
			string group,
			UnityObject target,
			Type t,
			Type rt,
			List<CallableMethodInfo> opts,
			params Type[] ptypes
		)
		{
			var methods = FindMethods(t, rt, ptypes);
			foreach (var m in methods)
			{
				var displayName = FormatMethodName(m);
				opts.Add(new CallableMethodInfo
				{
					group = group,
					target = target,
					method = m.Name,
					displayName = displayName,
					isProperty = m.IsSpecialName
				});
			}
		}

		private static bool IsPropertyName(in string n)
		{
			if (n.Length < 5) { return false; } // get_[x] or set_[x]
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
