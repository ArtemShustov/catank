using System;
using System.Threading;
using System.Threading.Tasks;
using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {
	public class RocketPanel: MonoBehaviour {
		[SerializeField] private Button _button;
		[SerializeField] private RectTransform _buttonRect;
		[SerializeField] private float _animationDuration = 1f;
		
		public event Action ButtonClicked;

		private CancellationTokenSource _source;
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
		
		public async Task ShowAsync(CancellationToken cancellationToken = default) {
			gameObject.SetActive(true);
			_button.interactable = false;
			
			var endPos = _buttonRect.anchoredPosition;
			var startPos = _buttonRect.anchoredPosition;
			startPos.y *= -1;
			
			var t = 0f;
			while (t < _animationDuration) {
				t += Time.deltaTime;
				_buttonRect.anchoredPosition = Vector3.Lerp(startPos, endPos, t / _animationDuration);
				await Awaitable.NextFrameAsync(cancellationToken);
			}
			_buttonRect.anchoredPosition = endPos;
			_button.interactable = true;
		}
		public async Task HideAsync(CancellationToken cancellationToken = default) {
			_button.interactable = false;
			
			var endPos = _buttonRect.anchoredPosition;
			var startPos = _buttonRect.anchoredPosition;
			endPos.y *= -1;
			
			var t = 0f;
			while (t < _animationDuration) {
				t += Time.deltaTime;
				_buttonRect.anchoredPosition = Vector3.Lerp(startPos, endPos, t / _animationDuration);
				await Awaitable.NextFrameAsync(cancellationToken);
			}
			
			_buttonRect.anchoredPosition = startPos;
			gameObject.SetActive(false);
		}

		private void OnButtonClicked() {
			ButtonClicked?.Invoke();
		}
		private void OnEnable() {
			_button.onClick.AddListener(OnButtonClicked);
		}
		private void OnDisable() {
			_button.onClick.RemoveListener(OnButtonClicked);
		}
	}
}