using Game.Buildings;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Game.UI {
	public class PumpPopup: MonoBehaviour {
		[SerializeField] private Pump _pump;
		[SerializeField] private LocalizedString _worksText;
		[SerializeField] private LocalizedString _notWorksText;
		[SerializeField] private LocalizeStringEvent _statusLabel;

		private void Refresh() {
			var text = _pump.IsWorking ? _worksText : _notWorksText;
			_statusLabel.StringReference = text;
		}

		private void OnEnable() {
			Refresh();
			_pump.TemperatureChanged += Refresh;
		}
		private void OnDisable() {
			_pump.TemperatureChanged -= Refresh;
		}
	}
}