using Game.Locations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI.Tasks {
	public class LavaTask: MonoBehaviour {
		[SerializeField] private Sprite _check;
		[SerializeField] private Image _image;
		[FormerlySerializedAs("_trigger")]
		[SerializeField] private LavaLake _location;

		private void OnEnable() {
			_location.Completed += OnCompleted;
		}
		private void OnDisable() {
			_location.Completed -= OnCompleted;
		}
		private void OnCompleted() {
			_image.sprite = _check;
		}
	}
}