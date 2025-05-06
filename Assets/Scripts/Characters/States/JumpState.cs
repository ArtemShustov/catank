using System;
using UnityEngine;

namespace Game.Characters.States {
	[Serializable]
	public class JumpState: AbstractState {
		[SerializeField] private float _force = 15;
		[SerializeField] private float _height = 2;
		[SerializeField] private float _duration = 0.5f;
		[SerializeField] private AnimationCurve _heightCurve;
		[SerializeField] private AudioSource _source;
		[SerializeField] private AudioClip _sound;
		
		private float _timer;

		public override void Update() {
			_timer += Time.deltaTime;
			var height = _height * _heightCurve.Evaluate(_timer / _duration);
			var movement = height * Time.deltaTime / _duration * Vector3.up;
			movement += Character.SharedData.Velocity * Time.deltaTime;
			Character.Controller.Move(movement);
			
			if (_timer >= _duration) {
				Character.ChangeState<FallState>();
				return;
			}
		}
		public override void OnEnter() {
			_timer = 0;
			var input = Convert(Character.GetRelatedMove().normalized);
			Character.SharedData.Velocity = input * _force;
			Character.Animator.SetIsJump(true);
			_source.PlayOneShot(_sound);
			RotateTo(input);
		}
		public override void OnExit() {
			Character.Animator.SetIsJump(false);
		}

		private Vector3 Convert(Vector2 vector2) {
			return new Vector3(vector2.x, 0, vector2.y);
		}
		private void RotateTo(Vector3 input) {
			if (input.sqrMagnitude < Mathf.Epsilon) {
				return;
			}
			
			Character.Controller.transform.rotation = Quaternion.LookRotation(input);
		}
	}
}