﻿// smidgens @ github

namespace Smidgenomics.Unity.Variables.Editor
{
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	[CustomPropertyDrawer(typeof(GenericAssetAttribute))]
	internal class GenericAssetAttribute_Drawer : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			var a = attribute as GenericAssetAttribute;

			if (l != GUIContent.none && !fieldInfo.FieldType.IsArray)
			{
				pos = EditorGUI.PrefixLabel(pos, l);
			}

			var blabel = "<none>";

			if(prop.objectReferenceValue)
			{
				blabel = prop.objectReferenceValue.name;
			}

			var rects = pos.SplitHorizontally(2.0, pos.height, 1f);

			DrawPreview(rects[0], prop.objectReferenceValue);

			if (GUI.Button(rects[1], blabel, EditorStyles.popup))
			{
				var type = fieldInfo.FieldType;

				var currentPath = prop.objectReferenceValue
				? AssetDatabase.GetAssetPath(prop.objectReferenceValue)
				: null;

				if (fieldInfo.FieldType.IsArray)
				{
					type = fieldInfo.FieldType.GetElementType();
				}

				var m = new GenericMenu();

				m.AddItem(new GUIContent("<none>"), currentPath == null, () =>
				{
					prop.objectReferenceValue = null;
					prop.serializedObject.ApplyModifiedProperties();
				});

				m.AddSeparator("");

				var gtype = GetGenericType();

				var title = $"{EditorReflection.GetDisplayName(gtype)}";

				m.AddDisabledItem(new GUIContent(title));

				var paths = FindVariableAssets(gtype);

				foreach(var path in paths)
				{
					var val = path;
					var name = path.Split('/').LastOrDefault().Split('.').FirstOrDefault();
					var label = new GUIContent(name);
					var active = path == currentPath;
					m.AddItem(label, active, () =>
					{
						var a = AssetDatabase.LoadAssetAtPath<ScriptableValue>(val);
						prop.objectReferenceValue = a;
						prop.serializedObject.ApplyModifiedProperties();

					});
				}
				m.DropDown(pos);
			}
		}

		private void DrawPreview(in Rect pos, UnityEngine.Object ob)
		{
			var tex = ob
			? AssetPreview.GetMiniThumbnail(ob)
			: AssetPreview.GetMiniTypeThumbnail(typeof(ScriptableValue));
			GUI.Box(pos, "");

			if (tex) { GUI.DrawTexture(pos, tex); }

			if(ob)
			{
				EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);

				if(GUI.Button(pos, GUIContent.none, GUIStyle.none))
				{
					EditorGUIUtility.PingObject(ob);
				}


			}
		}

		private Type GetGenericType()
		{
			var ftype = fieldInfo.FieldType;
			if (fieldInfo.FieldType.IsArray)
			{
				ftype = fieldInfo.FieldType.GetElementType();
			}
			var gargs = ftype.GenericTypeArguments;
			if (gargs.Length == 0) { return null; }
			return gargs[0];
		}

		private static string[] FindVariableAssets(Type t)
		{
			var guids = AssetDatabase.FindAssets("t:ScriptableValue");
			var r = new List<string>();
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var ta = AssetDatabase.GetMainAssetTypeAtPath(path);
				while (ta.BaseType != typeof(ScriptableValue))
				{
					ta = ta.BaseType;
				}

				var vtype = ta.GenericTypeArguments[0];

				if (t.IsPrimitive)
				{
					if(t != vtype) { continue; }
				}


				if (vtype == t || t.IsAssignableFrom(vtype))
				{
					r.Add(path);
				}

				//if (vtype != t) { continue; }
				//r.Add(path);
			}

			return r.ToArray();
		}
	}
}