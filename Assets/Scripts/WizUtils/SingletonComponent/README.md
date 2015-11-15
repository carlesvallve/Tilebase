# Singleton Component
Singleton component base class that handles the initialisation of the game object and holds them between scenes.

## Dependencies
* UnityEngine

## API
Instance

## Usage
### Creating a simple singleton
You can turn any normal class into a Unity singleton that will be propergated between scenes to do this you inherit the SingletonComponent and type it to the class that you are creating.

```
public class MyFirstClass : SingletonComponent<MyFirstClass> {
	public int DoSomething() {
		return 1 + 1;
	}
}
```

You do not need, and should not, attach this class to any game object in your scenes.  To access this new singleton class you need to get the instance of it.

```
public class MyFirstView {
	void Start() {
		MyFirstClass myClass = MyFirstClass.Instance;
	}
}
```

This will either get the instance if it has been accessed before, or create one if it is the first time.  Once you have the instance you can use any of its functions as normal.
```
public class MyFirstView {
	void Start() {
		MyFirstClass myClass = MyFirstClass.Instance;
		int math = myClass.DoSomething();
	}
}
```

## Notes
The new singletons MUST be called first in the main thread, if you do try to access the Instance in a different thread before you have accessed it in the main thread it will cause an error.
