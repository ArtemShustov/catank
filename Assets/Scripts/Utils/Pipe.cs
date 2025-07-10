using Game.Fluids;
using UnityEngine;

namespace Game.Utils {
	public class Pipe: MonoBehaviour {
		[SerializeField] private FluidContainer _input;
		[SerializeField] private FluidContainer _output;
		[SerializeField] private int _rate = 1000;

		private float _timer;

		private void Update() {
			_timer += Time.deltaTime;

			if (_timer >= 1) {
				_timer = 0;

				Transfer(_input, _output, _rate);
			}
		}
		private void Transfer(FluidContainer from, FluidContainer to, int max) {
			if (from.Fluid != to.Fluid) {
				to.SetFluid(from.Fluid);
			}
			
			var toTransfer = Mathf.Min(from.Stored, to.Capacity - to.Stored, max);
			from.SetStored(from.Stored - toTransfer);
			to.SetStored(to.Stored + toTransfer);
		} 
	}
}