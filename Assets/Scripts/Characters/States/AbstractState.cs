using System;

namespace Game.Characters.States {
	[Serializable]
	public abstract class AbstractState: ICharacterState {
		protected CharacterStateMachine Character { get; private set; }

		public void Init(CharacterStateMachine stateMachine) {
			Character = stateMachine;
		}
		public virtual void Update() { }
		public virtual void FixedUpdate() {}
		public virtual void OnEnter() { }
		public virtual void OnExit() {}
	}
}