using ActionPlatformer.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
namespace ActionPlatformer
{
	public class PlayerEntryListM : MonoBehaviour
	{
		[Header("UI References")]
		public Text PlayerNameText;

		public Image PlayerColorImage;
		public Button PlayerReadyButton;
		public Image PlayerReadyImage;

		private int ownerId;
		private bool isPlayerReady;

		#region UNITY

		public void OnEnable()
		{
			PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
		}

		public void Start()
		{
			if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
			{
				PlayerReadyButton.gameObject.SetActive(false);
			}
			else
			{
				Hashtable initialProps = new Hashtable() { { PlatformersGame.PLAYER_READY, isPlayerReady }, { PlatformersGame.PLAYER_LIVES, PlatformersGame.PLAYER_MAX_LIVES } };
				PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
				PhotonNetwork.LocalPlayer.SetScore(0);

				PlayerReadyButton.onClick.AddListener(() => ReadyButton());
			}
		}
		public void ReadyButton()
		{
			{
				isPlayerReady = !isPlayerReady;
				SetPlayerReady(isPlayerReady);

				Hashtable props = new Hashtable() { { PlatformersGame.PLAYER_READY, isPlayerReady } };
				PhotonNetwork.LocalPlayer.SetCustomProperties(props);

				if (PhotonNetwork.IsMasterClient)
				{
					FindObjectOfType<ConnectionManager>().LocalPlayerPropertiesUpdated();
				}
			}
		}

		public void OnDisable()
		{
			PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
		}

		#endregion

		public void Initialize(int playerId, string playerName)
		{
			ownerId = playerId;
			PlayerNameText.text = playerName;
		}

		private void OnPlayerNumberingChanged()
		{
			foreach (Player p in PhotonNetwork.PlayerList)
			{
				if (p.ActorNumber == ownerId)
				{
					PlayerColorImage.color = PlatformersGame.GetColor(p.GetPlayerNumber());
				}
			}
		}

		public void SetPlayerReady(bool playerReady)
		{
			PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
			PlayerReadyImage.enabled = playerReady;
		}
	}
}
