using UnityEngine;

namespace Game.Utils {
	public class FPSCounter: MonoBehaviour {
		[SerializeField] private GUIStyle _style;
		[SerializeField] private float _updateInterval = 0.25f;
		private float _baseFontSize;
		private float _accum = 0;
		private int _frames = 0;
		private float _timeLeft;
		private string _fpsString = "";

		private void Start() {
			_timeLeft = _updateInterval;
			_baseFontSize = _style.fontSize;
			AdjustFontSize();
		}
		private void Update() {
			AdjustFontSize();
			
			_timeLeft -= Time.deltaTime;
			_accum += Time.timeScale / Time.deltaTime;
			++_frames;

			if (_timeLeft <= 0.0) {
				float fps = _accum / _frames;
				_fpsString = $"{fps:F2} FPS";

				_timeLeft = _updateInterval;
				_accum = 0.0f;
				_frames = 0;
			}
		}

		private void AdjustFontSize() {
			float currentResolutionRatio = Screen.height / 1080f;
			_style.fontSize = Mathf.RoundToInt(_baseFontSize * currentResolutionRatio);
		}
		
		private void OnGUI() {
			GUILayout.Label(_fpsString, _style);
		}
	}
}