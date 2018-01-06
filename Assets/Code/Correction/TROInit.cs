using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Correction {
    public class TROInit : MonoBehaviour {
        // TODO: make this loaded once
        public List<Transform> mCorrectableObjectsRoots;

        void Start () {
#if CORRECTION
            //SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            Debug.Log("TRO subscribed to scene loaded");
            SceneManager_sceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
#endif
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
            Debug.Log("TRO got scene loaded");
            // spawn correction UI
            var prefab = Resources.Load("CorrectionUI");
            var go = Instantiate(prefab) as GameObject;

            // add TROObject to all objects that needed
            foreach (var root in mCorrectableObjectsRoots) {
                for (int childIdx = 0; childIdx < root.childCount; ++childIdx) {
                    var objTr = root.GetChild(childIdx);
                    objTr.gameObject.AddComponent<TROObject>();
                }
            }
        }
    }
}
