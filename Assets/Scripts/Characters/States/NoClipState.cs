using UnityEngine;

namespace Game.Characters.States {
	public class NoClipState: AbstractState {
		public override void Update() {
			var input = Character.GetRelatedMove();
			Character.Controller.transform.position += new Vector3(input.x, 0, input.y) * (Time.deltaTime * 10);
		}
		public override void OnEnter() {
			Character.Controller.enabled = false;
		}
		public override void OnExit() {
			Character.Controller.enabled = true;
		}
	}
}