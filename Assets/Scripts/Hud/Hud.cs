using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoBehaviour {

	public Transform container;

	private Game game;
	private Text dungeonLevelText;

	void Awake () {
		game = GetComponent<Game>();
		dungeonLevelText = container.Find("Header/DungeonLevel").GetComponent<Text>();
	}

	
	public void ButtonMenu () {
		game.Navigate("Home");
	}


	public void UpdateHeader (int currentDungeonLevel) {
		dungeonLevelText.text = "Dungeon level" + (currentDungeonLevel + 1);
	}
}
