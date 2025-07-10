namespace Game.SDK {
	public interface IGameSDK {
		bool IsReady();
		string GetLang();
		void GameReady();
		bool IsMobile();
	}
}