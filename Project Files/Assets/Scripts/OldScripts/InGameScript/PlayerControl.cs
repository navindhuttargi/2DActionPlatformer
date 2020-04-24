using ActionPlatformer.UI;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace ActionPlatformer.Gameplay
{
	public class PlayerControl : MonoBehaviourPun
	{
		[HideInInspector]
		public bool facingRight = true;         // For determining which way the player is currently facing.
		[HideInInspector]
		public bool jump = false;               // Condition for whether the player should jump.

		[Header("for Player movement")]
		public Button FireButton;
		public float moveForce = 365f;          // Amount of force added to move the player left and right.
		public float maxSpeed = 5f;             // The fastest the player can travel in the x axis.
		public AudioClip[] jumpClips;           // Array of clips for when the player jumps.
		public float jumpForce = 1000f;         // Amount of force added when the player jumps.
		public AudioClip[] taunts;              // Array of clips for when the player taunts.
		public float tauntProbability = 50f;    // Chance of a taunt happening.
		public float tauntDelay = 1f;           // Delay for when the taunt should happen.

		[Header("For Bullet")]
		public GameObject BulletPrefab;
		public Transform bulletTrasnform;
		public Gun gun;
		public PlayerHealth playerHealth;

		public GameplayManager gameManager;

		private int tauntIndex;                 // The index of the taunts array indicating the most recent taunt.
		private Transform groundCheck;          // A position marking where to check if the player is grounded.
		private bool grounded = false;          // Whether or not the player is grounded.
		private Animator anim;                  // Reference to the player's animator component.

		private new PhotonView photonView;

		[Header("for player physics")]
		private new Collider collider;
		private new Renderer renderer;

		private bool controllable = true;

		IGameUI uimanager;

		IGameController gameController;


		void Awake()
		{
			// Setting up references.
			uimanager = GameplayManager.Instance.controllerFactory.GetInstance<IGameUI>();

			gameController = GameplayManager.Instance.controllerFactory.GetInstance<IGameController>();

			FireButton = ((GameplayUIManager)uimanager).uiScreens["JoystickPanel"].transform.Find("fire").GetComponent<Button>();

			/*GameplayManager.Instance.*/
			FireButton.onClick.AddListener(() => BulletFire());

			groundCheck = transform.Find("groundCheck");

			anim = GetComponent<Animator>();

			photonView = GetComponent<PhotonView>();

			renderer = GetComponent<Renderer>();

			photonView = GetComponent<PhotonView>();

			gun = GetComponent<Gun>();

			playerHealth = GetComponentInChildren<PlayerHealth>();

			playerHealth.SetMaxHealth(100);
		}

		public void Update()
		{
			if (!photonView.IsMine || !controllable)
			{
				return;
			}
			// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
			grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

			// If the jump button is pressed and the player is grounded then the player should jump.
			if (Input.GetButtonDown("Jump") && grounded)
				jump = true;

			// If the fire button is pressed...
			if (Input.GetButtonDown("Fire1"))
			{
				//photonView.RPC("GunFire", RpcTarget.AllViaServer);
			}
		}

		public void BulletFire()
		{
			if (!photonView.IsMine || !controllable)
			{
				return;
			}
			if (!photonView.IsMine)
			{
				return;
			}

			if (!controllable)
			{
				return;
			}
			photonView.RPC("Fire111", RpcTarget.AllViaServer, bulletTrasnform.position, bulletTrasnform.rotation);
		}

		[PunRPC]
		public void Fire111(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
		{
			gun.BulletFire();


			#region not in use
			//anim.SetTrigger("Shoot");
			/** Use this if you want to fire one bullet at a time **/
			/*	if (facingRight)
				{
					bulletInstance = Instantiate(BulletPrefab, position, Quaternion.Euler(new Vector3(0, 0, 0))) ;
					bulletInstance.GetComponent<BulletFire>().InitializeBullet(photonView.Owner, (rotation * Vector3.right));
				}
				else
				{
					bulletInstance = Instantiate(BulletPrefab, position, Quaternion.Euler(new Vector3(0, 0, 180)));
					bulletInstance.GetComponent<BulletFire>().InitializeBullet(photonView.Owner, (rotation * Vector3.left));
				}
			*/


			/** Use this if you want to fire two bullets at once **/
			//Vector3 baseX = rotation * Vector3.right;
			//Vector3 baseZ = rotation * Vector3.forward;

			//Vector3 offsetLeft = -1.5f * baseX - 0.5f * baseZ;
			//Vector3 offsetRight = 1.5f * baseX - 0.5f * baseZ;

			//bullet = Instantiate(BulletPrefab, rigidbody.position + offsetLeft, Quaternion.identity) as GameObject;
			//bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
			//bullet = Instantiate(BulletPrefab, rigidbody.position + offsetRight, Quaternion.identity) as GameObject;
			//bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
			#endregion
		}

		public void FixedUpdate()
		{

			if (!photonView.IsMine)
			{
				return;
			}

			if (!controllable)
			{
				return;
			}

			// Cache the horizontal input.
			float h = Input.GetAxis("Horizontal");

			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat("Speed", Mathf.Abs(h));

			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			if (h * GetComponent<Rigidbody2D>().velocity.x < maxSpeed)
				// ... add a force to the player.
				GetComponent<Rigidbody2D>().AddForce(Vector2.right * h * moveForce);

			// If the player's horizontal velocity is greater than the maxSpeed...
			if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxSpeed)
				// ... set the player's velocity to the maxSpeed in the x axis.
				GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

			// If the input is moving the player right and the player is facing left...
			if (h > 0 && !facingRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (h < 0 && facingRight)
				// ... flip the player.
				Flip();

			// If the player should jump...
			if (jump)
			{
				// Set the Jump animator trigger parameter.
				anim.SetTrigger("Jump");

				// Play a random jump audio clip.
				int i = Random.Range(0, jumpClips.Length);
				AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

				// Add a vertical force to the player.
				GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));

				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				jump = false;
			}
		}


		void Flip()
		{
			// Switch the way the player is labelled as facing.
			facingRight = !facingRight;

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}


		public IEnumerator Taunt()
		{
			// Check the random chance of taunting.
			float tauntChance = Random.Range(0f, 100f);
			if (tauntChance > tauntProbability)
			{
				// Wait for tauntDelay number of seconds.
				yield return new WaitForSeconds(tauntDelay);

				// If there is no clip currently playing.
				if (!GetComponent<AudioSource>().isPlaying)
				{
					// Choose a random, but different taunt.
					tauntIndex = TauntRandom();

					// Play the new taunt.
					GetComponent<AudioSource>().clip = taunts[tauntIndex];
					GetComponent<AudioSource>().Play();
				}
			}
		}


		int TauntRandom()
		{
			// Choose a random index of the taunts array.
			int i = Random.Range(0, taunts.Length);

			// If it's the same as the previous taunt...
			if (i == tauntIndex)
				// ... try another random taunt.
				return TauntRandom();
			else
				// Otherwise return this index.
				return i;
		}
	}
}
