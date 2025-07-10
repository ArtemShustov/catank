using System;
using Game.Fluids;
using UnityEngine;

namespace Game.Buildings {
	public class Refinery: MonoBehaviour {
		[SerializeField] private int _production = 100;
		[SerializeField] private FluidContainer _input;
		[SerializeField] private FluidContainer _output;
		[SerializeField] private Fluid _fuel;

		private bool _used;
		private float _timer;

		public event Action UsedFirstTime;

		private void Awake() {
			_output.SetFluid(_fuel);
		}
		private void Update() {
			_timer += Time.deltaTime;
			if (_timer >= 1) {
				_timer = 0;
				OnTick();
			}
		}

		private void OnTick() {
			var toRefine = Mathf.Min(Mathf.Min(_production, _input.Stored), _output.Capacity - _output.Stored);
			if (toRefine > 0) {
				_input.SetStored(_input.Stored - toRefine);
				_output.SetStored(_output.Stored + toRefine);
				if (!_used) {
					UsedFirstTime?.Invoke();
					_used = true;
				}
			}
		}
	}
}