using UnityEngine;
using System.Collections;

public class Home : MonoBehaviour {

	Navigator navigator;


	void Start () {
		navigator = Navigator.instance;
		DontDestroyOnLoad(navigator.gameObject);

		Audio audio = Audio.instance;
		DontDestroyOnLoad(audio.gameObject);
		
		Audio.play("Audio/Bgm/Alone", 0.5f, 1f, true);
	}

	
	public void Tap () {
		navigator.Open("Game");
	}
}
