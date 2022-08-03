// smidgens @ github

namespace Smidgenomics.Unity.Variables
{
	using UnityEngine;
	using UnityObject = UnityEngine.Object;

	[CreateAssetMenu(menuName = Config.CreateAssetMenu.VAR_OBJECT, order = 50)]
	public class SVUnityObject : ScriptableValue<UnityObject> { }
}