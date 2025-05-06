using System.Linq;
using Game.Fluids;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.Interactions {
	public class FluidContainerInteraction: MonoBehaviour, IInteractable {
		[SerializeField] private bool _clearTanks = true;
		[SerializeField] private bool _allowDrain = true;
		[SerializeField] private bool _allowPut = true;
		[SerializeField] private Fluid[] _putWhitelist;
		[SerializeField] private LocalizedString _drainText;
		[SerializeField] private LocalizedString _putText;
		[SerializeField] private FluidContainer _container;
		
		private const string FLUID_KEY = "fluid";
		
		public bool CanInteract() {
			var player = GameRoot.Instance.Player.FluidContainer;

			return CanPlayerToContainer(player) || CanContainerToPlayer(player);
		}
		public void Interact(out bool hide) {
			hide = true;
			var player = GameRoot.Instance.Player.FluidContainer;
			var effect = GameRoot.Instance.BubbleEffect;
			
			if (CanContainerToPlayer(player)) {
				effect.TransferAsync(_container, player, _clearTanks).Forget();
				return;
			}
			if (CanPlayerToContainer(player)) {
				effect.TransferAsync(player, _container, _clearTanks).Forget();
				return;
			}
		}
		public LocalizedString GetText() {
			var player = GameRoot.Instance.Player.FluidContainer;

			if (CanContainerToPlayer(player)) {
				_drainText.Set(FLUID_KEY, _container.Fluid.Name);
				return _drainText;
			}
			if (CanPlayerToContainer(player)) {
				_putText.Set(FLUID_KEY, player.Fluid.Name);
				return _putText;
			}
			return null;
		}
		
		private bool CanContainerToPlayer(FluidContainer player) {
			return _allowDrain && _container.Fill != 0 
			       && IsTargetEmptyOrSame(_container, player)
			       && GetTransferCount(_container, player) > 0;;
		}
		private bool CanPlayerToContainer(FluidContainer player) {
			return _allowPut && player.Fill != 0 
			       && IsInWhitelist(player.Fluid)
			       && IsTargetEmptyOrSame(player, _container) 
			       && GetTransferCount(player, _container) > 0;
		}
		private bool IsTargetEmptyOrSame(FluidContainer from, FluidContainer to) => to.Fill == 0 || from.Fluid == to.Fluid;
		private int GetTransferCount(FluidContainer from, FluidContainer to) => Mathf.Min(from.Stored, to.Capacity - to.Stored);
		private bool IsInWhitelist(Fluid fluid) => _putWhitelist.Length == 0 || _putWhitelist.Contains(fluid);
	}
}