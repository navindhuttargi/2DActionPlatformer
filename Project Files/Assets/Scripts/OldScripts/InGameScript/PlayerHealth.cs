using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPlatformer.Gameplay
{
	public class PlayerHealth : MonoBehaviour
	{
		public float health = 100f;                 // The player's health.
		public AudioClip[] ouchClips;               // Array of clips to play when the player is damaged.
		public float hurtForce = 1f;               // The force with which the player is pushed when hurt.

		private PlayerControl playerControl;        // Reference to the PlayerControl script.
		private Animator anim;                      // Reference to the Animator on the player
		public Image fillImage;
		public Slider healthSlider;
		public Gradient healthColor;
		[SerializeField] PhotonView photonView;
		void Awake()
		{
			// Setting up references.
			playerControl = GetComponent<PlayerControl>();
			anim = GetComponent<Animator>();
			photonView = GetComponent<PhotonView>();
			// Getting the intial scale of the healthbar (whilst the player has full health).
		}
		public void SetMaxHealth(float maxHealthValue)
		{
			healthSlider.maxValue = maxHealthValue;
			health = maxHealthValue;
			healthSlider.value = maxHealthValue;
			fillImage.color = healthColor.Evaluate(healthSlider.normalizedValue);
		}
		public bool SetHealth(float damage, Transform transform)
		{
			if (health == 0)
			{
				return false;
			}
			TakeDamage(transform);
			health -= damage;
			health = Mathf.Clamp(health, healthSlider.minValue, healthSlider.maxValue);
			healthSlider.value = health;
			fillImage.color = healthColor.Evaluate(healthSlider.normalizedValue);
			if (health == 0 && photonView.IsMine)
			{
				SetMaxHealth(100);
				photonView.RPC("ResetPlayer", RpcTarget.All);
			}
			else if (health == 0)
			{
				SetMaxHealth(100);
				return true;
			}
			return false;
		}


		void TakeDamage(Transform enemy)
		{
			// Make sure the player can't jump.
			playerControl.jump = false;

			// Create a vector that's from the enemy to the player with an upwards boost.
			Vector3 hurtVector = transform.position - enemy.position + Vector3.up * 5f;

			// Add a force to the player in the direction of the vector and multiply by the hurtForce.
			GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce);

			// Play a random clip of the player getting hurt.
			int i = Random.Range(0, ouchClips.Length);
			AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);
		}
		[PunRPC]
		void ResetPlayer()
		{
			if (photonView.IsMine)
			{
				StartCoroutine(FindObjectOfType<PlayerScoreboard>().DisplayScore());
			}
		}
		//IEnumerator Wait()
		//{
		//	float timer = 4;
		//	while (timer > 0)
		//	{
		//		timer -= Time.deltaTime;
		//		yield return 0;
		//	}
		//	GameplayManager.Instance.controllerFactory.GetInstance<IGameController>().OnEventTOGameManager(new GameEvent(Enums.GAMEPLAY_EVENT.PLAYER_RESPAWN));
		//}
	}
}
