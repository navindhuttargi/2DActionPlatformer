using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPlatformer
{
	public class PlatformerChat : MonoBehaviour, IChatClientListener
	{
		public ChatClient chatClient;
		public Text messageText;
		public Button sendMessageButton;
		public InputField messageField;
		public GameObject TextChatPanel;
		public List<Button> buttons;

		private AppSettings chatAppSettings;
		#region UNITY_CORE_FUNCTIONS
		void Start()
		{
			chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
			this.chatClient = new ChatClient(this);
			this.chatClient.UseBackgroundWorkerForSending = true;
			this.chatClient.Connect(this.chatAppSettings.AppIdChat, Application.version, new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
			sendMessageButton.onClick.AddListener(() => { SendMessages(); });
			foreach (Button b in buttons)
				b.onClick.AddListener(() => { SendMessages(b.GetComponentInChildren<Text>().text); });
		}

		// Update is called once per frame
		void Update()
		{
			if (this.chatClient != null)
				this.chatClient.Service();
		}
		private void OnDisable()
		{
			if (this.chatClient != null)
				this.chatClient.Disconnect();
		}
		private void OnDestroy()
		{
			if (this.chatClient != null)
				this.chatClient.Disconnect();
		}
		private void OnApplicationQuit()
		{
			chatClient.Disconnect();
		}
		#endregion
		#region PHOTON_CALLBACKS_METHODS
		public void DebugReturn(DebugLevel level, string message)
		{
			if (level == DebugLevel.ERROR)
			{
				Debug.LogError(message);
			}
			else if (level == DebugLevel.WARNING)
			{
				Debug.LogWarning(message);
			}
			else
			{
				Debug.Log(message);
			}
		}

		public void OnChatStateChange(ChatState state)
		{
			Debug.Log(state);
		}

		public void OnConnected()
		{
			this.chatClient.Subscribe(PhotonNetwork.CurrentRoom.Name);
			this.chatClient.SetOnlineStatus(ChatUserStatus.Online);
		}

		public void OnDisconnected()
		{
			Debug.Log("Disconnected");
		}

		public void OnGetMessages(string channelName, string[] senders, object[] messages)
		{
			if (channelName.Equals(PhotonNetwork.CurrentRoom.Name))
			{
				this.ShowMessage(channelName);
			}
		}

		public void OnPrivateMessage(string sender, object message, string channelName)
		{

		}

		public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
		{
			throw new System.NotImplementedException();
		}

		public void OnSubscribed(string[] channels, bool[] results)
		{
			Debug.Log("OnSubscribed:" + string.Join(",", channels));
			this.chatClient.PublishMessage(channels[0], "I am connected");
		}

		public void OnUnsubscribed(string[] channels)
		{
			throw new System.NotImplementedException();
		}

		public void OnUserSubscribed(string channel, string user)
		{
			throw new System.NotImplementedException();
		}
		public void OnUserUnsubscribed(string channel, string user)
		{
			throw new System.NotImplementedException();
		}
		#endregion
		private void SendMessages(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
				this.chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, message);
			else if (string.IsNullOrEmpty(messageField.text))
				return;
			else
				this.chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, messageField.text);
		}
		private void ShowMessage(string channelName)
		{
			if (string.IsNullOrEmpty(channelName))
				return;

			ChatChannel channel = null;
			bool found = this.chatClient.TryGetChannel(channelName, out channel);
			if (!found)
			{
				Debug.Log("Failed to find the channel:" + channelName);
				return;
			}
			messageText.text = channel.ToStringMessages();
			StartCoroutine(DisplayMessage());
		}
		IEnumerator DisplayMessage()
		{
			float time = 2;
			RectTransform r;

			CanvasGroup cg = TextChatPanel.GetComponent<CanvasGroup>();
			cg.blocksRaycasts = true;
			cg.interactable = true;
			cg.alpha = 1;
			while (time > 0)
			{
				time -= Time.deltaTime;
				yield return 0;
			}
			cg.interactable = false;
			cg.blocksRaycasts = false;
			cg.alpha = 0;
		}
	}
}
