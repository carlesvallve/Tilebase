using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Navigator : MonoSingleton<Navigator> {
	
	public Color Color = Color.black;
	public float Duration = 1f;
	public short SortOrder = short.MaxValue;

	private Canvas canvas;
	private CanvasGroup group;
	private Image overlay;


	void Awake() {
		// Container and canvas inside the instanced Navigator
		GameObject container = new GameObject(this.GetType().ToString() + ".Overlay");
		container.transform.SetParent(transform);

		// Canvas for full screen overlay render
		canvas = container.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;

		// Overlay for the color
		overlay = container.AddComponent<Image>();
		overlay.transform.SetParent(container.transform);

		// Group for the alpha
		group = container.AddComponent<CanvasGroup>();
		group.transform.SetParent(container.transform);


		overlay.color = Color;
		canvas.sortingOrder = SortOrder;

		//GotoScene(Application.loadedLevelName);
		StartCoroutine(FadeIn());
	}


	public void Open(string sceneName, bool fade = true) {
		StartCoroutine(GotoScene(sceneName, fade));
	}


	private IEnumerator GotoScene(string sceneName, bool fade = true) {

		if (fade) {
			yield return StartCoroutine(FadeOut());
		}

		Application.LoadLevel(sceneName);
		
		yield return null;

		if (fade) {
			StartCoroutine(FadeIn());
		}
	}


	private IEnumerator FadeIn() {
		group.alpha = 1;
		group.gameObject.SetActive(true);
		
		float elapsedTime = 0;
		while (elapsedTime < Duration) {
			float t = elapsedTime / Duration;
			group.alpha = Mathf.Lerp(1, 0, Mathf.SmoothStep(0f, 1f, t)); 
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		group.alpha = 0;
		group.gameObject.SetActive(false);
	}


	private IEnumerator FadeOut() {
		group.gameObject.SetActive(true);
		group.alpha = 0;
		
		float elapsedTime = 0;
		while (elapsedTime < Duration) {
			float t = elapsedTime / Duration;
			group.alpha = Mathf.Lerp(0, 1, Mathf.SmoothStep(0f, 1f, t)); 
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		group.alpha = 1;	
	}
}
