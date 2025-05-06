using UnityEngine;

namespace Game {
	public class Test: MonoBehaviour {
		private void Update() {
			var time = Mathf.PingPong(Time.time, 1);
			transform.position = Vector3.Lerp(Vector3.left, Vector3.right, time);
		}
	}
}