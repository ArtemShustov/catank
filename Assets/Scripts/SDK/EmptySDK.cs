using UnityEngine;

namespace Game.SDK {
	public class EmptySDK: IGameSDK {
		public bool IsReady() => true;
		public string GetLang() => null;
		public void GameReady() { }
		public bool IsMobile() => Application.isMobilePlatform;
	}
}