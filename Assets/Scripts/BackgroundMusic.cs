using System;
using UnityEngine;

namespace Game {
	public class BackgroundMusic: MonoBehaviour {
		[SerializeField] private float _volume = 1;
		[SerializeField] private AudioSource _source;
		[SerializeField] private AudioClip[] _clips;

		private int _index;
		
		private void Awake() {
			_source.clip = _clips[0];
			_source.Play();
		}

		private void Update() {
			if (!_source.clip) {
				return;
			}
			
			if (_source.time < 1f) {
				_source.volume = Mathf.Lerp(0f, _volume, _source.time);
			}
			var timeLeft = _source.clip.length - _source.time;
			if (timeLeft < 1f) {
				_source.volume = Mathf.Lerp(_volume, 0f, 1 - timeLeft);
			}

			if (!_source.isPlaying) {
				_index = _index >= _clips.Length - 1 ? 0 : _index + 1;
				_source.clip = _clips[_index];
				_source.Play();
			}
		}
	}
}