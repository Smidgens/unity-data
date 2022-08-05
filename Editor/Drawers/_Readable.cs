// smidgens @ github

namespace Smidgenomics.Unity.Data.Editor
{
	using UnityEditor;
	using UnityEngine;
	using System;
	using SP = UnityEditor.SerializedProperty;

	/// <summary>
	/// Custom drawer for value ref
	/// </summary>
	[CustomPropertyDrawer(typeof(Readable<>), true)]
	internal class Readable_Drawer : PropertyDrawer
	{

		// [type][value]
		public static readonly float[] SIZES =
		{
			EditorGUIUtility.singleLineHeight,
			70f, 1f,
		};

		public override float GetPropertyHeight(SP prop, GUIContent label)
		{
			if (fieldInfo.FieldType.IsArray)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			var type = prop.FindPropertyRelative(SPHelper.WrappedValue.VALUE_TYPE);
			if (type.enumValueIndex != 2)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			return (EditorGUIUtility.singleLineHeight * 2f) + 2f;
		}

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{

			var isLarge = !fieldInfo.FieldType.IsArray;

			using (new EditorGUI.PropertyScope(pos, l, prop))
			{
				// source type
				var type = prop.FindPropertyRelative(SPHelper.WrappedValue.VALUE_TYPE);

				// label
				if (l != GUIContent.none && !fieldInfo.FieldType.IsArray)
				{
					pos = EditorGUI.PrefixLabel(pos, l);
				}

				if (fieldInfo.FieldType.IsArray)
				{
					pos.height = EditorGUIUtility.singleLineHeight;
				}

				var cols = pos.SplitHorizontally(2.0, SIZES);

				DrawPreview(cols[0]);

				// type field
				var tbtnRect = cols[1];
				tbtnRect.height = EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(tbtnRect, type, GUIContent.none);

				if(isLarge && type.enumValueIndex == 2)
				{
					var irect = cols[1];
					irect.height = EditorGUIUtility.singleLineHeight;
					irect.width = irect.height;
					irect.position += new Vector2(cols[1].width - irect.width - 5f, irect.height + 2f);
					PluginIcons.Draw(irect, PIcon.Arrow);
				}

				// display different prop depending on source type
				var sourcePropName = SPHelper.WrappedValue.GetTypeField(type.enumValueIndex);

				var valueProp = sourcePropName != null
				? prop.FindPropertyRelative(sourcePropName)
				: null;

				if (valueProp != null)
				{
					EditorGUI.PropertyField(cols[2], valueProp, GUIContent.none); // draw value field
				}
				else
				{
					GUI.Box(cols[2], "");
					EditorGUI.LabelField(cols[2], new GUIContent("unsupported"), EditorStyles.centeredGreyMiniLabel);
				}
			}
		}

		private void DrawPreview(in Rect pos, Type type)
		{
			var ipos = pos;
			ipos.size -= Vector2.one * 1f;
			ipos.center = pos.center;

			var isUnityType = typeof(UnityEngine.Object).IsAssignableFrom(type);

			if (!isUnityType)
			{
				var ic = PluginIcons.Find(type);
				PluginIcons.Draw(ipos, ic);
			}

			if(isUnityType)
			{
				var tex = AssetPreview.GetMiniTypeThumbnail(type);
				GUI.DrawTexture(ipos, tex);
			}
		}

		private void DrawPreview(Rect pos)
		{
			pos.height = pos.width;
			DrawPreview(pos, GetGenericType());
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
	}

}