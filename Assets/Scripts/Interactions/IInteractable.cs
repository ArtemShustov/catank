using UnityEngine.Localization;

namespace Game.Interactions {
	public interface IInteractable {
		bool CanInteract();
		void Interact(out bool hide);
		LocalizedString GetText();
	}
}