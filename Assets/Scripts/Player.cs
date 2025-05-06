using Game.Characters;
using Game.Fluids;
using Game.Interactions;
using UnityEngine;

namespace Game {
	public class Player: MonoBehaviour {
		[SerializeField] private PlayerInteractor _interactor;
		[SerializeField] private CharacterStateMachine _machine;
		[SerializeField] private FluidContainer _fluidContainer;
		private DefaultInput _input;
		
		public FluidContainer FluidContainer => _fluidContainer;

		private void Awake() {
			if (_input == null) {
				InitInput();
			}
		}
		private void InitInput() {
			_input = new DefaultInput();
			_machine.SetInput(_input);
			_interactor.SetInput(_input);
		}

		public void EnableInput() {
			if (_input == null) {
				InitInput();
			}
			_input?.Player.Enable();
		}
		public void DisableInput() {
			if (_input == null) {
				InitInput();
			}
			_input?.Player.Disable();
		}
	}
}