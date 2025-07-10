using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.UI {
	public class CanvasAlphaAnimation: MonoBehaviour {
		[SerializeField] private CanvasGroup _group;
		[SerializeField] private float _duration = 1;

		private CancellationTokenSource _animation;
		
		private async Task ShowAsync(CancellationToken token = default) {
			if (token == default) {
				token = destroyCancellationToken;
			}
			
			var t = 0f;
			while (t < _duration) {
				t += Time.deltaTime;
				_group.alpha = Mathf.Lerp(0f, 1f, t / _duration);
				await Awaitable.NextFrameAsync(token);
			}
			_group.alpha = 1f;
		}

		private void OnEnable() {
			_animation?.Dispose();
			_animation = new CancellationTokenSource();
			ShowAsync(_animation.Token).Forget();
		}
		private void OnDisable() {
			_animation?.Cancel();
			_animation?.Dispose();
			_animation = null;
		}
	}
}