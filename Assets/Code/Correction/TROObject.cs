using UnityEngine;

namespace Correction {
    public class TROObject : MonoBehaviour {
        public float ChangeTransform(TransformField field, Axis axis, float val) {
            return transform.AddValue(field, axis, val);
        }
    }
}