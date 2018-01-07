using NUnit.Framework;
using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Runtime.CompilerServices;
using UnityEngine;
using Correction;
using Random = UnityEngine.Random;

namespace Testing {
    public static class Utility {
        public static void CompareVectors (Vector3 v1, Vector3 v2, string msg) {
            Assert.AreEqual(v1.x, v2.x, 0.001f, msg);
            Assert.AreEqual(v1.y, v2.y, 0.001f, msg);
            Assert.AreEqual(v1.z, v2.z, 0.001f, msg);
        }
    }

    public partial class EnumTests {
        private void TestSettingFieldValue (Transform tr, TransformField field) {
            var setValue = Random.insideUnitSphere;

            tr.SetField(field, setValue);
            var realValue = tr.GetField(field);
            if (field == TransformField.rotation || field == TransformField.rotationLocal) {
                setValue = Quaternion.Euler(setValue).eulerAngles; // normalize angles vector
            }
            Utility.CompareVectors(setValue, realValue, field.ToString() + " set/get works incorrect");
        }

        private void TestAddingAxisValue (Transform tr, TransformField field, Axis axis, bool expectChange = true) {
            var addDelta = Random.value * Mathf.Pow(10, Random.Range(0,4)); // [0...1000]
            var expectedDelta = expectChange ? addDelta : 0f;

            var originalFieldValue = tr.GetField(field);
            var originalValue = tr.GetValue(axis, field);
            tr.AddValue(field, axis, addDelta);
            var newValue = tr.GetValue(axis, field);
            var expectedValue = originalValue + expectedDelta;

            var assertMsg = "field value is different after add for field " + field.ToString() + " and axis " + axis.ToString();
            if (field == TransformField.rotation || field == TransformField.rotationLocal) { // TODO: dont know how to check rotation

            } else {
                Assert.AreEqual(expectedValue, newValue, 0.001f, assertMsg);
            }

            tr.SetField(field, originalFieldValue);
        }

        [Test]
        public void TransformFieldEnumValuesTest() {
            var obj = new GameObject(); // TODO: test object that is a child. for local fields
            var tr = obj.transform;

            // var fieldValues = Enum.GetValues(typeof(TransformField)) as TransformField[];
            var fieldValues = new TransformField[] { TransformField.position, TransformField.rotation, TransformField.scale }; // TODO: cant test the rest for now

            foreach (var field in fieldValues) {
                TestSettingFieldValue(tr, field);
            }

            var axisValues = new Axis[] { Axis.X, Axis.Y, Axis.Z };
            foreach (var field in fieldValues) {
                foreach (var axis in axisValues) {
                    TestAddingAxisValue(tr, field, axis);
                }

                TestAddingAxisValue(tr, field, Axis.none, false);
            }
        }
    }
}
