using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Game.UI {
	public class InteractionHint: MonoBehaviour {
		[SerializeField] private Mode _keyHintMode = Mode.Both;
		[SerializeField] private InputIcons _icons;
		[SerializeField] private InputActionReference _input;
		[SerializeField] private LocalizeStringEvent _label;
		
		private const string BUTTON_KEY = "key";
		
		public void SetInteractionHint(LocalizedString text) {
			text.Set(BUTTON_KEY, GetInputText());
			_label.StringReference = text;
			_label.RefreshString();
		}

		private string GetInputText() {
			var control = _input.action.controls.FirstOrDefault(IsCurrentDevice);
			if (control == null) {
				return string.Empty;
			}
			
			var str = _keyHintMode switch {
				Mode.None => string.Empty,
				Mode.OnlyText => control?.displayName ?? string.Empty,
				Mode.OnlyIcons => _icons?.Get(control) ?? string.Empty,
				Mode.Both => _icons?.Get(control) ?? control?.displayName ?? string.Empty,
				_ => throw new ArgumentOutOfRangeException()
			};
			return str;

			bool IsCurrentDevice(InputControl ctrl) {
				var current = InputUtils.GetActiveDevice();
				return current is Mouse ? ctrl.device is Keyboard : ctrl.device == current;
			}
		}

		private void OnEnable() {
			InputUtils.OnDeviceChanged += OnDeviceChanged;
			OnDeviceChanged(InputUtils.GetActiveDevice());
		}
		private void OnDisable() {
			InputUtils.OnDeviceChanged -= OnDeviceChanged;
		}
		private void OnDeviceChanged(InputDevice device) {
			if (_label.StringReference == null) {
				return;
			}

			_label.StringReference.Set(BUTTON_KEY, GetInputText());
			_label.RefreshString();
		}

		private enum Mode {
			None, OnlyText, OnlyIcons, Both
		}
	}
}