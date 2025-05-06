using UnityEngine;
using UnityEngine.Localization;

namespace Game.Fluids {
	[CreateAssetMenu(menuName = "CMS/Fluid")]
	public class Fluid: ScriptableObject {
		[field: SerializeField] public string Id { get; private set; }
		[Header("Settings")]
		[field: SerializeField] public LocalizedString Name { get; private set; }
		[field: SerializeField] public Material Material { get; private set; }
	}
}