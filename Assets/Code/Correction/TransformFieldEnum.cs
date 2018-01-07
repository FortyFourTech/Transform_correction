using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Correction {
    public enum TransformField {
        position, rotation, scale,
        positionLocal, rotationLocal, scaleLocal
    }

    public static class TransformFieldEnumExtension {
        public static Vector3 GetField(this Transform transform, TransformField field) {
            Vector3 result = Vector3.zero;
            switch (field) {
                case TransformField.position:
                    result = transform.position;
                    break;
                case TransformField.rotation:
                    result = transform.eulerAngles;
                    break;
                case TransformField.scale:
                    result = transform.lossyScale;
                    break;
                case TransformField.positionLocal:
                    result = transform.localPosition;
                    break;
                case TransformField.rotationLocal:
                    result = transform.localEulerAngles;
                    break;
                case TransformField.scaleLocal:
                    result = transform.localScale;
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "field in transform for '" + field.ToString() + "' is not defined");
                    break;
            }

            return result;
        }
        public static void SetField(this Transform transform, TransformField field, Vector3 value) {
            switch (field) {
                case TransformField.position:
                    transform.position = value;
                    break;
                case TransformField.rotation:
                    transform.eulerAngles = value;
                    break;
                case TransformField.scale:
                    if (transform.parent == null) transform.localScale = value; // partial solution
                    //transform.lossyScale = value; // TODO: write the right implementation for this case
                    break;
                case TransformField.positionLocal:
                    transform.localPosition = value;
                    break;
                case TransformField.rotationLocal:
                    transform.localEulerAngles = value;
                    break;
                case TransformField.scaleLocal:
                    transform.localScale = value;
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "field in transform for '" + field.ToString() + "' is not defined");
                    break;
            }
        }

        public static float GetValue(this Transform tr, Axis axis, TransformField field) {
            var fieldVal = tr.GetField(field);
            float result = 0;
            result = fieldVal.GetValue(axis);

            return result;
        }

        public static float GetValue(this Vector3 vector, Axis axis) {
            float result = 0;

            switch (axis) {
                case Axis.X:
                    result = vector.x;
                    break;
                case Axis.Y:
                    result = vector.y;
                    break;
                case Axis.Z:
                    result = vector.z;
                    break;
                case Axis.none:
                    result = 0;
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "axis for '" + axis.ToString() + "' is not defined");
                    break;
            }

            return result;
        }
        public static float AddValue(this Transform tr, TransformField field, Axis axis, float value) {
            var currentFieldVal = tr.GetField(field);
            Vector3 newFieldVal;
            if (field == TransformField.rotation || field == TransformField.rotationLocal) {
                var rot = field == TransformField.rotation ? tr.rotation : tr.localRotation;
                tr.Rotate(axis.AxisVector()*value, field == TransformField.rotation ? Space.World : Space.Self);
                newFieldVal = tr.GetField(field);
            } else {
                Vector3 changeVector = axis.AxisVector();
                if (field == TransformField.positionLocal)
                    changeVector = tr.rotation * changeVector;
                newFieldVal = currentFieldVal + changeVector * value;
                tr.SetField(field, newFieldVal);
            }

            float newVal;
            switch (axis) {
                case Axis.X:
                    newVal = newFieldVal.x;
                    break;
                case Axis.Y:
                    newVal = newFieldVal.y;
                    break;
                case Axis.Z:
                    newVal = newFieldVal.z;
                    break;
                default:
                    newVal = 0;
                    break;
            }
            return newVal;
        }
    }
}
