using ActionPlatformer.Gameplay;
using Photon.Pun;
using UnityEngine;
namespace ActionPlatformer
{
	public class Gun : MonoBehaviourPun
	{
		public Rigidbody2D rocket;             // Prefab of the rocket.
		public float speed = 20f;               // The speed the rocket will fire at.
		public GameObject rocketSource;

		private PlayerControl playerCtrl;       // Reference to the PlayerControl script.
		private Animator anim;                  // Reference to the Animator component.

		public new PhotonView photonView;

		void Awake()
		{
			// Setting up the references.
			playerCtrl = GetComponent<PlayerControl>();
			rocketSource = transform.GetChild(transform.childCount - 2).gameObject;
			photonView = GetComponentInParent<PhotonView>();
			anim = GetComponent<Animator>();
		}
		private void Start()
		{

		}
		[PunRPC]
		public void BulletFire()
		{
			//	print("Fired");
			// ... set the animator Shoot trigger parameter and play the audioclip.
			anim.SetTrigger("Shoot");
			GetComponent<AudioSource>().Play();

			// If the player is facing right...
			if (transform.localScale.x > 0)
			{
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				Rigidbody2D bulletInstance = Instantiate(rocket, rocketSource.transform.position, Quaternion.Euler(new Vector3(0, 0, 0f))) as Rigidbody2D;
				bulletInstance.GetComponent<BulletFire>().InitializeBullet(photonView.Owner);
				bulletInstance.velocity = new Vector2(speed, 0);
			}
			else
			{
				// Otherwise instantiate the rocket facing left and set it's velocity to the left.
				Rigidbody2D bulletInstance = Instantiate(rocket, rocketSource.transform.position, Quaternion.Euler(new Vector3(0, 0, 180f))) as Rigidbody2D;
				bulletInstance.GetComponent<BulletFire>().InitializeBullet(photonView.Owner);
				bulletInstance.velocity = new Vector2(-speed, 0);
			}
		}
	}
}
