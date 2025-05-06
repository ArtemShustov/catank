using System.Threading;
using System.Threading.Tasks;
using Game.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Locations {
	public class Refinery: MonoBehaviour {
		[SerializeField] private CinemachineCamera _tutorCamera;
		[SerializeField] private PopupHint _hint;

		private CancellationTokenSource _source;
		private bool _triggered;

		public void ShowTutorial() {
			_source?.Cancel();
			_source = new CancellationTokenSource();
			ShowTutorial(_source.Token).Forget();
		}
		private async Task ShowTutorial(CancellationToken token) {
			GameRoot.Instance.Player.DisableInput();
			_tutorCamera.gameObject.SetActive(true);
			_tutorCamera.Priority = 1000;
			_hint.Show();
			
			await Awaitable.WaitForSecondsAsync(5f, token);
			
			_tutorCamera.gameObject.SetActive(false);
			_tutorCamera.Priority = 0;
			_hint.Hide();
			GameRoot.Instance.Player.EnableInput();
		}
		private void OnTriggerEnter(Collider other) {
			if (!_triggered && other.TryGetComponent<Player>(out var player)) {
				_triggered = true;
				ShowTutorial();
			}
		}
	}
}