using UnityEngine;
namespace ActionPlatformer
{
	public class GameManagerState : State
	{
		protected GameManager _myParent = null;

		public GameManagerState(GameManager myParent, uint id) : base(id)
		{
			_myParent = myParent;
		}
		public virtual bool OnEventFromPL(GameEvent gameEvent) { return true; }
	}
	public class GameManagerState_INITIALIZE : GameManagerState
	{
		public GameManagerState_INITIALIZE(GameManager myParent, uint id) : base(myParent, id)
		{
		}
		public override void OnEnter()
		{
			base.OnEnter();
			Debug.Log("<color=green>GAME STATE:" + this.GetType() + "</color>");
			_myParent.OnInitialize();
			_myParent.OnStateCompleted(this, (uint)Enums.GameStates.LOGGING_IN_STATE);
		}
	}
	public class GameManagerState_LOGIN : GameManagerState
	{
		public GameManagerState_LOGIN(GameManager myParent, uint id) : base(myParent, id) { }
		public override void OnEnter()
		{
			base.OnEnter();
			Debug.Log("<color=green>GAME STATE:" + this.GetType() + "</color>");
		}
		public override bool OnEventFromPL(GameEvent gameEvent)
		{
			if (gameEvent.uiEvent == Enums.ROOM_EVENT.LOGGED_IN)
			{
				gameEvent.eventType = Enums.EVENT_TYPE.UI_EVENT;
				_myParent.AddEventsToPL(gameEvent);
				_myParent.OnStateCompleted(this, (uint)Enums.GameStates.SELECTION_STATE);
				return true;
			}
			return base.OnEventFromPL(gameEvent);
		}
	}
	public class GameManagerState_SELECTION : GameManagerState
	{
		public GameManagerState_SELECTION(GameManager myParent, uint id) : base(myParent, id)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			Debug.Log("<color=green>GAME STATE:" + this.GetType() + "</color>");
		}

		public override bool OnEventFromPL(GameEvent gameEvent)
		{

			gameEvent.eventType = Enums.EVENT_TYPE.UI_EVENT;

			switch (gameEvent.uiEvent)
			{
				case Enums.ROOM_EVENT.ON_JOINED_ROOM:
					_myParent.AddEventsToPL(gameEvent);
					break;
				case Enums.ROOM_EVENT.ON_JOIN_ROOM_FAILED:
					_myParent.AddEventsToPL(gameEvent);
					break;
				case Enums.ROOM_EVENT.ON_LEFT_ROOM:
					_myParent.AddEventsToPL(gameEvent);
					break;
				case Enums.ROOM_EVENT.ON_CREATE_ROOM_FAILED:
					_myParent.AddEventsToPL(gameEvent);
					break;
				case Enums.ROOM_EVENT.ON_ROOM_LIST_UPDATE:
					break;
				case Enums.ROOM_EVENT.IS_PLAYER_READY:
					_myParent.AddEventsToPL(gameEvent);
					break;
			}
			return base.OnEventFromPL(gameEvent);
		}
	}
	public class GameManagerState_START_GAME : GameManagerState
	{
		public GameManagerState_START_GAME(GameManager myParent, uint id) : base(myParent, id)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			Debug.Log(this.GetType());
		}

		public override bool OnEventFromPL(GameEvent gameEvent)
		{
			if (gameEvent.uiEvent == Enums.ROOM_EVENT.ON_LEFT_LOBBY)
			{
				_myParent.OnStateCompleted(this, (uint)Enums.GameStates.GAME_OVER_STATE);
				return true;
			}

			return base.OnEventFromPL(gameEvent);
		}
	}
	public class GameManagerState_GAME_OVER : GameManagerState
	{
		public GameManagerState_GAME_OVER(GameManager myParent, uint id) : base(myParent, id)
		{
		}
		public override void OnEnter()
		{
			base.OnEnter();
			Debug.Log(this.GetType());
		}

		public override bool OnEventFromPL(GameEvent gameEvent)
		{
			if (gameEvent.uiEvent == Enums.ROOM_EVENT.ON_CREATE_ROOM_FAILED)
			{

				//((GameManager)_myParent).OnStateCompleted(this, (uint)Enums.GameStates.GAME_OVER_STATE);
				return true;
			}

			return base.OnEventFromPL(gameEvent);
		}
	}
}
