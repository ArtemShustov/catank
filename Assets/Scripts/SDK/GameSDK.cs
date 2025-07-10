using UnityEngine;

namespace Game.SDK {
	public static class GameSDK {
		private static IGameSDK _current;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Initialize() {
			#if !UNITY_EDITOR && (UNITY_WEBGL && YANDEX_SDK)
			_current = new YandexSDK();
			#else
			_current = new EmptySDK();
			#endif
			Debug.Log($"GameSDK initialized as '{_current?.GetType()}'.");
		}

		public static bool IsReady() => _current?.IsReady() ?? true;
		public static string GetLang() => _current?.GetLang() ?? string.Empty;
		public static void GameReady() => _current?.GameReady();
		public static bool IsMobile() => _current?.IsMobile() ?? Application.isMobilePlatform;
	}

}