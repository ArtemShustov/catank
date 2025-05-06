using Game.Fluids;
using UnityEngine;

namespace Game {
	public class KeepContainerFilled: MonoBehaviour {
		[SerializeField] private FluidContainer _container;
		private bool _lock;

		private void OnEnable() {
			_container.FillChanged += OnFillChanged;
		}
		private void OnDisable() {
			_container.FillChanged -= OnFillChanged;
		}
		private void OnFillChanged() {
			if (_lock) {
				return;
			}
			_lock = true;
			_container.SetStored(_container.Capacity / 2);
			_lock = false;
		}
	}
}