using System.Runtime.InteropServices;
using UnityEngine;

namespace Game.SDK {
	public class YandexSDK: IGameSDK {
		#if UNITY_WEBGL && YANDEX_SDK
		[DllImport("__Internal")]
		private static extern bool YSDK_IsReady();
		[DllImport("__Internal")]
		private static extern string YSDK_GetLang();
		[DllImport("__Internal")]
		private static extern void YSDK_GameReady();
		[DllImport("__Internal")]
		private static extern bool YSDK_IsMobile();
		#endif
		
		public bool IsReady() {
			#if UNITY_WEBGL && YANDEX_SDK
			return YSDK_IsReady();
			#else
			return true;
			#endif
		}
		public string GetLang() {
			#if UNITY_WEBGL && YANDEX_SDK
			var lang = YSDK_GetLang();
			Debug.Log("YSDK_GetLang: " + lang);
			return lang;
			#else
			return null;
			#endif
		}
		public void GameReady() {
			#if UNITY_WEBGL && YANDEX_SDK
			YSDK_GameReady();
			#else
			return;
			#endif
		}
		public bool IsMobile() {
			#if UNITY_WEBGL && YANDEX_SDK
			return YSDK_IsMobile();
			#else
			return false;
			#endif
		}
	}
}