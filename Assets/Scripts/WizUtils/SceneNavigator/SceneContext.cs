namespace WizUtils {
	/// <summary>
	/// Scene context for holding open args and the sceneId
	/// </summary>
	public class SceneContext {
		public string SceneId { get; private set; }
		public object Args { get; private set; }
		
		public SceneContext(string sceneId, object args) {
			this.SceneId = sceneId;
			this.Args = args;
		}
	}
}