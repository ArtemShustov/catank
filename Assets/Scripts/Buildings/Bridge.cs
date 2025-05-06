using System.Threading;
using System.Threading.Tasks;
using EasyButtons;
using UnityEngine;

namespace Game.Buildings {
	public class Bridge: MonoBehaviour {
		[SerializeField] private Vector3 _upAngle = new Vector3(30f, 0f, 0f);
		[SerializeField] private Transform _root;
		[SerializeField] private float _duration = 1f;

		private void Awake() {
			AnimateUp();
		}

		[Button(Mode = ButtonMode.EnabledInPlayMode)]
		public void AnimateUp() {
			AnimateAsync(Vector3.zero, _upAngle).Forget();
		}
		[Button(Mode = ButtonMode.EnabledInPlayMode)]
		public void AnimateDown() {
			AnimateAsync(_upAngle, Vector3.zero).Forget();
		}

		private async Task AnimateAsync(Vector3 start, Vector3 end, CancellationToken cancellationToken = default) {
			var t = 0f;
			while (t < _duration) {
				t += Time.deltaTime;
				_root.localEulerAngles = Vector3.Lerp(start, end, t / _duration);
				await Awaitable.NextFrameAsync(cancellationToken);
			}
			_root.localEulerAngles = end;
		}
	}
}