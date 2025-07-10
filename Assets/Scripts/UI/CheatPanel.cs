using Game.Characters;
using Game.Characters.States;
using Game.Fluids;
using Game.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game.UI {
	public class CheatPanel: MonoBehaviour {
		[SerializeField] private InputAction _input = new InputAction("Button", InputActionType.Button);
		[SerializeField] private GameObject _fistButton;
		[SerializeField] private GameObject _container;
		[SerializeField] private Fluid _fuel;

		private void Awake() {
			_container.SetActive(false);
		}

		public void Show() {
			_container.SetActive(true);
			EventSystem.current.SetSelectedGameObject(_fistButton);
		}
		public void Hide() {
			_container.SetActive(false);
		}

		public void ResetPosition() {
			GameRoot.Instance.Player.transform.position = Vector3.zero;
		}
		public void ClearTank() {
			GameRoot.Instance.Player.FluidContainer.SetStored(0);
			GameRoot.Instance.Player.FluidContainer.SetFluid(null);
		}
		public void EnableInput() {
			GameRoot.Instance.Player.EnableInput();
		}
		public void FillRocket() {
			var rocket = GameObject.Find("Rocket")?.GetComponent<FluidContainer>();
			if (rocket) {
				rocket.SetFluid(_fuel);
				rocket.SetStored(rocket.Capacity);
			}
		}
		public void GiveFuel() {
			var player = GameRoot.Instance.Player.FluidContainer;
			player.SetFluid(_fuel);
			player.SetStored(player.Capacity);
		}
		public void ToggleNoClip() {
			var player = GameRoot.Instance.Player.GetComponent<CharacterStateMachine>();
			if (!player) {
				return;
			}
			if (player.CurrentState is NoClipState) {
				player.ChangeState<IdleState>();
			} else {
				var state = new NoClipState();
				state.Init(player);
				player.ChangeState(state);
			}
		}
		public void ToggleFPS() {
			var fpsCounter = GameObject.FindFirstObjectByType<FPSCounter>(FindObjectsInactive.Include);
			if (!fpsCounter) {
				return;
			}
			fpsCounter.gameObject.SetActive(!fpsCounter.gameObject.activeSelf);
		}
		public void ToggleQualityLevel() {
			var current = QualitySettings.GetQualityLevel();
			QualitySettings.SetQualityLevel(current == 0 ? 1 : 0);
		}

		private void OnEnable() {
			_input.Enable();
			_input.performed += OnInput;
		}
		private void OnDisable() {
			_input.Disable();
			_input.performed -= OnInput;
		}
		private void OnInput(InputAction.CallbackContext obj) {
			if (_container.activeSelf) {
				Hide();
			} else {
				Show();
			}
		}
	}
}