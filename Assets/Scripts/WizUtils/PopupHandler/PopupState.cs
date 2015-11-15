namespace WizUtils.UI {

	/// <summary>
	/// Return value from Events to indicate what state the popup was in when the event was fired
	/// </summary>
	public enum PopupState {
		/// <summary>
		/// Default Value
		/// </summary>
		Null,
		
		/// <summary>
		/// Popup Canceled
		/// </summary>
		Cancel,
		
		/// <summary>
		/// Popup Accepted
		/// </summary>
		Accept,
		
		/// <summary>
		/// Popup Accepted, data set
		/// </summary>
		AcceptWithData
	}
}