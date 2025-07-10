using System;
using UnityEngine;

namespace Game.Fluids {
	public class FluidContainer: MonoBehaviour {
		[Header("Components")]
		[SerializeField] private Liquid _view;
		[Header("Settings")]
		[SerializeField] private float _minFill = -1;
		[SerializeField] private float _maxFill = 1;
		[Header("Data")]
		[SerializeField, Range(0, 1)] private float _fill = 0.5f;
		[SerializeField] private Fluid _fluid;
		[SerializeField] private int _capacity = 16 * 1000;
		[SerializeField] private int _stored = 0;
		[SerializeField] private bool _void = false;

		public event Action FluidChanged;
		public event Action FillChanged;
		public event Action<int, int> StoredChanged;
		
		public float Fill => _void ? 0 : _fill;
		public int Capacity => _void ? int.MaxValue : _capacity;
		public int Stored => _void ? _capacity / 2 : _stored;
		public Fluid Fluid => _fluid;

		public void SetFill(float fill) {
			_fill = Mathf.Clamp01(fill);
			_view?.SetFillAmount(Mathf.Lerp(_minFill, _maxFill, fill));

			_view?.gameObject.SetActive(!Mathf.Approximately(fill, 0));
			FillChanged?.Invoke();
		}
		public void SetFluid(Fluid fluid) {
			_fluid = fluid;
			if (fluid) {
				_view?.SetMaterial(fluid.Material);
			}
			FluidChanged?.Invoke();
		}
		public void SetStored(int stored) {
			if (_void) {
				return;
			}
			var before = _stored;
			_stored = Mathf.Clamp(stored, 0, _capacity);
			SetFill(_stored / (float)_capacity);
			StoredChanged?.Invoke(before, _stored);
		}

		private void OnValidate() {
			SetFluid(_fluid);
			SetStored(Mathf.RoundToInt(_fill * _capacity));
		}
	}
}