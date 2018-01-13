using System;
using System.Linq;
using UnityEngine;

public static class Utility {
    public static bool CheckScriptOnScene(Type type) {
        var objectsFound = GameObject.FindObjectsOfTypeAll(type);
        objectsFound = objectsFound.Where(x => (x as Component).gameObject.scene.buildIndex >= 0).ToArray();
        // FindObjectsOfTypeAll returns 2 objects for some reason. So this filter is filtering prefabs
        
        return objectsFound.Length == 1;
    }

    public static bool CheckObjectOnScene(string name) {
        var objectFound = GameObject.Find(name);

        return objectFound != null;
    }
}