using UnityEngine;

namespace Correction {
    public class TROObject : MonoBehaviour {
        public float changeTransform(TransformField field, Axis axis, float val) {
            return
                gameObject.transform.AddValue(field, axis, val);
        }
    }
}