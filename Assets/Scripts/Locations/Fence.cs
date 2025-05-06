using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Buildings;
using Game.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Locations {
	public class Fence: MonoBehaviour {
		[SerializeField] private Transform _fence;
		[SerializeField] private Vector3 _localPosition;
		[SerializeField] private float _animationDuration = 1f;
		[SerializeField] private CinemachineCamera _tutorialCamera;
		[SerializeField] private Pylon[] _pylons;
		[SerializeField] private GameObject _backwardWall;
		[SerializeField] private CinemachineCamera _fenceCamera;
		[SerializeField] private GameObject _helpPickup;
		[SerializeField] private PopupHint _hint;
		[SerializeField] private PopupHint _hint2;

		private CancellationTokenSource _source;
		private bool _triggered;
		private bool _passed;

		private void Activate() {
			_triggered = true;
			
			_source?.Cancel();
			_source = new CancellationTokenSource();
			ShowAsync(_source.Token).Forget();
		}
		public void ShowTutorial() {
			ShowTutorialAsync(default).Forget();
		}
		private async Task ShowAsync(CancellationToken cancellationToken) {
			_backwardWall.SetActive(true);
			
			await ShowTutorialAsync(cancellationToken);

			while (_triggered && !_passed) {
				if (_pylons.All(p => p.Active)) {
					await OnPassed(cancellationToken);
					break;
				}
				await Awaitable.NextFrameAsync(cancellationToken);
			}
		}
		private async Task ShowTutorialAsync(CancellationToken cancellationToken) {
			GameRoot.Instance.Player.DisableInput();
			
			_hint.Show();
			_tutorialCamera.gameObject.SetActive(true);
			_tutorialCamera.Priority = 1000;
			await Awaitable.WaitForSecondsAsync(5, cancellationToken);
			_hint.Hide();
			_hint2.Show();
			await Awaitable.WaitForSecondsAsync(5, cancellationToken);
			_hint2.Hide();
			_tutorialCamera.gameObject.SetActive(false);
			_tutorialCamera.Priority = 0;
			
			GameRoot.Instance.Player.EnableInput();
		}
		private async Task OnPassed(CancellationToken cancellationToken) {
			_passed = true;
			Debug.Log("Fence passed!");
			
			foreach (var pylon in _pylons) {
				pylon.Lock();
			}
			_helpPickup.SetActive(false);
			_backwardWall.SetActive(false);
			_fenceCamera.gameObject.SetActive(true);
			_fenceCamera.Priority = 1000;

			await Awaitable.WaitForSecondsAsync(1, cancellationToken);
			await AnimateFence(cancellationToken);
					
			_fenceCamera.gameObject.SetActive(false);
			_fenceCamera.Priority = 0;
		}
		private async Task AnimateFence(CancellationToken cancellationToken) {
			var t = 0f;
			while (t < _animationDuration) {
				t += Time.deltaTime;
				_fence.localPosition = Vector3.Lerp(Vector3.zero, _localPosition, t / _animationDuration);
				await Awaitable.NextFrameAsync(cancellationToken);
			}
			_fence.localPosition = _localPosition;
		}
		
		private void OnTriggerEnter(Collider other) {
			if (!_triggered && other.TryGetComponent<Player>(out var player)) {
				Activate();
			}
		}
	}
}