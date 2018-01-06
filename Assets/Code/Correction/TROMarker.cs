using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Correction {
    public class TROMarker : MonoBehaviour {
        public Transform axisX;
        public Transform axisY;
        public Transform axisZ;

        private GameObject m_xSelector;
        private GameObject m_ySelector;
        private GameObject m_zSelector;

        // Use this for initialization
        void Start() {
            m_xSelector = axisX.Find("Sphere/Cube").gameObject;
            m_ySelector = axisY.Find("Sphere/Cube").gameObject;
            m_zSelector = axisZ.Find("Sphere/Cube").gameObject;
        }

        public void SetActiveAxis(Axis p_axis) {
            m_xSelector.SetActive(p_axis != Axis.X);
            m_ySelector.SetActive(p_axis != Axis.Y);
            m_zSelector.SetActive(p_axis != Axis.Z);
        }
    }
}
