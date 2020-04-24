using ActionPlatformer.UI;
using UnityEngine;
namespace ActionPlatformer
{
	public class GameManager : StateMachine<GameManager>, IGameManager
	{
		public const uint INVALID_STATE = 999;

		//private IControllerFactory _controllerFactory;
		//public IControllerFactory controllerFactory
		//{
		//	get
		//	{
		//		return _controllerFactory;
		//	}
		//}
		//[Inject]
		//public void Construct(IControllerFactory controllerFactory)
		//{
		//	_controllerFactory = controllerFactory;
		//}
		#region UNITY_CORE_METHODS
		protected override void Start()
		{
			base.Start();
		}
		protected override void Update()
		{
			base.Update();
		}
		#endregion
		#region	STATE_MACHINE_EVENTS_AND_METHODS
		protected override void RegisterStates()
		{
			base.RegisterStates();
			AddState(new GameManagerState_INITIALIZE(this, (uint)Enums.GameStates.INITIALIZE_STATE));
			AddState(new GameManagerState_LOGIN(this, (uint)Enums.GameStates.LOGGING_IN_STATE));
			AddState(new GameManagerState_SELECTION(this, (uint)Enums.GameStates.SELECTION_STATE));
			AddState(new GameManagerState_START_GAME(this, (uint)Enums.GameStates.START_STATE));
			AddState(new GameManagerState_GAME_OVER(this, (uint)Enums.GameStates.GAME_OVER_STATE));
			currentState = INVALID_STATE;
		}

		public override void OnInitialize(IGameManager gameManager = null)
		{

			//for loadng Main Menu canvas from assetbundle
			//string filePath = Path.Combine(Application.streamingAssetsPath, "ui_assetbundle.maincanvas");
			//AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
			//GameObject g = Instantiate(bundle.LoadAsset("MainCanvas")) as GameObject;
			//GameObject g = Resources.Load("UIPrefabs/MainCanvas") as GameObject;

			Instantiate(Resources.Load("MainCanvas") as GameObject);
			GameObject connection = new GameObject("ConnectionManager");
			connection.AddComponent<ConnectionManager>();
			gameObject.AddComponent<SceneHandler>();
			gameObject.AddComponent<Photon.Pun.UtilityScripts.PlayerNumbering>().dontDestroyOnLoad = true;

			//ISceneHandler sceneHandler = controllerFactory.GetInstance<ISceneHandler>();
			//controllerFactory.GetInstance<IGameUI>().OnInitialize();
			base.OnInitialize(this);
			controllerFactory.GetInstance<IConnectionManager>().OnInitialize();
			//gameController = controllerFactory.GetInstance<IGameController>();
			//gameController.OnInitialize(this);
		}
		public void OnEventFromPL(GameEvent gameEvent)
		{
			if (managerCurrentState != null)
			{
				if (managerCurrentState is State)
				{
					((GameManagerState)managerCurrentState).OnEventFromPL(gameEvent);
				}
			}
		}
		public override void OnStateCompleted(State state, uint targetState = 999)
		{
			bool isHandled = false;
			switch ((Enums.GameStates)state.ID)
			{
				case Enums.GameStates.INITIALIZE_STATE:
					goalState = targetState;
					isHandled = true;
					break;
				case Enums.GameStates.LOGGING_IN_STATE:
					goalState = targetState;
					isHandled = true;
					break;
				case Enums.GameStates.SELECTION_STATE:
					goalState = targetState;
					isHandled = true;
					break;
				case Enums.GameStates.START_STATE:
					goalState = targetState;
					isHandled = true;
					break;
				case Enums.GameStates.GAME_OVER_STATE:
					goalState = targetState;
					isHandled = true;
					break;
			}
			if (!isHandled && targetState != INVALID_STATE)
				OnStateCompleted(state, targetState);
		}
		#endregion
	}
}
