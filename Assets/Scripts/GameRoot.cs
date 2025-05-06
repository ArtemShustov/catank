using Game.UI;
using UnityEngine;

namespace Game {
	public class GameRoot: MonoBehaviour {
		[field: SerializeField] public Player Player { get; private set; }
		[field: SerializeField] public GameUI UI { get; private set; }
		[field: SerializeField] public BubbleEffect BubbleEffect { get; private set; }
		
		private void Awake() {
			if (Player == null || UI == null) {
				Debug.LogWarning("GameRoot is not initialized!");
			}
			Instance = this;
			Player.EnableInput();
		}
		
		public static GameRoot Instance { get; private set; }
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Initialize() {
			Instance = null;
		}
	}
}