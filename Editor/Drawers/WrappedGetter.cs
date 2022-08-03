// smidgens @ github

namespace Smidgenomics.Unity.Variables.Editor
{
	using UnityEngine;
	using UnityEditor;
	using UOB = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using System;

	[CustomPropertyDrawer(typeof(WrappedGetter<>))]
	internal class WrappedGetter_Drawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SP prop, GUIContent label)
		{
			if (label == GUIContent.none)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			return (EditorGUIUtility.singleLineHeight * 2f) + 2f;
		}

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			using (new EditorGUI.PropertyScope(pos, l, prop))
			{
				var target = prop.FindPropertyRelative(SPHelper.WrappedGetter.TARGET);
				var type = prop.FindPropertyRelative(SPHelper.WrappedGetter.TYPE);
				var method = prop.FindPropertyRelative(SPHelper.WrappedGetter.METHOD);

				var rows =
				!fieldInfo.FieldType.IsArray;

				// temporary hack
				if(pos.height < EditorGUIUtility.singleLineHeight + 5f)
				{
					rows = false;
				}

				// label
				if (l != GUIContent.none && !fieldInfo.FieldType.IsArray)
				{
					pos = EditorGUI.PrefixLabel(pos, l);
				}

				var rects = GetFieldRects(pos, rows);

				TargetField(rects[0], target, type, method);
				MethodField(rects[1], target, type, method);
			}
		}

		private Rect[] GetFieldRects(Rect pos, bool rows)
		{
			if (rows)
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

			var label = GetButtonLabel
			(
				target.objectReferenceValue,
				method.stringValue,
				valueType
			);

			using(new EditorGUI.DisabledScope(!target.objectReferenceValue))
			{
				if (GUI.Button(pos, label, EditorStyles.popup))
				{
					var m = GetOptions(
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
			var m = ReflectionUtility.FindMethod(method, t.GetType(), rtype);
			return m != null ? label : $"<Missing {label}>";
		}

		private static GenericMenu GetOptions(UOB t, Type rt, string v, Action<UOB, string> onSelect)
		{
			var m = new GenericMenu();

			m.AddItem(new GUIContent(Config.Label.NO_FUNCTION_SET), string.IsNullOrEmpty(v), () => onSelect.Invoke(t, null));

			if (!t) { return m; }

			m.AddSeparator("");
			m.AddDisabledItem(new GUIContent("Dynamic " + EditorReflection.GetDisplayName(rt)));

			var ol = EditorReflection.FindCallableMethods(t, rt);

			if(ol.Length == 0)
			{
				m.AddDisabledItem(new GUIContent("No Options"));
			}

			foreach(var o in ol)
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