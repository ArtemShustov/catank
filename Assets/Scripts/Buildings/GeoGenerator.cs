using Game.Fluids;
using Game.Utils;
using UnityEngine;

namespace Game.Buildings {
	public class GeoGenerator: MonoBehaviour {
		[SerializeField] private int _usage = 1000;
		[Header("Components")]
		[SerializeField] private FluidContainer _container;
		[SerializeField] private AudioSource _source;
		[SerializeField] private Rotate _fan;
		private float _timer;
		
		public float Fill => _container.Fill;
		
		private void Awake() {
			OnFillChanged();
		}
		private void Update() {
			_timer += Time.deltaTime;
			if (_timer >= 1) {
				var use = Mathf.Min(_usage, _container.Stored);
				if (use > 0) {
					_container.SetStored(_container.Stored - use);
				}
				_timer = 0;
			}
		}

		private void OnFillChanged() {
			var isWorking = _container.Fill != 0;
			_fan.enabled = isWorking;
			if (isWorking) {
				_source.Play();
			} else {
				_source.Stop();
			}
		}
		private void OnEnable() {
			_container.FillChanged += OnFillChanged;
		}
		private void OnDisable() {
			_container.FillChanged -= OnFillChanged;
		}
	}
}