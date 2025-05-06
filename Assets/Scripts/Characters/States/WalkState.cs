using System;
using UnityEngine;

namespace Game.Characters.States {
	[Serializable]
	public class WalkState: AbstractState {
		[SerializeField] private float _speed = 10;
		[SerializeField] private float _rotateSpeed = 5;
		[SerializeField] private float _acceleration = 5;
		[Space]
		[SerializeField] private float _stepDelay = 0.2f;
		[SerializeField] private AudioClip _stepSound;
		[SerializeField] private AudioSource _source;

		private Vector3 _lastMoveInput;
		private float _currentSpeed;
		private float _stepTimer;
		
		public override void Update() {
			if (Character.Input.Player.Jump.WasPerformedThisFrame()) {
				Character.ChangeState<JumpState>();
				return;
			}
			if (GetMoveInput().sqrMagnitude < Mathf.Epsilon) {
				if (_currentSpeed > 0) {
					_currentSpeed -= _acceleration * Time.deltaTime;
					Character.Controller.Move(_lastMoveInput.normalized * (_speed * _currentSpeed * Time.deltaTime));
					return;
				}
				Character.ChangeState<IdleState>();
				return;
			} else {
				if (_currentSpeed < 1) {
					_currentSpeed += _acceleration * Time.deltaTime;
				}
			}
			
			_stepTimer += Time.deltaTime;
			if (_stepTimer >= _stepDelay) {
				_stepTimer = 0;
				var pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_source.pitch = pitch;
				_source.PlayOneShot(_stepSound);
			}
			
			Move();
			RotateToMovement();
		}
		public override void OnEnter() {
			Character.Animator.SetIsMove(true);
			_currentSpeed = Character.SharedData.Velocity.sqrMagnitude > Mathf.Epsilon ? 1 : 0;
		}
		public override void OnExit() {
			Character.Animator.SetIsMove(false);
		}

		private void Move() {
			var input = Convert(Character.GetRelatedMove().normalized);
			var movement = input * (_speed * _currentSpeed * Time.deltaTime);
			movement += Physics.gravity * Time.deltaTime;
			Character.Controller.Move(movement);
			Character.SharedData.Velocity = movement / Time.deltaTime;
			
			_lastMoveInput = input;
		}
		private void RotateToMovement() {
			var moveDirection = Character.GetRelatedMove();
			if (moveDirection.sqrMagnitude < Mathf.Epsilon) {
				return;
			}
			
			var targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.y));
			var transform = Character.Controller.transform;
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				targetRotation,
				Time.deltaTime * _rotateSpeed * (Quaternion.Angle(targetRotation, transform.rotation) > 90 ? 1 : 2)
			);
		}
		private Vector2 GetMoveInput() => Character.Input.Player.Move.ReadValue<Vector2>();
		private Vector3 Convert(Vector2 vector2) {
			return new Vector3(vector2.x, 0, vector2.y);
		}
	}
}