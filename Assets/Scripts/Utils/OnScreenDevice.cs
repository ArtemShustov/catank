#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace Game.Utils {
	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	[Preserve]
	[InputControlLayout(displayName = "On-Screen Virtual Device", stateType = typeof(OnScreenDeviceState))]
	public class OnScreenDevice: InputDevice {
		public ButtonControl Jump { get; private set; }
		public ButtonControl Interact { get; private set; }
		
		public Vector2Control Move { get; private set; }
		public Vector2Control Look { get; private set; }


		static OnScreenDevice() {
			InputSystem.RegisterLayout<OnScreenDevice>();
			Debug.Log("On-Screen Virtual Device has been registered");
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Initialize() {}
		
		protected override void FinishSetup() {
			base.FinishSetup();

			Jump = GetChildControl<ButtonControl>("Jump");
			Interact = GetChildControl<ButtonControl>("Interact");
			
			Move = GetChildControl<Vector2Control>("Move");
			Look = GetChildControl<Vector2Control>("Look");
		}
	}
	[Preserve]
	public struct OnScreenDeviceState: IInputStateTypeInfo {
		public FourCC format => new FourCC('S', 'C', 'R', 'N');

		[InputControl(name = "Jump", layout = "Button", bit = 0)]
		[InputControl(name = "Interact", layout = "Button", bit = 1)]
		public ushort Buttons;

		[InputControl(name = "Move", layout = "Vector2")]
		public Vector2 Move;
		[InputControl(name = "Look", layout = "Vector2")]
		public Vector2 Look;
	}
}