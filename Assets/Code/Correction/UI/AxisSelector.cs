using System;
using UnityEngine;
using UnityEngine.UI;

namespace Correction.UI {
    public class AxisSelector : MonoBehaviour {
        
#region EDITOR_FIELDS
        [SerializeField] private Button _XBtn;
        [SerializeField] private Button _YBtn;
        [SerializeField] private Button _ZBtn;
#endregion

#region UNITY_FUNCTIONS
        private void OnDestroy() {
            _XBtn.onClick.RemoveAllListeners();
            _YBtn.onClick.RemoveAllListeners();
            _ZBtn.onClick.RemoveAllListeners();
        }
#endregion

#region PUBLIC_FUNCTIONS
        public void AddAxisBtnAction(Axis axis, UnityEngine.Events.UnityAction action) {
            var button = GetButtonForAxis(axis);
            button.onClick.AddListener(action);
        }

        public void RemoveAxisBtnAction(Axis axis, UnityEngine.Events.UnityAction action) {
            var button = GetButtonForAxis(axis);
            button.onClick.RemoveListener(action);
        }

        public void SetActiveAxis(Axis axis) {
            _XBtn.interactable = (axis != Axis.X);
            _YBtn.interactable = (axis != Axis.Y);
            _ZBtn.interactable = (axis != Axis.Z);
    }
#endregion

#region PRIVATE_FUNCTIONS
        private Button GetButtonForAxis(Axis axis) {
            Button result = null;
            switch(axis) {
                case Axis.X:
                    result = _XBtn;
                    break;
                case Axis.Y:
                    result = _YBtn;
                    break;
                case Axis.Z:
                    result = _ZBtn;
                    break;
                case Axis.none:
                    throw new Exception("Axis selector does not contain button for \"none\" axis");
            }

            return result;
        }
#endregion
    }
}
