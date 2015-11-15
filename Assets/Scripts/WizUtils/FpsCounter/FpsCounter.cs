// .Net includes
using System.Collections;

// Unity includes
using UnityEngine;
using UnityEngine.UI;

namespace WizUtils.UI {

	/// <summary>
	/// FPS counter to display the measured fps over a set period for a cleaner adv
	/// </summary>
	public class FpsCounter : SingletonComponent<FpsCounter> {
		/// <summary>
		/// The canvas the fps will be rendered to
		/// </summary>
		private Canvas canvas;

		/// <summary>
		/// Sample and display
		/// </summary>
		private const float FPS_MEASURE_PERIOD = 0.5f;
		private const string DISPLAY_FORMAT = "{0} FPS";
		private int fpsAccumulator = 0;
		private float fpsNextPeriod = 0;

		/// <summary>
		/// Current fps and whether to display it
		/// </summary>
		private int currentFps;
		private bool show;

		/// <summary>
		/// Text element that will display
		/// </summary>
		private Text textElement;

		/// <summary>
		/// The layer that the fade overlay will be in
		/// </summary>
		public short sortOrder = short.MaxValue;

		/// <summary>
		/// Configure game object
		/// </summary>
		void Awake() {
			canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;

			// Text element to display
			textElement = gameObject.AddComponent<Text>();
			textElement.transform.SetParent(canvas.transform);
		}

		/// <summary>
		/// Set up defaults
		/// </summary>
		void Start() {
			// Sample period 
			fpsNextPeriod = Time.realtimeSinceStartup + FPS_MEASURE_PERIOD;

			// Text
			textElement.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			textElement.color = Color.red;

			// sorting layer
			canvas.sortingOrder = sortOrder;
		}

		/// <summary>
		/// On Update decide whether to display
		/// </summary>
		void Update() {
			if (!Debug.isDebugBuild) { 
				return; 
			}

			if (!show) {
				return;
			}
			
			// measure average frames per second
			fpsAccumulator++;
			if (Time.realtimeSinceStartup > fpsNextPeriod) {
				currentFps = (int) (fpsAccumulator/FPS_MEASURE_PERIOD);
				fpsAccumulator = 0;
				fpsNextPeriod += FPS_MEASURE_PERIOD;
				textElement.text = string.Format(DISPLAY_FORMAT, currentFps);
			}
		}

		/// <summary>
		/// Toggle in debug mode if necessary
		/// </summary>
		/// <param name="toggle">toggle value</param>
		public void Show(bool toggle = true) {
			show = toggle;
		}
	}
}