using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Correction {
    public enum Axis {
        none, X, Y, Z
    }

    public static class AxisEnumExtension {
        public static Vector3 AxisVector(this Axis axis) {
            Vector3 result = Vector3.zero;
            switch (axis) {
                case Axis.X:
                    result = Vector3.right;
                    break;
                case Axis.Y:
                    result = Vector3.up;
                    break;
                case Axis.Z:
                    result = Vector3.forward;
                    break;
                case Axis.none:
                    result = Vector3.zero;
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "vector for axis '" + axis.ToString() + "' is not defined");
                    break;
            }

            return result;
        }

        // not using at the time, but may be handful
        public static Vector3 AxisVector(this Transform tr, Axis axis) {
            Vector3 result = Vector3.zero;
            switch (axis) {
                case Axis.X:
                    result = tr.right;
                    break;
                case Axis.Y:
                    result = tr.up;
                    break;
                case Axis.Z:
                    result = tr.forward;
                    break;
                case Axis.none:
                    result = Vector3.zero;
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "vector for axis '" + axis.ToString() + "' is not defined");
                    break;
            }

            return result;
        }
    }
}
