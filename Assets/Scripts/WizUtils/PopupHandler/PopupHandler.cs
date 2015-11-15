// .Net includes
using System.Collections;

// Unity includes
using UnityEngine;

namespace WizUtils.UI {

	/// <summary>
	/// Class that can be used directly or as a base class for popups in the system.
	/// </summary>
	public class PopupHandler : MonoBehaviour {
		/// <summary>
		/// Open animation delay before popup is useable. In seconds
		/// </summary>
		public float openAnimationDelay = 0.0f;
		
		/// <summary>
		/// Delay before closing the popup. In seconds
		/// </summary>
		public float closeAnimationDelay = 0.3f;
		
		/// <summary>
		/// Current popup state
		/// </summary>
		private PopupState state = PopupState.Null;
		
		/// <summary>
		/// State Accessor
		/// </summary>
		public PopupState State {
			get { return state; }
			set { state = value; }
		}
		
		/// <summary>
		/// Delegate signature that is called when a popup is opened
		/// </summary>
		/// <param name="sender">popup that fired event</param>
		public delegate void PopupOpenEventHandler(object sender);
		
		/// <summary>
		/// Delegate signature that is called when a popup is closed
		/// </summary>
		/// <param name="sender">popup that fired event</param>
		/// <param name="state">current popup state</param>
		public delegate void PopupCloseEventHandler(object sender, PopupState state);
		
		/// <summary>
		/// Delegate signature that is called when a popup changes visiblity
		/// </summary>
		/// <param name="sender">popup that fired event</param>
		/// <param name="state">current popup state</param>
		public delegate void PopupVisibleEventHandler(object sender, PopupState state);
		
		/// <summary>
		/// Event Fired on open (using Open method)
		/// </summary>
		public event PopupOpenEventHandler OnOpen;
		
		/// <summary>
		/// Event Fired on closed (using Close method)
		/// </summary>
		public event PopupCloseEventHandler OnClose;
		
		/// <summary>
		/// Event fired when popup visibilty changed (using visible accessor)
		/// </summary>
		public event PopupVisibleEventHandler OnVisibleChanged;
		
		/// <summary>
		/// Visible Accessor
		/// </summary>
		public bool Visible {
			get {
				// Get whether the popup is active
				return gameObject.activeSelf;
			}
			set {
				// Set active / not active
				gameObject.SetActive(value);
				
				// Emit event for listeners
				if (OnVisibleChanged != null) {
					OnVisibleChanged(this, state);
				}
			}
		}
		
		/// <summary>
		/// To open popup (sets visible, not created)
		/// </summary>
		public virtual void Open() {
			// Activate the popup and then play animation if there is one
			// Note: ActivatePopup has to be outside of the Coroutine otherwise exception.
			ActivatePopup();
			
			// Play open animation
			StartCoroutine(PlayOpenAnimation());
		}
		
		/// <summary>
		/// To close popup (sets invisible, not destroyed)
		/// </summary>
		public virtual void Close() {
			StartCoroutine(CloseWithAnimation());
		}
		
		/// <summary>
		/// Play Open animation if there is one
		/// </summary>
		private IEnumerator PlayOpenAnimation() {
			// Get popup animator if exists
			Animator popupAnimator = this.GetComponent<Animator>();
			
			if (popupAnimator == null) {
				// No open animation on popup
				yield break;
			}
			
			// Attempt to trigger (this is caught above if no open trigger)
			popupAnimator.SetTrigger("open");
			
			// This could techincally be "yield return null", but if we want to delay user interaction we could implement this here in the future.
			yield return new WaitForSeconds(openAnimationDelay);
		}
		
		/// <summary>
		/// Close Popup with animation if there is one
		/// </summary>
		private IEnumerator CloseWithAnimation() {
			// Get popup animator if exists
			Animator popupAnimator = this.GetComponent<Animator>();
			
			if (popupAnimator == null) {
				// no close animation
				DeactivatePopup();
				yield break;
			}
			
			// Attempt to trigger (this is caught above if no close trigger)
			popupAnimator.SetTrigger("close");
			
			// wait for fadeOutDelay to be done.
			yield return new WaitForSeconds(closeAnimationDelay);
			DeactivatePopup();
		}
		
		/// <summary>
		/// Activate popup and emit event
		/// </summary>
		protected void ActivatePopup() {
			Visible = true;
			
			if (OnOpen != null) {
				OnOpen(this);
			}
		}
		
		/// <summary>
		/// Deactivate popup and emit event
		/// </summary>
		protected void DeactivatePopup() {
			Visible = false;
			
			if (OnClose != null) {
				OnClose(this, state);
			}
		}
	}
}