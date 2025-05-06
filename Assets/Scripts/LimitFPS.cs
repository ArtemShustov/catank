using UnityEngine;

namespace Game {
	public static class LimitFPS {
		#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Initialize() {
			Application.targetFrameRate = 60;
		}
		#endif
	}
}