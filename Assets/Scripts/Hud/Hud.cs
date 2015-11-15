using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoBehaviour {

	public Transform container;

	private Game game;
	private Text dungeonLevelText;
	private Text logText;

	void Awake () {
		game = GetComponent<Game>();
		dungeonLevelText = container.Find("Header/DungeonLevel").GetComponent<Text>();
		logText = container.Find("Footer/Log/Msg").GetComponent<Text>();
	}

	
	public void ButtonMenu () {
		game.Navigate("Home");
	}


	public void UpdateHeader (int currentDungeonLevel) {
		dungeonLevelText.text = "Dungeon level" + (currentDungeonLevel + 1);
	}

	public void UpdateLog (string msg) {
		logText.text = msg;
	}
}
