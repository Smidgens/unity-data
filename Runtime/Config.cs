// smidgens @ github

namespace Smidgenomics.Unity.Data
{
	/// <summary>
	/// magic constants, lt.dan
	/// </summary>
	internal static class Config
	{

		public static class AddComponentMenu
		{
			public const string
			EMIT_VALUES = _PREFIX + "Emit Values";
			private const string _PREFIX = "Smidgenomics/Data/";
		}

		/// <summary>
		/// CreateAssetMenuAttribute, menuName
		/// </summary>
		public static class CreateAssetMenu
		{
			public const string
			VAR_FLOAT = _PREFIX + "float",
			VAR_BOOL = _PREFIX + "bool",
			VAR_INT = _PREFIX + "int",
			VAR_STRING = _PREFIX + "string",
			VAR_CURVE = _PREFIX + "Animation Curve",
			VAR_COLOR = _PREFIX + "Color",
			VAR_VECTOR2 = _PREFIX + "Vector2",
			VAR_VECTOR3 = _PREFIX + "Vector3",
			VAR_OBJECT = _PREFIX + "Object (Unity)";
			private const string _PREFIX = "Variable/";
		}
	}
}