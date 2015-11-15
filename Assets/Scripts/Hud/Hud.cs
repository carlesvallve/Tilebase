using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {

	private Navigator navigator;
	

	void Awake () {
		navigator = Navigator.instance;
	}

	
	public void ButtonMenu () {
		navigator.Open("Home");
	}
}
