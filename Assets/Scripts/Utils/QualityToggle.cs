using UnityEngine;
using UnityEngine.UI;

namespace Game.Utils {
	public class QualityToggle: MonoBehaviour {
		[SerializeField] private Toggle _toggle;

		private void OnValueChanged(bool state) {
			var target = state ? 1 : 0;
			QualitySettings.SetQualityLevel(target, true);
		}
		private void OnEnable() {
			_toggle.isOn = QualitySettings.GetQualityLevel() == 1;
			_toggle.onValueChanged.AddListener(OnValueChanged);
		}
		private void OnDisable() {
			_toggle.onValueChanged.RemoveListener(OnValueChanged);
		}
	}
}