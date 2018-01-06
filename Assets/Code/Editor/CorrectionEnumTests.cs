using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Correction;

namespace Testing {
    public partial class EnumTests {
        [Test]
        public void TransformFieldEnumValuesTest() {
            Action<Vector3, Vector3, string> compareVectors = (v1, v2, msg) => {
                Assert.AreEqual(v1.x, v2.x, 0.001f, msg);
                Assert.AreEqual(v1.y, v2.y, 0.001f, msg);
                Assert.AreEqual(v1.z, v2.z, 0.001f, msg);
            };
            //TestExtensionMethods(typeof(TransformField));
            var posOriginal = new Vector3(1, 2, 3);
            var rotOriginal = new Vector3(4, 5, 6);
            var scaleOriginal = new Vector3(7, 8, 9);
            var obj = new GameObject(); // TODO: test object that is a child. for local fields
            var tr = obj.transform;
            var fieldValues = Enum.GetValues(typeof(TransformField)) as TransformField[];
            var axisValues = new Axis[] { Axis.X, Axis.Y, Axis.Z };

            tr.SetField(TransformField.position, posOriginal);
            var posField = tr.GetField(TransformField.position);
            compareVectors(posOriginal, posField, "position is not correct");
            tr.SetField(TransformField.positionLocal, posOriginal);
            posField = tr.GetField(TransformField.positionLocal);
            compareVectors(posOriginal, posField, "local position is not correct");

            tr.SetField(TransformField.rotation, rotOriginal);
            var rotField = tr.GetField(TransformField.rotation);
            compareVectors(rotOriginal, rotField, "rotation is not correct");
            tr.SetField(TransformField.rotationLocal, rotOriginal);
            rotField = tr.GetField(TransformField.rotationLocal);
            compareVectors(rotOriginal, rotField, "local rotation is not correct");

            tr.SetField(TransformField.scale, scaleOriginal);
            var scaleField = tr.GetField(TransformField.scale);
            compareVectors(scaleOriginal, scaleField, "scale is not correct");
            tr.SetField(TransformField.scaleLocal, scaleOriginal);
            scaleField = tr.GetField(TransformField.scaleLocal);
            compareVectors(scaleOriginal, scaleField, "local scale is not correct");

            Action<TransformField, Axis, float, float> testSettingAxisValue = delegate (TransformField field, Axis axis, float addDelta, float expectedDelta) {
                var originalField = tr.GetField(field);

                var originalFieldValue = tr.GetValue(axis, field);
                tr.AddValue(axis, field, addDelta);
                var newFieldValue = tr.GetValue(axis, field);
                Assert.AreEqual(originalFieldValue + expectedDelta, newFieldValue, 0.001f, "field value is different after set for field " + field.ToString() + " and axis " + axis.ToString()); // wrong checking

                tr.SetField(field, originalField);
            };
            float delta = 30;
            foreach (var field in fieldValues) {
                foreach (var axis in axisValues) {
                    testSettingAxisValue(field, axis, delta, delta);
                }

                testSettingAxisValue(field, Axis.none, delta, 0);
            }
        }
    }
}
