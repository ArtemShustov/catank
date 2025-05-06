using System.Threading.Tasks;
using Game.Fluids;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.Interactions {
	public class TestInteraction: MonoBehaviour, IInteractable {
		[SerializeField] private bool _canInteract = true;
		[SerializeField] private bool _hide = false;
		[SerializeField] private LocalizedString _text;
		[SerializeField] private Fluid[] _fluids;
		
		public bool CanInteract() {
			return _canInteract;
		}
		public void Interact(out bool hide) {
			Debug.Log("Interacted");
			ShowAsync().Forget();
			hide = _hide;
		}
		public LocalizedString GetText() {
			_text.Set("test", Random.Range(10, 100).ToString());
			return _text;
		}

		private async Task ShowAsync() {
			var player = GameRoot.Instance.Player;
			var effect = GameRoot.Instance.BubbleEffect;
			var fluid = _fluids.Random();
			var duration = 1f;

			effect.SetFluid(fluid);
			effect.Play(transform.position, player.transform.position + Vector3.up);
			player.DisableInput();
			player.FluidContainer.SetFluid(fluid);
			player.FluidContainer.SetStored(0);
			
			var t = 0f;
			while (t < duration) {
				t += Time.deltaTime;
				await Awaitable.NextFrameAsync();
				var c = Mathf.RoundToInt(Mathf.Lerp(0, player.FluidContainer.Capacity, t / duration));
				player.FluidContainer.SetStored(c);
			}
			
			player.FluidContainer.SetStored(player.FluidContainer.Capacity);
			player.EnableInput();
			effect.Stop();
		}
	}
}