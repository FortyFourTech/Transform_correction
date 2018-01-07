using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Correction {
    public class TROMarker : MonoBehaviour {

#region EDITOR_FIELDS
        [SerializeField] private Transform _xAxisSelector, _yAxisSelector, _zAxisSelector;
#endregion

#region PUBLIC_FUNCTIONS
        public void SetActiveAxis(Axis axis) {
            _xAxisSelector.gameObject.SetActive(axis != Axis.X);
            _yAxisSelector.gameObject.SetActive(axis != Axis.Y);
            _zAxisSelector.gameObject.SetActive(axis != Axis.Z);
        }

        public void SetParentObject(GameObject parentObject) {
            var tr = transform;
            if (parentObject == null) {
                tr.SetParent(Camera.main.transform);
                tr.localPosition = new Vector3(0.0f, 0.0f, -10.0f);
            } else {
                tr.SetParent(parentObject.transform);
                tr.localPosition = Vector3.zero;
            }
            tr.localEulerAngles = Vector3.zero;
            tr.localScale = Vector3.one;
        }
#endregion
    }
}
