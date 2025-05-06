using Game.Fluids;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.Interactions {
	public class FluidSourceInteraction: MonoBehaviour, IInteractable {
		[SerializeField] private LocalizedString _text;
		[SerializeField] private FluidContainer _container;
		
		public bool CanInteract() {
			var player = GameRoot.Instance.Player.FluidContainer;
			return player.Fill == 0;
		}
		public void Interact(out bool hide) {
			hide = true;
			var player = GameRoot.Instance.Player.FluidContainer;
			var effect = GameRoot.Instance.BubbleEffect;

			_container.SetStored(player.Capacity);
			effect.TransferAsync(_container, player, false).Forget();
		}
		public LocalizedString GetText() {
			_text.Set("fluid", _container.Fluid.Name);
			return _text;
		}
	}
}