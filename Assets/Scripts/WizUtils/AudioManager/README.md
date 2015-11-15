# Audio Manager
Singleton component that can manage BGM and SFX with up to 16 channels

## Dependencies
* UnityEngine
* WizUtils.SingletonComponent

## API
* PlayBGM
* StopBGM
* PlaySFX
* CurrentBGMVolume
* CurrentSFXVolume
* ToggleBGM
* ToggleSFX
* SetBGMVolume
* SetSFXVolume

## Usage
### Playing BGM
To play a simple looped background music file make sure that the file is available in the resources folder.  In this example it is in Resources/Audio/ and the file is called BackgroundMusic.  This file will automatically be looped by the AudioManager.

```
public class MyFirstView {
	public AudioManager audioManager {
		get {
			return AudioManager.Instance;
		}
	}
	
	void Start() {
		audioManager.PlayBGM("Audio/BackgroundMusic");
	}
}
```

### Playing SFX
You can play a sound effect in the exact same way, these will not be looped.
```
public class MyFirstView {
	public AudioManager audioManager {
		get {
			return AudioManager.Instance;
		}
	}
	
	void Start() {
		audioManager.PlaySFX("Audio/SFX/Bang");
	}
}
```

### Advanced BGM
There is the ability to play a non-looped start to the music and then loop the second part, this is an overload for the API.
```
public class MyFirstView {
	public AudioManager audioManager {
		get {
			return AudioManager.Instance;
		}
	}
	
	void Start() {
		audioManager.PlayBGM("Audio/BGMIntro", "Audio/BGMLoop");
	}
}
```

## Notes
There is no real need to create a getter for each view, it is possible to use the instance directly, however for readability it is recommended.
```
public class MyFirstView {
	public AudioManager audioManager {
		get {
			return AudioManager.Instance;
		}
	}
	
	void Start() {
		// These are the same
		AudioManager.Instance.PlayBGM("Audio/BGMIntro", "Audio/BGMLoop");
		audioManager.PlayBGM("Audio/BGMIntro", "Audio/BGMLoop);
	}
}
```

There are a few overloads on the play functions which allow you to pass in an AudioSource directly rather than loading from resources, this is useful when you are loading from an asset bundle for example.