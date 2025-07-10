using System;
using System.Threading;
using System.Threading.Tasks;
using Game.Buildings;
using Game.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Locations {
	public class Pumps: MonoBehaviour {
		[SerializeField] private PopupHint[] _hints;
		[SerializeField] private CinemachineCamera _tutorCamera;
		[SerializeField] private Pump[] _pumps;

		private CancellationTokenSource _source;
		private bool _triggered;

		public event Action<int> OilTaken;
		
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
			await ShowTutorialAsync(cancellationToken);
		}
		private async Task ShowTutorialAsync(CancellationToken cancellationToken) {
			GameRoot.Instance.Player.DisableInput();
			
			_tutorCamera.gameObject.SetActive(true);
			_tutorCamera.Priority = 1000;

			foreach (var hint in _hints) {
				hint.Show();
				await Awaitable.WaitForSecondsAsync(5f, cancellationToken);
				hint.Hide();
			}
			
			_tutorCamera.gameObject.SetActive(false);
			_tutorCamera.Priority = 0;
			
			GameRoot.Instance.Player.EnableInput();
		}
		
		private void OnTriggerEnter(Collider other) {
			if (!_triggered && other.TryGetComponent<Player>(out var player)) {
				Activate();
			}
		}
		private void OnEnable() {
			foreach (var pump in _pumps) {
				pump.Output.StoredChanged += OnStoredChanged;
			}
		}
		private void OnStoredChanged(int before, int after) {
			var delta = after - before;
			if (delta < 0) {
				OilTaken?.Invoke(Mathf.Abs(delta));
			}
		}
	}
}