// Unity includes
using UnityEngine;

namespace WizUtils {

	/// <summary>
	/// Singleton Implementation, for use with unity components.
	/// </summary>
	public class SingletonComponent<T> : MonoBehaviour where T : Component, new() {
		// Private
		private static T instance;

		// Public
		public static T Instance { 
			get { 
				if (instance == null) {
					T otherCheck = GameObject.FindObjectOfType<T>();

					if (otherCheck != null) {
						return otherCheck;
					}

					GameObject go = new GameObject(typeof(T).ToString());
					instance = go.AddComponent<T>();

					DontDestroyOnLoad(instance.gameObject);
				}
				return instance;
			} 
		}
	}
}