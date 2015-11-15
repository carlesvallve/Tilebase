# FPS Counter
This will display the current average FPS of the game, once initialised it will add the display to the top left of the screen.

## Dependencies
* UnityEngine
* UnityEngine.UI
* WizUtils.SingletonComponent

## API
sortOrder
Show

## Usage
### Display FPS
For showing the FPS counter on the top you can simply call the Instance, by default the sort order is the highest possible in Unity.

```
public class MyFirstView {
	void Start() {
		FpsCounter fps = FpsCounter.Instance;
		fps.Show();
	}
}
```

You can change the layer order directly in the Start method on the view

```
public class MyFirstView {
	void Start() {
		FpsCounter fps = FpsCounter.Instance;
		fps.sortOrder = 2;
		fps.Show();
	}
}
```

## Notes
Show can take a bool value of false if you want to only display the FPS at certain times.  FPS counter is ALWAYS turned off when you are not in Unity.  This maybe changed in the future to reply on the Environment type of the Settings Manager.
