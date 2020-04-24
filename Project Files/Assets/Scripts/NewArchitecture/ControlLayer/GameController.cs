namespace ActionPlatformer.UI
{
	public interface IGameController
	{
		void OnEventFromGameManager(GameEvent gameEvent);
		void OnEventTOGameManager(GameEvent gameEvent);
		void OnInitialize(IGameManager gameManager);
	}
	public interface ISceneHandler
	{
		void OnInitialize();
		void OnEventFromGameManager(GameEvent gameEvent);
	}
	public class GameController : IGameController
	{
		private IGameUI uiManager;
		private ISceneHandler sceneHandler;
		private IGameManager gameManager;
		public GameController()
		{

		}
		public void OnInitialize(IGameManager gameManager)
		{
			this.gameManager = gameManager;
			uiManager = this.gameManager.controllerFactory.GetInstance<IGameUI>();
			sceneHandler = this.gameManager.controllerFactory.GetInstance<ISceneHandler>();
		}
		public void OnEventTOGameManager(GameEvent gameEvent)
		{
			if (gameManager != null)
			{
				gameManager.OnEventFromPL(gameEvent);
			}
		}
		public void OnEventFromGameManager(GameEvent gameEvent)
		{
			switch (gameEvent.eventType)
			{
				case Enums.EVENT_TYPE.SCENE_EVENT:
					if (sceneHandler != null)
						sceneHandler.OnEventFromGameManager(gameEvent);
					break;
				case Enums.EVENT_TYPE.UI_EVENT:
					if (uiManager != null)
						uiManager.OnEventFromGameManager(gameEvent);
					break;
			}
		}
	}
}
