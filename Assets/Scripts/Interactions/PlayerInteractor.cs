using Game.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;

namespace Game.Interactions {
	public class PlayerInteractor: MonoBehaviour {
		private DefaultInput _input;
		private IInteractable _current;

		public void SetInput(DefaultInput input) {
			if (_input != null) {
				_input.Player.Interact.performed -= OnInput;
			}
			_input = input;
			if (_input != null) {
				_input.Player.Interact.performed += OnInput;
			}
		}

		private void OnInput(InputAction.CallbackContext obj) {
			if (_current == null) {
				return;
			}
			
			_current.Interact(out var hide);
			if (hide) {
				_current = null;
				GameUI.Instance.SetInteractionHint(null);
			}
		}
		private void OnEnable() {
			if (_input != null) {
				_input.Player.Interact.performed += OnInput;
			}
		}
		private void OnDisable() {
			if (_input != null) {
				_input.Player.Interact.performed -= OnInput;
			}
		}
		private void OnTriggerEnter(Collider other) {
			if (other.TryGetComponent<IInteractable>(out var interactable) && interactable.CanInteract()) {
				_current = interactable;
				GameUI.Instance.SetInteractionHint(interactable.GetText());
			}
		}
		private void OnTriggerExit(Collider other) {
			if (_current != null && other.TryGetComponent<IInteractable>(out var interactable) && interactable == _current) {
				_current = null;
				GameUI.Instance.SetInteractionHint(null);
			}
		}
	}
}