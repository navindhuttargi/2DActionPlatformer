using System;
using UnityEngine;
namespace ActionPlatformer
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CanvasGroup))]
	public class UI_Screen : MonoBehaviour
	{
		[Header("Screen Events")]
		public Action OnScreenStart = null;
		public Action OnScreenClose = null;

		private Animator animator;
		private void Awake()
		{
			animator = GetComponent<Animator>();
		}
		public virtual void StartScreen()
		{
			if (OnScreenStart != null)
				OnScreenStart.Invoke();
			HandleAnimation("fadeIn");
		}
		public virtual void CloseScreen()
		{
			if (OnScreenClose != null)
				OnScreenClose.Invoke();
			HandleAnimation("fadeOut");
		}
		private void HandleAnimation(string animationKey)
		{
			if (animator)
				animator.SetTrigger(animationKey);
			else
				Debug.LogError("don't have animator");
		}
	}
}
