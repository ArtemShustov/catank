using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

		public static bool IsOverUI(PointerEventData eventData) {
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventData, results);

			return results.Count > 0;
		}
		public static bool IsOverUI(Vector2 screenPoint) {
			var eventData = new PointerEventData(EventSystem.current) {
				position = screenPoint
			};

			return IsOverUI(eventData);
		}
	}
}