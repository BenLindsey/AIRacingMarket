using UnityEngine;
using System.IO;

public class LocalRaceStarter : MonoBehaviour {

	public string[] scriptsToRace;

	// Use this for initialization
	void Start () {
		foreach (string scriptName in scriptsToRace) {
            Script script;
            script.name = scriptName;
            script.contents = GetScript(Directory.GetCurrentDirectory() + "\\Assets\\Racing Scripts\\" + scriptName);
			GetComponent<CarManager>().AddScriptByName(script.name);
            GetComponent<CarManager>().AddScriptContent(script.contents);
		}
		GetComponent<CarManager>().StartRace("");
	}

    private string GetScript(string scriptPath) {

        // Return null if the script does not exist.
        if (!File.Exists(scriptPath)) {
            Debug.LogWarning("The script '" + scriptPath + "' does not exist!");

            return null;
        }

        // Otherwise, return the AI script.
        StreamReader reader = new StreamReader(scriptPath);
        string script = reader.ReadToEnd();
        reader.Close();

        return script;
    }
}
