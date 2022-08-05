// smidgens @ github

namespace Smidgenomics.Unity.Data
{
	using UnityEngine;
	using UnityObject = UnityEngine.Object;

	[CreateAssetMenu(menuName = Config.CreateAssetMenu.VAR_OBJECT, order = 50)]
	internal class AssetObject : ScriptableValue<UnityObject> { }
}