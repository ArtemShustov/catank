using System.Threading;
using System.Threading.Tasks;
using Game.Fluids;
using Game.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Locations {
	public class Bridge: MonoBehaviour {
		[SerializeField] private CinemachineCamera _camera;
		[SerializeField] private PopupHint _hint;
		[SerializeField] private PopupHint _extendedHint;
		[SerializeField] private Buildings.Bridge _bridge;
		[SerializeField] private GameObject _lockCollider;
		[SerializeField] private FluidContainer[] _containers;
		[SerializeField] private int[] _targetFills;
		[SerializeField] private GameObject _helpPickup;

		private bool _triggered;
		private bool _passed;

		private void Update() {
			if (_triggered && !_passed && CheckCondition()) {
				_passed = true;
				_bridge.AnimateDown();
				_lockCollider.gameObject.SetActive(false);
				_helpPickup.SetActive(false);
				Debug.Log($"Bridge passed!");
			}
		}

		public void ShowTutorial() {
			ShowTutorialAsync().Forget();
		}
		public void ShowTutorialExtended() {
			ShowTutotialExtendedAsync().Forget();
		}

		private async Task ShowTutorialAsync(CancellationToken cancellationToken = default) {
			GameRoot.Instance.Player.DisableInput();
			
			_camera.gameObject.SetActive(true);
			_camera.Priority = 1000;
			_hint.Show();
			
			await Awaitable.WaitForSecondsAsync(5, cancellationToken);
			
			_camera.gameObject.SetActive(false);
			_camera.Priority = 0;
			_hint.Hide();
				
			GameRoot.Instance.Player.EnableInput();
		}
		private async Task ShowTutotialExtendedAsync(CancellationToken cancellationToken = default) {
			GameRoot.Instance.Player.DisableInput();
			_camera.gameObject.SetActive(true);
			_camera.Priority = 1000;
			
			_hint.Show();
			await Awaitable.WaitForSecondsAsync(5, cancellationToken);
			_hint.Hide();
			
			_extendedHint.Show();
			await Awaitable.WaitForSecondsAsync(5, cancellationToken);
			_extendedHint.Hide();
				
			_camera.gameObject.SetActive(false);
			_camera.Priority = 0;
			GameRoot.Instance.Player.EnableInput();
		}
		private bool CheckCondition() {
			for (var i = 0; i < _containers.Length; i++) {
				var container = _containers[i];
				var targetFill = _targetFills[i];
				if (Mathf.Abs(container.Stored - targetFill) > 400) {
					return false;
				}
			}
			return true;
		}

		private void OnTriggerEnter(Collider other) {
			if (!_triggered && other.TryGetComponent<Player>(out var player)) {
				_triggered = true;
				ShowTutorial();
			}
		}
	}
}