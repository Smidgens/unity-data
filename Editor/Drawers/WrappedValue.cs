// smidgens @ github

namespace Smidgenomics.Unity.Variables.Editor
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Custom drawer for value ref
	/// </summary>
	[CustomPropertyDrawer(typeof(WrappedValue<,>), true)]
	internal class WrappedValue_Drawer : PropertyDrawer
	{
		// width of source type dropdown
		public const float TYPE_WIDTH = 60f;
		public const double PADDING = 2.0;

		// [type][value]
		public static readonly float[] SIZES = { 60f, 1f, };

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
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

				var cols = pos.SplitHorizontally(PADDING, SIZES);

				// type field
				EditorGUI.PropertyField(cols[0], type, GUIContent.none);

				// display different prop depending on source type
				var sourcePropName = SPHelper.WrappedValue.GetTypeField(type.enumValueIndex);
				if (sourcePropName != null)
				{
					var valueProp = prop.FindPropertyRelative(sourcePropName);
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