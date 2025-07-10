using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Game.Utils {
	[AddComponentMenu("Input/Touch On-Screen Look")]
	public class TouchOnScreenLook: OnScreenControl {
		[InputControl(layout = "Vector2")]
		[SerializeField] private string _controlPath;

		protected override string controlPathInternal {
			get => _controlPath;
			set => _controlPath = value;
		}

		private int _currentId = -1;
		private Vector2 _previousPosition;

		private void OnFingerDown(Finger finger) {
			if (_currentId == -1 && !InputUtils.IsOverUI(finger.screenPosition)) {
				_currentId = finger.index;
				_previousPosition = finger.screenPosition;
				// Debug.Log($"Touch started - ID: {_currentId}, Position: {finger.screenPosition}");
			}
		}
		private void OnFingerMove(Finger finger) {
			if (_currentId == finger.index) {
				Vector2 delta = finger.screenPosition - _previousPosition;
				// Debug.Log($"Touch moved - Delta: {delta}");
				SendValueToControl(delta);
				_previousPosition = finger.screenPosition;
			}
		}
		private void OnFingerUp(Finger finger) {
			if (_currentId == finger.index) {
				// Debug.Log($"Touch ended - ID: {finger.index}, Final Position: {finger.screenPosition}");
				SendValueToControl(Vector2.zero);
				_currentId = -1;
			}
		}
		
		protected override void OnEnable() {
			base.OnEnable();
			
			EnhancedTouchSupport.Enable();
			Touch.onFingerDown += OnFingerDown;
			Touch.onFingerUp += OnFingerUp;
			Touch.onFingerMove += OnFingerMove;
		}
		protected override void OnDisable() {
			base.OnDisable();
			
			Touch.onFingerDown -= OnFingerDown;
			Touch.onFingerUp -= OnFingerUp;
			Touch.onFingerMove -= OnFingerMove;
			EnhancedTouchSupport.Disable();
		}
	}
}