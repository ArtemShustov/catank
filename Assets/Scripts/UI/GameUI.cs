using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.UI {
	public class GameUI: MonoBehaviour {
		[SerializeField] private InteractionHint _interactionHint;
		[SerializeField] private RocketPanel _rocketPanel;
		[SerializeField] private CanvasGroup _endPanel;
		
		public RocketPanel RocketPanel => _rocketPanel;
		public CanvasGroup EndPanel => _endPanel;
		
		private void Awake() {
			if (Instance != null && Instance != this) {
				Debug.LogWarning("Game UI is already exists.");
			} else {
				Instance = this;
			}
			
			_interactionHint.gameObject.SetActive(false);
			_rocketPanel.gameObject.SetActive(false);
			_endPanel.gameObject.SetActive(false);
		}

		public void SetInteractionHint(LocalizedString text) {
			_interactionHint.SetInteractionHint(text);
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