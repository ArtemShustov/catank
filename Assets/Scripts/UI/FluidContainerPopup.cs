using Game.Fluids;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Game.UI {
	public class FluidContainerPopup: MonoBehaviour {
		[SerializeField] private LocalizedString _emptyFluid;
		[SerializeField] private FluidContainer _container;
		[SerializeField] private LocalizeStringEvent _label;

		private void Refresh() {
			_label.StringReference.Set("fluid", _container?.Fluid?.Name ?? _emptyFluid);
			_label.StringReference.Set("fill", $"{_container?.Fill * 100:00.#}");
		}

		private void OnEnable() {
			Refresh();
			_container.FillChanged += Refresh;
			_container.FluidChanged += Refresh;
		}
		private void OnDisable() {
			_container.FillChanged -= Refresh;
			_container.FluidChanged -= Refresh;
		}
	}
}