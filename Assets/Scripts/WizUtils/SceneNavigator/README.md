# Scene Navigator
This class controls the opening and closing of scenes, it allows you to keep a history, and pass data between scenes.

## Dependencies
* UnityEngine

## API
Open
Back
FadeInScene
FadeOutScene
OpenArgs
Color
Duration
SortOrder

## Usage
### Open a scene
All scenes must be in the build list for Unity, after this it's simple to call the open method.  The signature for this method is Scene, Arguments, Store in History, Fade Transition.  By default history is stored and the fade transitions is played.

```
public class MyFirstView {
	void Start() {
		SceneNavigator.Instance.Open("MySecondView", null, false, false);
	}
}
```

### History and Open arguments
The history object allows the use of the Back function on the scene manager, it will propergate the Arguments automatically by saving them on the stack.  This means they will always be held in scope and they can be changed as the arguments are stored as a reference.  The arguments are stored in a single object, so the view being opened is responsible for casting to the correct type.

```
public class MyFirstView {
	void Start() {
		SceneNavigator.Instance.Open("MySecondView", "my open string", false, false);
	}
}

public class MySecondView {
	void Start() {
		SceneNavigator.Instance.Open("MyThirdView", null, true, false);
	}
}

public class MyThirdView {
	void Start() {
		SceneNavigator.Instance.Back();
	}
}
```

The above code will navigate from the first view to the second view without history this means you will not be able to use the back to go from the second view to the first, it passes the open args that are required for the second view to load.  When the second view navigates to the third view it stores itself in history, which is saved with the current open args.  On the third view you can now use the "Back" functionality which will navigate to the second view, with the open args from the first.

## Notes
There are a few settings that you can change Color, Duration, and SortOrder these will effect the fade if you choose to use it.