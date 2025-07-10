using System;
using System.Threading;
using System.Threading.Tasks;
using Game.Fluids;
using Game.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Game {
	public class EndGame: MonoBehaviour {
		[SerializeField] private AnimationCurve _speed;
		[SerializeField] private FluidContainer _rocket;
		[SerializeField] private CinemachineCamera _camera;
		[SerializeField] private ParticleSystem _fire;
		[SerializeField] private GameUI _ui;

		private bool _triggered;
		
		private void Update() {
			if (!_triggered && Mathf.Approximately(_rocket.Fill, 1)) {
				_triggered = true;
				_camera.gameObject.SetActive(true);
				_camera.Priority = 1000;
				GameRoot.Instance.Player.gameObject.SetActive(false);
				GameUI.Instance.RocketPanel.Show();
				GameUI.Instance.RocketPanel.ButtonClicked += OnButtonClicked;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				_ui.SetGamePanel(false);
			}
		}
		private void OnButtonClicked() {
			GameUI.Instance.RocketPanel.ButtonClicked -= OnButtonClicked;
			ShowEndAsync(default).Forget();
		}
		private async Task ShowEndAsync(CancellationToken cancellationToken) {
			_fire.Play();
			await GameUI.Instance.RocketPanel.HideAsync(cancellationToken);
			GameUI.Instance.ShowEndPanelAsync(cancellationToken).Forget();

			var t = 0f;
			while (!cancellationToken.IsCancellationRequested) {
				t += Time.deltaTime;
				_rocket.transform.position += Vector3.up * Time.deltaTime * _speed.Evaluate(t);
				await Awaitable.NextFrameAsync(cancellationToken);
			}
		}

		private void OnDestroy() {
			GameUI.Instance.RocketPanel.ButtonClicked -= OnButtonClicked;
		}
	}
}