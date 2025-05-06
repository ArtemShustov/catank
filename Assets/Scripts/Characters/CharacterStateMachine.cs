using System.Linq;
using UnityEngine;

namespace Game.Characters {
	public class CharacterStateMachine: MonoBehaviour {
		[SerializeReference, SubclassSelector] private ICharacterState[] _states;
		[Header("Components")]
		[SerializeField] private CharacterController _controller;
		[SerializeField] private CharacterAnimator _animator;
		private DefaultInput _input;
		private Camera _camera;
		private ICharacterState _currentState;

		public CharacterData SharedData { get; } = new CharacterData();
		public DefaultInput Input => _input; // FIXME: Unsafe
		public CharacterController Controller => _controller;
		public CharacterAnimator Animator => _animator;
		public ICharacterState CurrentState => _currentState;

		private void Awake() {
			_camera = Camera.main;
			foreach (var state in _states) {
				state.Init(this);
			}
			ChangeState(_states[0]);
		}
		private void Update() {
			_currentState?.Update();
		}
		private void FixedUpdate() {
			_currentState?.FixedUpdate();
		}

		public void SetInput(DefaultInput input) {
			_input = input;
		}
		public void ChangeState(ICharacterState state) {
			_currentState?.OnExit();
			_currentState = state;
			_currentState?.OnEnter();
		}
		public void ChangeState<T>() where T: ICharacterState {
			var state = _states.First(s => s is T);
			ChangeState(state);
		}
		
		public Vector2 GetRelatedMove() {
			var input = _input.Player.Move.ReadValue<Vector2>();
			
			if (!_camera) {
				return input;
			}

			var directionAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
			if (directionAngle < 0) {
				directionAngle += 360;
			}
			directionAngle += _camera.transform.eulerAngles.y;
			if (directionAngle > 360) {
				directionAngle -= 360;
			}
			var forward = Quaternion.Euler(0, directionAngle, 0) * Vector3.forward;
			var result = forward * input.magnitude;
			return new Vector2(result.x, result.z);
		}
	}
}