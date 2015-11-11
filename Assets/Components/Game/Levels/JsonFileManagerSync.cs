using UnityEngine;
using System.Collections;
using System.IO;

public class JsonFileManagerSync : MonoBehaviour {

	public static bool SaveJsonFile (string fileName, JSONObject data) {
		// get file path
		string path = null;

		#if UNITY_EDITOR
			path = Application.dataPath + "/Resources/" + fileName + ".json";
		#else
			path = Application.persistentDataPath + "/" + fileName + ".json";  
		#endif

		Directory.CreateDirectory(Path.GetDirectoryName(path));

		// write data as a string
		string str = data.ToString();
		using (FileStream fs = new FileStream(path, FileMode.Create)) {
			using (StreamWriter writer = new StreamWriter(fs)) {
				writer.Write(str);

				// refresh the editor
				#if UNITY_EDITOR
					UnityEditor.AssetDatabase.Refresh ();
				#endif

				return true;
			}
		}
	}


	public static JSONObject LoadJsonFile (string fileName) {
		string path = null;

		#if UNITY_EDITOR
			path = Application.dataPath + "/Resources/" + fileName + ".json";
		#else
			path = Application.persistentDataPath + "/" + fileName + ".json";  
		#endif
		
		
		if (File.Exists(path)) {
			FileStream file = new FileStream (path, FileMode.Open, FileAccess.Read);
			StreamReader sr = new StreamReader( file );

			string str = null;
			str = sr.ReadLine ();

			sr.Close();
			file.Close();

			JSONObject json = new JSONObject (str);
			return json;
		}

		return null;
	}
}
