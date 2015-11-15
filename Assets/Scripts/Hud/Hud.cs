using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {

	
	private Game game;

	void Awake () {
		game = GetComponent<Game>();
	}

	
	public void ButtonMenu () {
		game.Navigate("Home");
	}
}
