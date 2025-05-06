using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.UI {
	public class PopupHint: MonoBehaviour {
		[SerializeField] private Transform _canvas;
		[SerializeField] private float _animationDuration = 1;
		[SerializeField] private AnimationCurve _scaleCurve;
		private Camera _camera;
		private CancellationTokenSource _animation;

		private void Awake() {
			_camera = Camera.main;
			_canvas.gameObject.SetActive(false);
		}
		private void Update() {
			if (_canvas.gameObject.activeSelf) {
				_canvas.rotation = _camera.transform.rotation;
			}
		}

		public void Show() {
			_animation?.Cancel();
			_animation = new CancellationTokenSource();
			ShowAsync(_animation.Token).Forget();
		}
		public void Hide() {
			_animation?.Cancel();
			_animation = new CancellationTokenSource();
			HideAsync(_animation.Token).Forget();
		}
		
		private void OnTriggerEnter(Collider other) {
			if (other.TryGetComponent<Player>(out var player)) {
				_animation?.Cancel();
				_animation = new CancellationTokenSource();
				ShowAsync(_animation.Token).Forget();
			}
		}
		private void OnTriggerExit(Collider other) {
			if (other.TryGetComponent<Player>(out var player)) {
				_animation?.Cancel();
				_animation = new CancellationTokenSource();
				HideAsync(_animation.Token).Forget();
			}
		}

		public async Task ShowAsync(CancellationToken cancellationToken) {
			_canvas.gameObject.SetActive(true);
			var t = 0f;
			while (t < _animationDuration) {
				t += Time.deltaTime;
				_canvas.localScale = Vector3.one * _scaleCurve.Evaluate(t / _animationDuration);
				await Awaitable.NextFrameAsync(cancellationToken);
			}
			_canvas.localScale = Vector3.one;
		}
		public async Task HideAsync(CancellationToken cancellationToken) {
			var t = 0f;
			while (t < _animationDuration) {
				t += Time.deltaTime;
				_canvas.localScale = Vector3.one * _scaleCurve.Evaluate(1 - t / _animationDuration);
				await Awaitable.NextFrameAsync(cancellationToken);
			}
			_canvas.localScale = Vector3.one;
			_canvas.gameObject.SetActive(false);
		}
	}
}