using System.Threading;
using System.Threading.Tasks;
using Game.SDK;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game {
	public class LevelBoot: MonoBehaviour {
		[SerializeField] private GameUI _ui;
		[Space]
		[SerializeField] private GameObject _loadingPanel;
		[SerializeField] private LocalizedString _loadingText;
		[SerializeField] private TMP_Text _loadingLabel;
		[SerializeField] private Transform _loadingCircle;

		private void Awake() {
			LoadAsync(destroyCancellationToken).Forget();
		}
		private async Task LoadAsync(CancellationToken token = default) {
			Debug.Log($"[LevelBoot] SDK: {GameSDK.IsReady()}");
			_loadingLabel.text = $"0%";
			_loadingPanel.SetActive(true);
			
			await WaitForSDK(token);
			await ApplySettings(token);
			await WaitForLangFix(token);
			await WaitForLocaleLoading(token);

			_loadingPanel.SetActive(false);
			GameSDK.GameReady();
			
			Debug.Log($"[LevelBoot] Game ready");
		}
		
		private async Task WaitForSDK(CancellationToken token) {
			while (!GameSDK.IsReady()) {
				_loadingCircle.Rotate(Vector3.forward, -360 * Time.deltaTime);
				await Awaitable.NextFrameAsync(token);
			}
		}
		private async Task ApplySettings(CancellationToken token) {
			_ui.SetIsMobile(GameSDK.IsMobile());
			
			var targetLevel = GameSDK.IsMobile() ? 0 : 1;
			QualitySettings.SetQualityLevel(targetLevel, true);

			await Task.CompletedTask;
		} 
		private async Task WaitForLangFix(CancellationToken token) {
			var sdkLang = GameSDK.GetLang();
			var localeLoading = LocalizationSettings.SelectedLocaleAsync;
			while (!localeLoading.IsDone) {
				_loadingCircle.Rotate(Vector3.forward, -360 * Time.deltaTime);
				await Awaitable.NextFrameAsync(token);
			}
			var currentLang = localeLoading.Result.Identifier.Code;
			if (sdkLang != currentLang) {
				Debug.Log("[LevelBoot] Fixing language");
				var locale = LocalizationSettings.AvailableLocales.GetLocale(sdkLang);
				if (locale == null && currentLang != "en") {
					Debug.Log($"[LevelBoot] Locale '{sdkLang}' not found");
					locale = LocalizationSettings.AvailableLocales.GetLocale("en");
					LocalizationSettings.SelectedLocale = locale;
				}
			}
		}
		private async Task WaitForLocaleLoading(CancellationToken token) {
			var localizationLoading = _loadingText.GetLocalizedStringAsync();
			while (!localizationLoading.IsDone) {
				_loadingCircle.Rotate(Vector3.forward, -360 * Time.deltaTime);
				await Awaitable.NextFrameAsync(token);
			}
		}
	}
}