﻿using UnityEngine;

namespace Correction {
    public struct GameObjectCorrection {

        public string objectName;

        // position
        public float posX, posY, posZ;

        // rotation
        public float rotX, rotY, rotZ;

        // scale
        public float scalX, scalY, scalZ;

        public GameObjectCorrection(GameObject go) {
            // name
            objectName = go.getScenePath();

            // position
            posX = go.transform.localPosition.x;
            posY = go.transform.localPosition.y;
            posZ = go.transform.localPosition.z;

            // rotation
            rotX = go.transform.localEulerAngles.x;
            rotY = go.transform.localEulerAngles.y;
            rotZ = go.transform.localEulerAngles.z;

            // scale
            scalX = go.transform.localScale.x;
            scalY = go.transform.localScale.y;
            scalZ = go.transform.localScale.z;
        }

        public void Apply() {
            var go = GameObject.Find(objectName);

            if (go != null) {
                go.transform.localPosition = new Vector3(posX, posY, posZ);
                go.transform.localEulerAngles = new Vector3(rotX, rotY, rotZ);
                go.transform.localScale = new Vector3(scalX, scalY, scalZ);
            }
        }
    }
}
