// smidgens @ github

namespace Smidgenomics.Unity.ScriptableData
{
	using UnityEngine;

	[CreateAssetMenu(menuName = Config.CreateAssetMenu.VAR_CURVE, order = 26)]
	internal class AssetCurve : ScriptableValue<AnimationCurve> { }
}