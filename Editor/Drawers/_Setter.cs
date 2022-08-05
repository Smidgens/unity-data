// smidgens @ github

namespace Smidgenomics.Unity.Data.Editor
{
	using UnityEngine;
	using UnityEditor;
	using UOB = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using System;

	[CustomPropertyDrawer(typeof(Setter<>))]
	internal class Setter_Drawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SP prop, GUIContent label)
		{
			if (fieldInfo.FieldType.IsArray)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			return (EditorGUIUtility.singleLineHeight * 2f) + 2f;
		}

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			using (new EditorGUI.PropertyScope(pos, l, prop))
			{
				var target = prop.FindPropertyRelative(SPHelper.WrappedMethod.TARGET);
				var type = prop.FindPropertyRelative(SPHelper.WrappedMethod.TYPE);
				var method = prop.FindPropertyRelative(SPHelper.WrappedMethod.METHOD);

				// label
				if (l != GUIContent.none && !fieldInfo.FieldType.IsArray)
				{
					pos = EditorGUI.PrefixLabel(pos, l);
				}

				//pos.height = EditorGUIUtility.singleLineHeight;


				var rects = GetFieldRects(pos);

				TargetField(rects[0], target, type, method);
				MethodField(rects[1], target, type, method);
			}

		}

		private Rect[] GetFieldRects(Rect pos)
		{
			if (!fieldInfo.FieldType.IsArray)
			{
				var t = pos;
				t.height = EditorGUIUtility.singleLineHeight;
				var b = t;
				b.position += new Vector2(0f, t.height + 2f);
				return new Rect[] { t, b };
			}
			var cols = pos.width < Config.WrappedMethod.FIXED_BREAKPOINT
			? pos.SplitHorizontally(2.0, Config.WrappedMethod.SIZES_FLUID)
			: pos.SplitHorizontally(2.0, Config.WrappedMethod.SIZES_FIXED);
			return cols;
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
			var valueType = fieldInfo.GetFirstGenericType();
			if (valueType == null) { return; }

			using(new EditorGUI.DisabledScope(!target.objectReferenceValue))
			{
				var label = GetButtonLabel
				(
					target.objectReferenceValue,
					method.stringValue,
					valueType
				);

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
			if (!t || string.IsNullOrEmpty(method)) { return Config.Label.NO_FUNCTION_SET; }
			var mname = EditorReflection.FormatMethodName(method);
			var label = $"{t.GetType().Name}.{mname}";
			var m = ReflectionUtility.FindMethod(method, t.GetType(), typeof(void), rtype);
			return m != null ? label : $"<Missing {label}>";
		}

		private static GenericMenu GetMethodOptions(UOB t, Type rt, string v, Action<UOB, string> onSelect)
		{
			var m = new GenericMenu();

			m.AddItem(new GUIContent(Config.Label.NO_FUNCTION_SET), string.IsNullOrEmpty(v), () => onSelect.Invoke(t, null));

			if (!t) { return m; }

			m.AddSeparator("");
			m.AddDisabledItem(new GUIContent("Dynamic " + EditorReflection.GetDisplayName(rt)));

			var ol = EditorReflection.FindCallableMethods(t, typeof(void), rt);

			if (ol.Length == 0)
			{
				m.AddDisabledItem(new GUIContent("No Options"));
			}

			foreach (var o in ol)
			{
				var ov = o;
				var active = v == o.method && t == o.target;
				var l = new GUIContent($"{o.group}/{o.displayName}");
				m.AddItem(l, active, () => onSelect.Invoke(ov.target, ov.method));
			}
			return m;
		}

	}
}