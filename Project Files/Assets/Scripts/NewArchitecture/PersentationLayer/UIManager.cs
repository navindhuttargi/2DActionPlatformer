using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ActionPlatformer.UI
{

	public interface IGameUI
	{
		void OnInitialize();
		void OnEventFromGameManager(GameEvent gameEvent);
	}
	/// <summary>
	/// handles the ui related part
	/// </summary>
	public class UIManager : IGameUI
	{
		#region PRIVATE_MEMBERS
		Dictionary<string, InputField> inputFields = new Dictionary<string, InputField>();
		List<Button> onClickBtn = new List<Button>();

		private UI_Screen currentScreen;
		private UI_Screen previousScreen;

		private Image fadePanel;

		private Canvas canvas;

		private Dictionary<string, UI_Screen> _uiScreen = new Dictionary<string, UI_Screen>();

		private Dictionary<string, Action> localEvents = new Dictionary<string, Action>();
		private Dictionary<Enums.ROOM_EVENT, Action> gameManagerEvents = new Dictionary<Enums.ROOM_EVENT, Action>();

		private IGameController gameController;
		private IConnectionManager connectionManager;
		#endregion
		public Dictionary<string, UI_Screen> uiScreens
		{
			get
			{
				return _uiScreen;
			}
		}
		#region INITIALIZE_UI_MANAGER
		public UIManager()
		{

		}
		public void OnInitialize()
		{
			//getting references of classes
			gameController = GameManager.Instance.controllerFactory.GetInstance<IGameController>();
			connectionManager = GameManager.Instance.controllerFactory.GetInstance<IConnectionManager>();

			//getting reference of ui elements
			canvas = MonoBehaviour.FindObjectOfType<Canvas>();
			foreach (UI_Screen screen in canvas.GetComponentsInChildren<UI_Screen>(true))
			{
				_uiScreen.Add(screen.name, screen);
				onClickBtn.AddRange(screen.GetComponentsInChildren<Button>());
				foreach (InputField inputField in screen.GetComponentsInChildren<InputField>(true))
					inputFields.Add(inputField.name, inputField);
			}
			foreach (Button item in onClickBtn)
				item.onClick.AddListener(() => OnClickEvent(item.name));

			inputFields["playerNameInputField"].text = "Player" + Random.Range(1, 1000);
			fadePanel = canvas.GetComponentInChildren<Image>();

			AddLocalEvents();
			AddGameManagerEvents();
			SwitchScreen(uiScreens["LoginPanel"]);
		}
		private void AddLocalEvents()
		{
			localEvents.Add(onClickBtn[0].name, () =>
			{
				onClickBtn[0].gameObject.SetActive(false);
				OnLoginButtonClicked();
			});
			localEvents.Add(onClickBtn[1].name, () =>
			{
				Debug.Log(onClickBtn[1].name);
				SwitchScreen(_uiScreen["CreateRoomPanel"]);
			});
			localEvents.Add(onClickBtn[2].name, () =>
			{
				connectionManager.OnJoinRandomRoom();
			});
			localEvents.Add(onClickBtn[3].name, () =>
			{
				SwitchScreen(_uiScreen["RoomListPanel"]);
			});
			localEvents.Add(onClickBtn[4].name, () =>
			{
				connectionManager.OnCreateRoom(inputFields["roomNameInputField"].text, inputFields["maxPlayersInputField"]);
			});
			localEvents.Add(onClickBtn[5].name, () =>
			{
				Debug.Log(onClickBtn[1].name);
				GoToPreviousScreen();
			});
			localEvents.Add(onClickBtn[6].name, () =>
			{
				SwitchScreen(_uiScreen["SelectionPanel"]);
			});
			localEvents.Add(onClickBtn[7].name, () =>
			{
				connectionManager.OnLeaveRoom();
			});
			localEvents.Add(onClickBtn[8].name, () =>
			{
				connectionManager.OnStartGame();
			});
		}
		private void AddGameManagerEvents()
		{
			gameManagerEvents.Add(Enums.ROOM_EVENT.LOGGED_IN, () =>
			{
				if (uiScreens.ContainsKey("SelectionPanel"))
					SwitchScreen(uiScreens["SelectionPanel"]);
			});
			gameManagerEvents.Add(Enums.ROOM_EVENT.ON_JOINED_ROOM, () =>
			{
				if (uiScreens.ContainsKey("InsideRoomPanel"))
					SwitchScreen(uiScreens["InsideRoomPanel"]);
			});
			gameManagerEvents.Add(Enums.ROOM_EVENT.ON_LEFT_ROOM, () =>
			{
				if (uiScreens.ContainsKey("SelectionPanel"))
					SwitchScreen(uiScreens["SelectionPanel"]);
			});
			gameManagerEvents.Add(Enums.ROOM_EVENT.IS_PLAYER_READY, () =>
			{
				onClickBtn[8].gameObject.SetActive(connectionManager.CheckPlayersReady());
			});
			gameManagerEvents.Add(Enums.ROOM_EVENT.ON_CREATE_ROOM_FAILED, () =>
			{
				if (uiScreens.ContainsKey("SelectionPanel"))
					SwitchScreen(uiScreens["SelectionPanel"]);
			});
			gameManagerEvents.Add(Enums.ROOM_EVENT.ON_JOIN_ROOM_FAILED, () =>
			{
				if (uiScreens.ContainsKey("SelectionPanel"))
					SwitchScreen(uiScreens["SelectionPanel"]);
			});
		}
		#endregion
		#region ON_CLICK_EVENTS
		private void OnClickEvent(string b)
		{
			if (localEvents.ContainsKey(b))
			{
				localEvents[b].Invoke();
			}
		}
		private void OnLoginButtonClicked()
		{

			if (inputFields["playerNameInputField"].text.Length > 0)
			{
				uiScreens["LoginPanel"].GetComponentInChildren<Text>().text = "";
				connectionManager.OnLogin(inputFields["playerNameInputField"].text);
			}
			else
			{

				Debug.LogError("Player Name is invalid.");
				GameManager.Instance.StartCoroutine(DisplayMessage(UI_Messages.NO_USER_NAME.ToCharArray(), uiScreens["LoginPanel"].GetComponentInChildren<Text>(), onClickBtn[0]));
			}
		}
		IEnumerator DisplayMessage(char[] message, Text messageText, Button button = null)
		{
			string _message = message.ToString();
			int index = 0;
			while (index != message.Length)
			{
				messageText.text += message[index];
				index++;
				yield return null;
			}
			if (button != null)
				button.gameObject.SetActive(true);
		}
		#endregion
		#region SWITCH_SCREEN_FUNCTIONS
		private void SwitchScreen(UI_Screen newScreen)
		{
			if (newScreen)
			{
				GameManager.Instance.StartCoroutine(FadingEffect());
				if (currentScreen)
				{
					currentScreen.CloseScreen();
					previousScreen = currentScreen;
				}
				currentScreen = newScreen;
				currentScreen.gameObject.SetActive(true);
				currentScreen.StartScreen();
			}
		}
		private void GoToPreviousScreen()
		{
			if (previousScreen)
			{
				SwitchScreen(previousScreen);
			}
		}
		IEnumerator<float> FadingEffect()
		{
			//FadeOut();
			Fade(.5f);
			float time = 1f;
			while (time > 0)
			{
				time -= Time.deltaTime;
				yield return 0;
			}
			Fade(0);
			//FadeIn();
		}
		private void Fade(float value)
		{
			if (fadePanel)
				fadePanel.CrossFadeAlpha(value, .5f, false);
			else
				Debug.LogError("Don't have fade panel");
		}
		#endregion
		#region CONNECTION_MANAGER_CALLBACKS 
		public void OnEventFromGameManager(GameEvent gameEvent)
		{
			/*
			switch (gameEvent.eventType)
			{
				case Enums.GAME_EVENT.LOGIN_IN:
					break;
				case Enums.GAME_EVENT.LOGGED_IN:
					SetActivePanel(selectionPanel.name);
					break;
				case Enums.GAME_EVENT.ON_ROOM_LIST_UPDATE:
					break;
				case Enums.GAME_EVENT.ON_LEFT_LOBBY:
					break;
				case Enums.GAME_EVENT.ON_CREATE_ROOM_FAILED:
					break;
				case Enums.GAME_EVENT.ON_JOIN_ROOM_FAILED:
					break;
				case Enums.GAME_EVENT.ON_JOIN_RANDOM_ROOM_FAILED:
					break;
				case Enums.GAME_EVENT.ON_JOINED_ROOM:
					break;
				case Enums.GAME_EVENT.ON_LEFT_ROOM:
					break;
				case Enums.GAME_EVENT.ON_PLAYER_ENTERED_ROOM:
					break;
				case Enums.GAME_EVENT.ON_MASTER_CLIENT_SWITCHED:
					break;
			}
			*/
			if (gameManagerEvents.ContainsKey(gameEvent.uiEvent))
			{
				gameManagerEvents[gameEvent.uiEvent]?.Invoke();
			}
		}
		#endregion
	}
}
