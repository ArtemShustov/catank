using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
#if UNITY_WEBGL || UNITY_EDITOR
using UnityEngine.InputSystem.WebGL;
#endif
using UnityEngine.InputSystem.XInput;

namespace Game.UI {
	[CreateAssetMenu(menuName = "UI/InputIcons")]
	public class InputIcons: ScriptableObject {
		[SerializeField] private TMP_SpriteAsset _atlas;
		
		public string Get(InputControl control) {
			if (_atlas == null) {
				return null;
			}
			
			var device = control.device switch {
				XInputController => "xbox_",
				DualShockGamepad => "playstation_",
				Keyboard => "keyboard_",
				Mouse => "keyboard_",
				#if UNITY_WEBGL || UNITY_EDITOR
				WebGLGamepad => "webgamepad_",
				#endif
				_ => string.Empty
			};
			var id = device + control.name;
			if (!_atlas.spriteCharacterTable.Exists(c => c.name.Equals(id))) {
				return null;
			}
			
			return $"<sprite name={id}>";
		}
	}
}