using UnityEngine;

namespace Correction {
    public class TROObject : MonoBehaviour {
        
        public void setReady(bool _b) {
            //Trigger tr = gameObject.GetComponent<Trigger> ();
            //if (tr != null) {
            //	tr.setVisible (_b);
            //}
        }

        public float changeTransform(TransformField field, Axis axis, float val) {
            return
                gameObject.transform.AddValue(axis, field, val);
        }
    }
}