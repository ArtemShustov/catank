using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.UI {
	public class GameUI: MonoBehaviour {
		[SerializeField] private InteractionHint _interactionHint;
		[SerializeField] private GameObject _interactionButton;
		[SerializeField] private RocketPanel _rocketPanel;
		[SerializeField] private CanvasGroup _endPanel;
		[SerializeField] private GameObject _onScreenControl;
		[SerializeField] private GameObject _gamePanel;
		private bool _isMobile;
		
		public RocketPanel RocketPanel => _rocketPanel;
		public CanvasGroup EndPanel => _endPanel;
		
		private void Awake() {
			if (Instance != null && Instance != this) {
				Debug.LogWarning("Game UI is already exists.");
			} else {
				Instance = this;
			}
			
			_rocketPanel.gameObject.SetActive(false);
			_endPanel.gameObject.SetActive(false);
			_interactionButton.SetActive(false);
			_interactionHint.gameObject.SetActive(false);
			_onScreenControl.SetActive(false);
		}

		public void SetIsMobile(bool isMobile) {
			_isMobile = isMobile;
			var isActive = _interactionButton.activeSelf || _interactionHint.gameObject.activeSelf;
			_interactionHint.gameObject.SetActive(isActive && !isMobile);
			_interactionButton.SetActive(isActive && isMobile);
			_onScreenControl.SetActive(isMobile);
		}
		public void SetInteractionHint(LocalizedString text) {
			if (_isMobile) {
				_interactionButton.SetActive(text != null);
			} else {
				_interactionHint.gameObject.SetActive(text != null);
				_interactionHint.SetInteractionHint(text);
			}
		}
		public void SetGamePanel(bool active) {
			_gamePanel.SetActive(active);
		}

		public async Task ShowEndPanelAsync(CancellationToken cancellationToken) {
			var duration = 2f;
			
			_endPanel.gameObject.SetActive(true);
			var t = 0f;
			while (t < duration) {
				t += Time.deltaTime;
				_endPanel.alpha = t / duration;
				await Awaitable.NextFrameAsync(cancellationToken);
			}
			_endPanel.alpha = 1;
		}
		
		public static GameUI Instance { get; private set; }
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Initialize() {
			Instance = null;
		}
	}
}