namespace Game.Characters {
	public interface ICharacterState {
		void Init(CharacterStateMachine stateMachine);
		void Update();
		void FixedUpdate();
		void OnEnter();
		void OnExit();
	}
}