# Popup Handler
The popup handler is a base class to provide popup functionality to any class that requires open and close functionality.  It can also be extended to use custom animations, and to pass state data.

## Dependencies
* UnityEngine
* WizUtils.UI
* WizUtils.UI.PopupState

## API
Open
Close
Visible
State
OnOpen
OnClose
ActivatePopup
DeactivatePopup
openAnimationDelay
closeAnimationDelay

## Usage
### 
The basic usage is to inherit the PopupHandler class and then override any functionality that you want to use.  Of course you can use the default settings however if you want to use custom animations they are supported.

```
public class MyPopup : PopupHandler {

}
```

By default you can directly use this if it is attached to a gameobject in your scene
```
public class MyFirstView {
	void Start() {
		MyPopup myPopup = popupGameObject.GetComponent<MyPopup>();
		myPopup.Open();
	}
}
```

The class that extends the PopupHandler should handle all the interaction of the popup, including button handles.  The Close function is provided by the PopupHandler.
```
public class MyPopup : PopupHandler {
	public void ButtonCloseHandler() {
		Close();
	}
}
```

### Events
The event system for the PopupHandler is tied into the functions that can be overriden, you can hook into these events by subscribing to them in your view.
```
public class MyFirstView {
	void Start() {
		MyPopup myPopup = popupGameObject.GetComponent<MyPopup>();
		
		myPopup.OnOpen += (object sender) => {

		};

		myPopup.OnClose += (object sender, WizUtils.UI.PopupState state) => {

		};

		myPopup.OnVisibleChanged += (object sender, WizUtils.UI.PopupState state) => {

		};
		
		// This will trigger the OnOpen event
		myPopup.Open();
	}
}
```

### Visible Settings
Using this you do not need to trigger the close events, but can still hide the popup if you want to stack them and have the close events chain.  Close and Visiblity settings have a State enum passed through, this can be used to control the flow of the popups when you are using a multi-popup system.

```
public class MyFirstView {
	void Start() {
		MyPopup myPopup = popupGameObject.GetComponent<MyPopup>();
	
		myPopup.OnVisibleChanged += (object sender, WizUtils.UI.PopupState state) => {

		};
		
		// This will trigger the OnOpen event
		myPopup.Open();
		
		myPopup.Visible = false;
	}
}
```

### Controlling Flow
When moving to and from a popup it's often necessary to set and retrieve data, the PopupHandler events facilitate this.  The below example shows how you can expose public getters and setters that can be accessed during the events.

```
public class UserData {
	public string Name { get; set; }
	public int Age { get; set; }
}

public class MyPopup : PopupHandler {
	public UserData Data { get; private set; }

	public void ButtonCloseHandler() {
		UserData data = new UserData();
		data.Name = "Bob";
		data.Age = 27;
		
		State = WizUtils.UI.PopupState.AcceptWithData;
		Close();
	}
}

public class MyFirstView {
	void Start() {
		MyPopup myPopup = popupGameObject.GetComponent<MyPopup>();

		myPopup.OnClose += (object sender, WizUtils.UI.PopupState state) => {
			if (state == WizUtils.UI.PopupState.AcceptWithData) {
				Debug.Log(myPopup.Data.Name);
				Debug.Log(myPopup.Data.Age);
			}
		};
		
		myPopup.Open();
	}
}
```

### Overriding Animations
There are two ways to implement popup animations for popups that use the PopupHandler, the first is to attach an animation controller to the popup at the level of the script.  The animation controller will need the triggers open and close, these will be fired when the Open and Close functions are called.

The second way is to override the Open and Close functions to fire your own custom animation scripts.  When doing this you will need to Activate and Deactivate the popup in the correct place.  The example below shows how you can achieve this.

```
public class MyPopup : PopupHandler {
	public override void Open () {
		StartCoroutine(PlayOpenAnimation(openAnimationDuration));
	}
	
	public override void Close () {
		StartCoroutine(PlayCloseAnimation(closeAnimationDuration));
	}

	protected IEnumerator PlayOpenAnimation (float duration) {
		// Activate popup first
		ActivatePopup();
		
		// Complete animation
	}
	
	protected IEnumerator PlayCloseAnimation (float duration) {
		// Complete animation first
		
		// Deactivate the popup
		DeactivatePopup();
	}
}
```

## Notes
The close and open delay are in experimental phase and are to be used as such.