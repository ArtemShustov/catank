using System.Threading;
using System.Threading.Tasks;
using EasyButtons;
using Game.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Locations {
	public class MainIsland: MonoBehaviour {
		[SerializeField] private CinemachineCamera _camera;
		[SerializeField] private PopupHint[] _hints;
		[SerializeField] private Player _player;
		[SerializeField] private Transform _spawnPoint;

		private CancellationTokenSource _source;

		private void Start() {
			Show();
		}

		[Button(Mode = ButtonMode.EnabledInPlayMode)]
		public void Show() {
			_source?.Cancel();
			_source = new CancellationTokenSource();
			ShowAsync(_source.Token).Forget();
		}
		[Button(Mode = ButtonMode.EnabledInPlayMode)]
		public void Hide() {
			_source?.Cancel();
			_source = new CancellationTokenSource();
			HideAsync(_source.Token).Forget();
		}
		private async Task ShowAsync(CancellationToken token) {
			GameUI.Instance.RocketPanel.ButtonClicked += OnStartButton;
			
			_player.gameObject.SetActive(false);
			_camera.gameObject.SetActive(true);
			_camera.Priority = 1000;
			await GameUI.Instance.RocketPanel.ShowAsync(token);
		}
		private async Task HideAsync(CancellationToken token) {
			GameUI.Instance.RocketPanel.ButtonClicked -= OnStartButton;
			
			await GameUI.Instance.RocketPanel.HideAsync(token);
			
			foreach (var hint in _hints) {
				hint.Show();
				await Awaitable.WaitForSecondsAsync(5, token);
				hint.Hide();
			}
			
			_camera.gameObject.SetActive(false);
			_camera.Priority = 0;
			_player.transform.position = _spawnPoint.position;
			_player.gameObject.SetActive(true);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void OnStartButton() {
			Hide();
		}

		private void OnDestroy() {
			GameUI.Instance.RocketPanel.ButtonClicked -= OnStartButton;
		}
	}
}