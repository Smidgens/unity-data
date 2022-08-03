// smidgens @ github

namespace Smidgenomics.Unity.Variables.Editor
{
	using UnityEditor;
	using UnityEngine;
	using System;
	using SP = UnityEditor.SerializedProperty;
	
	/// <summary>
	/// Custom drawer for value ref
	/// </summary>
	[CustomPropertyDrawer(typeof(WrappedValue<>), true)]
	internal class WrappedValue_Drawer : PropertyDrawer
	{
		// width of source type dropdown
		public const float TYPE_WIDTH = 60f;
		public const double PADDING = 2.0;

		// [type][value]
		public static readonly float[] SIZES = { 70f, 1f, };

		public readonly Lazy<Texture> FN_ICON = new Lazy<Texture>(() =>
		{
			return Resources.Load<Texture>("smidgenomics.unity.scriptable-variables/arrow");
		});

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


				var cols = pos.SplitHorizontally(PADDING, SIZES);

				// type field
				EditorGUI.PropertyField(cols[0], type, GUIContent.none);


				if(type.enumValueIndex == 2)
				{
					var irect = cols[0];
					irect.height = EditorGUIUtility.singleLineHeight;
					irect.width = irect.height;

					irect.position += new Vector2(cols[0].width - irect.width, irect.height + 2f);

					if (FN_ICON.Value)
					{
						GUI.DrawTexture(irect, FN_ICON.Value);
					}


					//EditorGUI.DrawRect(irect, Color.red);
				}

				// display different prop depending on source type
				var sourcePropName = SPHelper.WrappedValue.GetTypeField(type.enumValueIndex);

				var valueProp = sourcePropName != null
				? prop.FindPropertyRelative(sourcePropName)
				: null;

				if (valueProp != null)
				{
					EditorGUI.PropertyField(cols[1], valueProp, GUIContent.none); // draw value field
				}
				else
				{
					EditorGUI.DrawRect(cols[1], Color.yellow * 0.2f);
					EditorGUI.LabelField(cols[1], new GUIContent("<not implemented>"), EditorStyles.centeredGreyMiniLabel);
				}
			}
		}
	}

}