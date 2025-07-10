using System;
using Game.Fluids;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Buildings {
	public class Pump: MonoBehaviour {
		[SerializeField] private int _produce = 200;
		[SerializeField] private Vector2 _workArea = Vector2.up;
		[SerializeField] private Gradient _tempGradient;
		[SerializeField] private float _activeDelta = 0.1f;
		[SerializeField] private float _passiveDelta = 0.1f;
		[SerializeField] private Fluid _oil;
		[SerializeField] private Fluid _heat;
		[SerializeField] private Fluid _cool;
		[SerializeField] private FluidContainer _controlTank;
		[SerializeField] private FluidContainer _output;
		[SerializeField] private MeshRenderer _lamp;
		[SerializeField] private AudioSource _source;
		
		private MaterialPropertyBlock _lampBlock;
		private State _state;
		private float _temp;
		private float _timer;

		public bool IsWorking => _temp >= _workArea.x && _temp <= _workArea.y;
		public FluidContainer Output => _output;
		public event Action TemperatureChanged;
		
		private void Awake() {
			_output.SetFluid(_oil);
			_lampBlock = new MaterialPropertyBlock();
			UpdateLamp();
		}
		private void Update() {
			_timer += Time.deltaTime;
			if (_timer >= 1f) {
				OnTick();
				_timer = 0f;
			}
		}

		private void OnTick() {
			var stateByFluid = _controlTank.Fluid switch {
				var f when f == _heat => State.ActiveHeating,
				var f when f == _cool => State.ActiveCooling,
				_ => State.None
			};
			var state = _state switch {
				State.None when stateByFluid != State.None => stateByFluid,
				State.Cooling when stateByFluid != State.None => stateByFluid,
				State.Heating when stateByFluid != State.None => stateByFluid,
				State.ActiveCooling when stateByFluid == State.None => Random.Range(0, 2) == 0 ? State.Cooling : State.Heating,
				State.ActiveHeating when stateByFluid == State.None => Random.Range(0, 2) == 0 ? State.Cooling : State.Heating,
				_ => _state
			};
			if (state != _state) {
				// Debug.Log($"State changed: {_state} > {state}");
				_state = state;
			}
			
			var delta = _state switch {
				State.None => 0,
				State.Heating => _passiveDelta,
				State.Cooling => -_passiveDelta,
				State.ActiveHeating => _activeDelta,
				State.ActiveCooling => -_activeDelta,
				_ => throw new ArgumentOutOfRangeException()
			};
			if (delta != 0) {
				_temp = Mathf.Clamp01(_temp + delta);
				
				UpdateLamp();

				TemperatureChanged?.Invoke();
				
				if (_source.isPlaying != IsWorking) {
					if (IsWorking) {
						_source.Play();
					} else {
						_source.Stop();
					}
				}
			}

			if (IsWorking) {
				_output.SetStored(_output.Stored + _produce);
			}
		}
		private void UpdateLamp() {
			_lamp.GetPropertyBlock(_lampBlock);
			var color = _tempGradient.Evaluate(_temp);
			_lampBlock.SetColor("_LitColor", color * 10);
			_lampBlock.SetColor("_DarkColor", color * 10);
			_lamp.SetPropertyBlock(_lampBlock);
		}
		
		private enum State {
			None,
			Heating, Cooling, 
			ActiveHeating, ActiveCooling,
		}
	}
}