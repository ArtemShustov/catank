using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game {
	public class InputUtils: MonoBehaviour {
		public static event Action<InputDevice> OnDeviceChanged;

		private static InputDevice _lastDevice;
		
		private static void OnActionChane(object arg1, InputActionChange change) {
			if (change == InputActionChange.ActionStarted) {
				var lastDevice = ((InputAction)arg1).activeControl.device;
				if (_lastDevice != lastDevice) {
					_lastDevice = lastDevice;
					OnDeviceChanged?.Invoke(_lastDevice);
					// Debug.Log($"Device changed to: {_lastDevice.displayName}");
				}
			}
		}
		public static InputDevice GetActiveDevice() => _lastDevice;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Init() {
			_lastDevice = null;
			InputSystem.onActionChange += OnActionChane;
		}
	}
}