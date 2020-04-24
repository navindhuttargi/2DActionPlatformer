using ActionPlatformer.Gameplay;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace ActionPlatformer
{
	public class GameplayStates : State
	{
		protected GameplayManager _gameplayManager;
		public GameplayStates(GameplayManager gameplayManager, uint _id) : base(_id)
		{
			_gameplayManager = gameplayManager;
		}
		public virtual bool OnEventFromPL(GameEvent gameEvent) { return true; }
	}
	public class GameplayStates_INITIALIZE : GameplayStates
	{
		//CHEACK THE GAME TYPE
		public GameplayStates_INITIALIZE(GameplayManager gameplayManager, uint _id) : base(gameplayManager, _id) { }
		public override void OnEnter()
		{
			base.OnEnter();
			_gameplayManager.OnInitialize();
			_gameplayManager.InfoText.text = UI_Messages.WAITING_FOR_PLAYERS;

			Hashtable props = new Hashtable
			{
				{PlatformersGame.PLAYER_LOADED_LEVEL, true}
			};
			PhotonNetwork.LocalPlayer.SetCustomProperties(props);
			_gameplayManager.OnStateCompleted(this, (uint)Enums.GAMEPLAY_EVENT.START);
		}


		public override bool OnEventFromPL(GameEvent gameEvent)
		{

			return base.OnEventFromPL(gameEvent);
		}
	}
	public class GameplayState_START_GAME : GameplayStates
	{
		public GameplayState_START_GAME(GameplayManager gameplayManager, uint _id) : base(gameplayManager, _id)
		{
		}

		public override void OnEnter()
		{
			Debug.Log("<color=green>GAME STATE:" + this.GetType() + "</color>");
		}

		public override bool OnEventFromPL(GameEvent gameEvent)
		{
			if (gameEvent.gamePlayEvent == Enums.GAMEPLAY_EVENT.TIME_EXPIRED)
			{
				gameEvent.eventType = Enums.EVENT_TYPE.SCENE_EVENT;
				_gameplayManager.AddEventsToPL(gameEvent);
				return true;
			}
			if (gameEvent.gamePlayEvent == Enums.GAMEPLAY_EVENT.PLAYER_RESPAWN)
			{
				gameEvent.eventType = Enums.EVENT_TYPE.SCENE_EVENT;
				_gameplayManager.AddEventsToPL(gameEvent);
			}
			return base.OnEventFromPL(gameEvent);
		}
	}
	public class GameplayState_GAMEOVER : GameplayStates
	{
		public GameplayState_GAMEOVER(GameplayManager gameplayManager, uint _id) : base(gameplayManager, _id)
		{
		}
	}
}
