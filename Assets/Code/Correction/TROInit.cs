using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Correction {
    /// <summary>
    /// Add this script to the scene, then
    /// Add root objects to mCorrectableObjectsRoots.
    /// After start all objects inside root objects will be correctable.
    /// </summary>
    public class TROInit : MonoBehaviour {

#region EDITOR_FIELDS
        public List<Transform> CorrectableObjectsRoots;
#endregion

#region UNITY_FUNCTIONS

        void Start () {
            CheckUIOnScene();

            Destroy(gameObject);
        }
#endregion

#region PRIVATE_FUNCTIONS
        private void CheckUIOnScene() {
            // Dont spawn UI, if its been placed to the scene manually.
            if (!IsUIOnScene()) {
                // spawn correction UI
                var prefab = Resources.Load("CorrectionUI") as GameObject;
                /*var go =*/ Instantiate(prefab) /*as GameObject*/;
            }
            
            // add TROObject to all objects that needed
            foreach (var root in CorrectableObjectsRoots) {
                for (int childIdx = 0; childIdx < root.childCount; ++childIdx) {
                    var objTr = root.GetChild(childIdx);
                    var objCompo = objTr.GetComponent<TROObject>();
                    if (objCompo == null)
                        objTr.gameObject.AddComponent<TROObject>();
                }
            }
        }

        private bool IsUIOnScene() {
            return Utility.CheckScriptOnScene(typeof(UI.UIManager));
        }
#endregion
    }
}
