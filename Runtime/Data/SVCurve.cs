// smidgens @ github

namespace Smidgenomics.Unity.Variables
{
	using UnityEngine;

	[CreateAssetMenu(menuName = Config.CreateAssetMenu.VAR_CURVE, order = 26)]
	public class SVCurve : ScriptableValue<AnimationCurve> { }
}