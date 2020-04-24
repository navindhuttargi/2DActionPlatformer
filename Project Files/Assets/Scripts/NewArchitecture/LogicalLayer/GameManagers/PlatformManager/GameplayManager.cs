using UnityEngine.UI;

namespace ActionPlatformer.Gameplay
{
	public class GameplayManager : StateMachine<GameplayManager>, IGameManager
	{
		public const uint INVALID_STATE = 999;
		public Text InfoText;

		#region UNITY_CORE_FUNCTIONS
		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
		}
		#endregion
		public override void OnInitialize(IGameManager gameManager = null)
		{
			//controllerFactory.GetInstance<IGameUI>().OnInitialize();
			//controllerFactory.GetInstance<ISceneHandler>().OnInitialize();
			//gameController = controllerFactory.GetInstance<IGameController>();
			//gameController.OnInitialize(this);
			base.OnInitialize(this);

		}
		protected override void RegisterStates()
		{
			base.RegisterStates();
			AddState(new GameplayStates_INITIALIZE(this, (uint)Enums.GAMEPLAY_EVENT.INITIALIZE));
			AddState(new GameplayState_START_GAME(this, (uint)Enums.GAMEPLAY_EVENT.START));
			AddState(new GameplayState_GAMEOVER(this, (uint)Enums.GAMEPLAY_EVENT.GAMEOVER));
			currentState = INVALID_STATE;
		}
		public void OnEventFromPL(GameEvent gameEvent)
		{
			if (managerCurrentState != null)
			{
				if (managerCurrentState is State)
				{
					((GameplayStates)managerCurrentState).OnEventFromPL(gameEvent);
				}
			}
		}
		public override void OnStateCompleted(State state, uint targetState = 999)
		{
			bool isHandled = false;
			switch ((Enums.GAMEPLAY_EVENT)state.ID)
			{
				case Enums.GAMEPLAY_EVENT.INITIALIZE:
					goalState = targetState;
					isHandled = true;
					break;
				case Enums.GAMEPLAY_EVENT.START:
					goalState = targetState;
					isHandled = true;
					break;
				case Enums.GAMEPLAY_EVENT.GAMEOVER:
					goalState = targetState;
					isHandled = true;
					break;
			}
			if (!isHandled && targetState != INVALID_STATE)
				base.OnStateCompleted(state, targetState);
		}
	}
}
