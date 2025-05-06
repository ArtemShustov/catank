using System.Threading;
using System.Threading.Tasks;
using Game.Fluids;
using Game.Interactions;
using UnityEngine;

namespace Game.Buildings {
	public class Pylon: MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private float _animationDuration;
		[SerializeField] private Fluid _targetFluid;
		[SerializeField] private Vector3 _basePosition;
		[SerializeField] private Vector3 _activePosition;
		[Header("Components")]
		[SerializeField] private FluidContainer _container;
		[SerializeField] private Transform _sphere;
		[SerializeField] private FluidContainerInteraction _interaction;

		public bool Active => _container.Fluid == _targetFluid;
		
		private CancellationTokenSource _animation;
		private bool _isActive;

		private void Awake() {
			_sphere.localPosition = _basePosition;
			OnFluidChanged();
		}

		public void Lock() => _interaction.gameObject.SetActive(false);
		
		private void OnEnable() {
			_container.FluidChanged += OnFluidChanged;
		}
		private void OnDisable() {
			_container.FluidChanged -= OnFluidChanged;
		}
		private void OnFluidChanged() {
			var active = _container.Fluid == _targetFluid;
			if (active != _isActive) {
				_isActive = active;
				
				_animation?.Cancel();
				_animation = new CancellationTokenSource();
				Animate(_isActive ? _activePosition : _basePosition, _animation.Token).Forget();
			}
		}

		private async Task Animate(Vector3 targetPosition, CancellationToken token) {
			var t = 0f;
			var startPosition = _sphere.localPosition;
			while (t < _animationDuration) {
				t += Time.deltaTime;
				_sphere.localPosition = Vector3.Lerp(startPosition, targetPosition, t / _animationDuration);
				await Awaitable.NextFrameAsync(token);
			}
			_sphere.localPosition = targetPosition;
		}
	}
}