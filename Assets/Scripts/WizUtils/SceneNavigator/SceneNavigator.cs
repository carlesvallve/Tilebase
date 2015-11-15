// .Net includes
using System.Collections;
using System.Collections.Generic;

// Unity includes
using UnityEngine;
using UnityEngine.UI;

namespace WizUtils {

	/// <summary>
	/// The Scene Navigator is placed into each scene (or at the start) and will propergate itself through the scenes, 
	/// it can track open args to the scenes as object (requires scene to cast)
	/// </summary>
	public class SceneNavigator : SingletonComponent<SceneNavigator> {
		/// <summary>
		/// The canvas that holds the fade
		/// </summary>
		private Canvas canvas;

		/// <summary>
		/// The that will be faded
		/// </summary>
		private CanvasGroup group;

		/// <summary>
		/// The color overlay that will fade during transition
		/// </summary>
		private Image overlay;

		/// <summary>
		/// Reference to current scene so we can build a list of previous scenes
		/// </summary>
		private string currentScene;
			
		/// <summary>
		/// Previous scenes in stack
		/// </summary>
		private Stack<SceneContext> previousScenes = new Stack<SceneContext>();

		/// <summary>
		/// This is to propergate data between scenes
		/// </summary>
		public object OpenArgs { get; private set; }

		/// <summary>
		/// The color of the default fade
		/// </summary>
		public Color Color = Color.black;
		
		/// <summary>
		/// The duration.
		/// </summary>
		public float Duration = 1f;

		/// <summary>
		/// The layer that the fade overlay will be in
		/// </summary>
		public short SortOrder = short.MaxValue;

		/// <summary>
		/// On awake set up and game objects and references we will need
		/// </summary>
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

			currentScene = Application.loadedLevelName;
		}

		/// <summary>
		/// On start set any defaults that need setting
		/// </summary>
		void Start() {
			overlay.color = Color;
			canvas.sortingOrder = SortOrder;
		}

		/// <summary>
		/// Opens a scene
		/// </summary>
		/// <param name="scene">Scene name to open</param>
		/// <param name="args">Arguments to pass, the scene will have to cast</param>
		/// <param name="history">If we want to use this in back</param>
		/// <param name="fade">Wether to fade the scene</param>
		public void Open(string scene, object args, bool history = true, bool fade = true) {
			// Push last scene to stack only if we keep history
			if (history) {
				previousScenes.Push(new SceneContext(currentScene, OpenArgs));
			}

			// Set open args, we do not preserve state for more than one transition
			OpenArgs = args;

			// Set the scene we are loading to current
			currentScene = scene;

			StartCoroutine(GotoScene(currentScene, fade));
		}

		/// <summary>
		/// Custom access to the IEnumerator so we can manually trigger fades
		/// </summary>
		public void FadeInScene() {
			StartCoroutine(FadeIn());
		}

		/// <summary>
		/// Custom access to the IEnumerator so we can manually trigger fades
		/// </summary>
		public void FadeOutScene() {
			StartCoroutine(FadeOut());
		}

		/// <summary>
		/// Go back to the previous scene
		/// </summary>
		public void Back() {
			// If there are no previous scenes so nothing
			if (previousScenes.Count == 0) {
				return;
			}
			
			// Get previous scene from stack and set to current
			SceneContext previousContext = previousScenes.Pop();
			currentScene = previousContext.SceneId;
			OpenArgs = previousContext.Args;
			
			StartCoroutine(GotoScene(currentScene));
		}

		/// <summary>
		/// Transitions to scene with or without fade
		/// </summary>
		/// <returns>The scene.</returns>
		/// <param name="sceneName">Scene to go to</param>
		/// <param name="fade">If we want to fade in and out</param>
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

		/// <summary>
		/// Fades the in.
		/// </summary>
		/// <returns>The in.</returns>
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

		/// <summary>
		/// Fades the out.
		/// </summary>
		/// <returns>The out.</returns>
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
}

