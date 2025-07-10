using Game.SDK;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Scripting;

namespace Game.Utils {
	[Preserve]
	public class SDKLocaleSelector: IStartupLocaleSelector {
		[SerializeField] private bool _selectDefault = true;
		
		public Locale GetStartupLocale(ILocalesProvider availableLocales) {
			if (!GameSDK.IsReady()) {
				Debug.LogWarning("[SDKLocaleSelector] GameSDK is not ready.");
			}
			
			var sdkLang = GameSDK.GetLang();
			
			var locale = LocalizationSettings.AvailableLocales.GetLocale(sdkLang);
			if (locale == null && _selectDefault) {
				locale = LocalizationSettings.AvailableLocales.GetLocale(sdkLang == "ru" ? "ru" : "en");
			}

			if (locale) {
				Debug.Log($"[SDKLocaleSelector] Selected locale: {locale.Identifier.Code}. SDK: {sdkLang}");
			} else {
				Debug.Log($"[SDKLocaleSelector] Locale not selected. SDK: {sdkLang}");
			}
			return locale;
		}
	}
}