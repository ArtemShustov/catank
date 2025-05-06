using UnityEngine;

namespace Game.Utils {
	public class Rotate: MonoBehaviour {
		[SerializeField] private Vector3 _speed;

		private void Update() {
			transform.Rotate(_speed * Time.deltaTime);
		}
	}
}