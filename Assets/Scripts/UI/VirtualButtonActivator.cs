using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.UI {
	public class VirtualButtonActivator: MonoBehaviour {
		[SerializeField] private Button _button;
		[SerializeField] private InputActionReference _input;

		private void OnEnable() {
			_input.action.Enable();
			_input.action.performed += OnInput;
		}
		private void OnDisable() {
			_input.action.Disable();
			_input.action.performed -= OnInput;
		}
		private void OnInput(InputAction.CallbackContext obj) {
			_button.onClick.Invoke();
		}
	}
}