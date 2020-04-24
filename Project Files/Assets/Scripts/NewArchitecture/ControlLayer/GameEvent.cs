namespace ActionPlatformer
{
	public class GameEvent
	{
		public Enums.ROOM_EVENT uiEvent;
		public Enums.GAMEPLAY_EVENT gamePlayEvent;
		public Enums.EVENT_TYPE eventType;
		public GameEvent(Enums.ROOM_EVENT eventTyp)
		{
			uiEvent = eventTyp;
		}
		public GameEvent(Enums.GAMEPLAY_EVENT gAMEPLAY_EVENT)
		{
			gamePlayEvent = gAMEPLAY_EVENT;
		}
	}
}
