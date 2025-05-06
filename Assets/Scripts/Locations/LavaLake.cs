using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Buildings;
using Game.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Locations {
	public class LavaLake: MonoBehaviour {
		[SerializeField] private PopupHint _lakeHint;
		[SerializeField] private CinemachineCamera _lakeCamera;
		[SerializeField] private PopupHint _generatorsHint;
		[SerializeField] private CinemachineCamera _generatorsCamera;
		[SerializeField] private GameObject _backwardWall;
		[SerializeField] private GameObject _forwardWall;
		[SerializeField] private GeoGenerator[] _generators;
		[SerializeField] private GameObject _helpPickup;

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
			_forwardWall.SetActive(true);
			
			await ShowTutorialAsync(cancellationToken);

			while (_triggered && !_passed) {
				if (_generators.All(g => g.Fill >= 0.5f)) {
					_passed = true;
					Debug.Log("LavaLake passed!");
					_helpPickup.SetActive(false);
					_backwardWall.SetActive(false);
					_forwardWall.SetActive(false);
					break;
				}
				await Awaitable.NextFrameAsync(cancellationToken);
			}
		}
		private async Task ShowTutorialAsync(CancellationToken cancellationToken) {
			GameRoot.Instance.Player.DisableInput();
			
			_lakeHint.Show();
			await _lakeCamera.ToggleCameraAsync(5, cancellationToken);
			_lakeHint.Hide();
			
			_generatorsHint.Show();
			await _generatorsCamera.ToggleCameraAsync(5, cancellationToken);
			_generatorsHint.Hide();
			
			GameRoot.Instance.Player.EnableInput();
		}
		private void OnTriggerEnter(Collider other) {
			if (!_triggered && other.TryGetComponent<Player>(out var player)) {
				Activate();
			}
		}
	}
}