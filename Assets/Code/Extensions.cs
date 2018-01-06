using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
    public static void DebugLog(this MonoBehaviour script, string debugText) {
        var possiblePathes = new string[] {
            "DebugCanvas/Text",
            "Canvas/SceneName",
        };
        GameObject textObject = null;
        foreach (var path in possiblePathes) {
            textObject = GameObject.Find(path);
            if (textObject != null) break;
        }

        if (textObject != null) {
            var textComponent = textObject.GetComponent<UnityEngine.UI.Text>();
            textComponent.text = debugText;
        }

        Debug.Log(debugText);
    }

    public static string getScenePath(this GameObject go) {
        string fullName = go.name;
        Transform parent = go.transform.parent;
        while (parent != null) {
            fullName = parent.gameObject.name + "/" + fullName;
            parent = parent.parent;
        }

        return fullName;
    }
}
