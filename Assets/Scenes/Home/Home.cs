using UnityEngine;
using System.Collections;

public class Home : MonoBehaviour {

	Navigator navigator;
	AudioManager sfx;


	void Start () {
		navigator = Navigator.instance;
		sfx = AudioManager.instance;

		sfx.Play("Audio/Bgm/Alone", 0.5f, 1f, true);
	}

	
	public void Tap () {
		navigator.Open("Game");
	}
}
