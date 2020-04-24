using ActionPlatformer.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ActionPlatformer.Gameplay
{
	public class GameplayUIManager : IGameUI
	{
		private Text InfoText;
		private Canvas canvas;
		public Dictionary<string, UI_Screen> uiScreens = new Dictionary<string, UI_Screen>();

		public void OnInitialize()
		{
			canvas = GameplayManager.FindObjectOfType<Canvas>();
			foreach (UI_Screen screen in canvas.GetComponentsInChildren<UI_Screen>())
			{
				uiScreens.Add(screen.name, screen);
			}
			InfoText = uiScreens["MessagePanel"].transform.Find("infoText").GetComponent<Text>();
		}
		public void ShowMessage(string message)
		{
			InfoText.text = message;
		}
		public void OnEventFromGameManager(GameEvent gameEvent)
		{

		}
	}
}
