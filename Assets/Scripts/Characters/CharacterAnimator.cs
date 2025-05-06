using UnityEngine;

namespace Game.Characters {
	public class CharacterAnimator: MonoBehaviour {
		[SerializeField] private string _isMoveKey = "IsMove";
		[SerializeField] private string _isJumpKey = "IsJump";
		[SerializeField] private string _isFallKey = "IsFall";
		[SerializeField] private Animator _animator;

		public void SetIsMove(bool isMove) {
			_animator.SetBool(_isMoveKey, isMove);
		}
		public void SetIsJump(bool isJump) {
			_animator.SetBool(_isJumpKey, isJump);
		}
		public void SetIsFall(bool isFall) {
			_animator.SetBool(_isFallKey, isFall);
		}
	}
}