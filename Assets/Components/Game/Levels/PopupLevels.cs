using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PopupLevels : MonoBehaviour {

	public GameObject buttonPrefab;
	private List<Button> levelButtons = new List<Button>();
	private Transform bg;
	private InputField inputField;

	private Hud hud;
	private LevelManager levelManager;
	private string mode;


	public void Init (Hud hud, LevelManager levelManager, string mode) {

		bg = transform.Find("Bg");
		inputField = transform.Find("InputField").GetComponent<InputField>();
		transform.Find("Footer/ButtonLoadSave/Text").GetComponent<Text>().text = mode.ToUpper();

		this.hud = hud;
		this.levelManager = levelManager;
		this.mode = mode;

		Reset();
		LoadLevels();
	}


	private void Reset () {
		foreach (Button button in levelButtons) {
			Destroy(button.gameObject);
		}

		levelButtons.Clear();
	}


	private void LoadLevels () {
		JSONObject json = JsonFileManagerSync.LoadJsonFile("Data/LevelsIndex");
		if (json != null) {
			GenerateLevelButtonsFromData(json);
		}
	}


	private void GenerateLevelButtonsFromData (JSONObject json) {
		JSONObject levels = json["levels"];

		for (int i = 0; i < levels.Count; i++) {
			levelButtons.Add(CreateLevelButton(i, levels[i]["name"].str));
		}
	}


	private Button CreateLevelButton (int i, string name) {
		GameObject obj = GameObject.Instantiate(buttonPrefab);
		obj.transform.SetParent(bg.transform, false);
		obj.name = "Button " + i;
		obj.transform.Find("Text").GetComponent<Text>().text = name;
		Button button = obj.GetComponent<Button>();
		
		button.onClick.AddListener( delegate { 
			print ("clicked on button");
			SelectLevelButton(name);
		} );

		return button;
	}


	private void SelectLevelButton (string name) {
		inputField.text = name;
	}


	public void ButtonLoadSave () {
		if (System.String.IsNullOrEmpty(inputField.text)) {
			return;
		}

		if (mode == "save") {
			levelManager.SaveFile(inputField.text);
		} else {
			levelManager.LoadFile(inputField.text);
		}
		
		hud.HidePopupLevels();
	}


	public void ButtonCancel () {
		hud.HidePopupLevels();
	}
}
