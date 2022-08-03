// smidgens @ github

namespace Smidgenomics.Unity.Variables.Editor
{
	using UnityEngine;
	using UnityEditor;
	using UOB = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using System;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(WrappedSetter<>))]
	internal class WrappedSetter_Drawer : PropertyDrawer
	{
		public const string EMPTY_FN_LABEL = "No Function";
		public const float FIXED_BREAKPOINT = 300f;
		public const double PADDING = 2.0;

		// [target][method]
		public static readonly float[]
		SIZES_FLUID = { 0.4f, 0.6f },
		SIZES_FIXED = { 120f, 1f };

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			//using (new EditorGUI.PropertyScope(pos, l, prop))
			//{
				var target = prop.FindPropertyRelative(SPHelper.WrappedMethod.TARGET);
				var type = prop.FindPropertyRelative(SPHelper.WrappedMethod.TYPE);
				var method = prop.FindPropertyRelative(SPHelper.WrappedMethod.METHOD);

				pos.height = EditorGUIUtility.singleLineHeight;

				// label
				if (l != GUIContent.none && !fieldInfo.FieldType.IsArray)
				{
					pos = EditorGUI.PrefixLabel(pos, l);
				}

				var cols = pos.width < FIXED_BREAKPOINT
				? pos.SplitHorizontally(PADDING, SIZES_FLUID)
				: pos.SplitHorizontally(PADDING, SIZES_FIXED);

				TargetField(cols[0], target, type, method);
				MethodField(cols[1], target, type, method);
			//}

		}

		private Type GetGenericValueType()
		{
			var t = fieldInfo.FieldType;
			if (t.IsArray)
			{
				t = t.GetElementType();
			}
			var args = t.GetGenericArguments();
			return args.Length > 0 ? args[0] : null; ;
		}

		private static void TargetField(Rect pos, SP target, SP type, SP method)
		{
			var oref = target.objectReferenceValue;
			EditorGUI.PropertyField(pos, target, GUIContent.none);
			var nref = target.objectReferenceValue;
			if (oref != nref)
			{
				type.stringValue = nref.GetStringifiedType();
				method.stringValue = "";
			}
		}

		private void MethodField(Rect pos, SP target, SP type, SP method)
		{
			var valueType = GetGenericValueType();


			if (valueType == null) { return; }

			var label = GetButtonLabel(
				target.objectReferenceValue,
				method.stringValue,
				valueType
			);

			
			using(new EditorGUI.DisabledScope(!target.objectReferenceValue))
			{
				if (GUI.Button(pos, label, EditorStyles.popup))
				{
					var m = GetMethodOptions(
						target.objectReferenceValue,
						valueType,
						method.stringValue,
						(nt, v) =>
						{
							type.stringValue = nt.GetStringifiedType(); ;
							target.objectReferenceValue = nt;
							method.stringValue = v;
							method.serializedObject.ApplyModifiedProperties();
						}
					);
					m.DropDown(pos);
				}
			}
		}

		private static string GetButtonLabel(UOB t, string method, Type rtype)
		{
			if (!t || string.IsNullOrEmpty(method)) { return EMPTY_FN_LABEL; }
			var mname = ReflectionUtility.FormatMethodName(method, false);
			var label = $"{t.GetType().Name}.{mname}";
			var m = ReflectionUtility.FindMethod(method, t.GetType(), typeof(void), rtype);
			return m != null ? label : $"<Missing {label}>";
		}

		private static GenericMenu GetMethodOptions(UOB t, Type rt, string v, Action<UOB, string> onSelect)
		{
			if (!t) { return new GenericMenu(); }
			var m = new GenericMenu();
			m.AddItem(new GUIContent(EMPTY_FN_LABEL), string.IsNullOrEmpty(v), () => onSelect.Invoke(t, null));
			var options = new Dictionary<string, MethodOption>();
			AddComponentOptions(t, rt, options);
			if (options.Count > 0)
			{
				m.AddSeparator("");
				foreach (var o in options)
				{
					var active = v == o.Value.method && t == o.Value.target;
					m.AddItem(new GUIContent(o.Key), active, () => onSelect.Invoke(o.Value.target, o.Value.method));
				}
			}
			return m;
		}

		private struct MethodOption
		{
			public UOB target;
			public string method;
		}

		private static void AddComponentOptions(UOB t, Type rt, Dictionary<string, MethodOption> options)
		{
			if (!t) { return; }
			var tt = t.GetType();

			GameObject go = null;
			var targets = new Dictionary<string, UOB>();

			if (DerivesFrom(tt, typeof(Component)))
			{
				go = (t as Component).gameObject;
			}
			else if (tt == typeof(GameObject))
			{
				go = t as GameObject;
			}

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

			foreach (var v in targets)
			{
				var target = v.Value;
				AddTypeOptions(v.Key, target, target.GetType(), rt, options);
			}
		}

		private static void AddTypeOptions(string label, UOB target, Type t, Type vt, Dictionary<string, MethodOption> d)
		{
			var methods = ReflectionUtility.FindMethods(t, typeof(void), vt);

			foreach (var m in methods)
			{
				var name = m.GetPrettyName();
				var k = $"{label}/{name}";
				d[k] = new MethodOption
				{
					target = target,
					method = m.Name,
				};
			}
		}

		private static bool DerivesFrom(Type t, Type bt)
		{
			var ot = typeof(object);
			var x = t.BaseType;
			while (x != ot && x != bt) { x = x.BaseType; }
			return x == bt;
		}

	}
}