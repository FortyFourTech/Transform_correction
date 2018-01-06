﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Correction;

namespace Correction.UI {
    public class TROManager : MonoBehaviour {
        public GameObject TransformMarkPrefab;
        private TROMarker _transformMarker;

        private Camera Cam;
        private TROObject[] Ts;
        private Axis _choosedAxis = Axis.none;
        private TROObject _currentTRO = null;
        private bool started = false, selected = false;

        private Toggle Toggle_Start;

        // selection
        private Toggle Toggle_Select;
        [SerializeField]
        private Transform m_axisSelection;
        private Button Button_X, Button_Y, Button_Z;

        // position correction
        [SerializeField]
        private TROStepper m_positionManage;
        // rotation correction
        [SerializeField]
        private TROStepper m_rotationManage;
        // scale correction
        [SerializeField]
        private TROStepper m_scaleManage;

        private List<GameObject> redactedObjects = new List<GameObject>();

        void Start() {
            Cam = Camera.main;
            Ts = GameObject.FindObjectsOfType<TROObject>();
            
            _transformMarker = (Instantiate(TransformMarkPrefab) as GameObject).GetComponent<TROMarker>();
            _setObjectTransformMarkerObject(null);

            _initGui();
            //--------
            _setObjectTransformMarkerObject(null);

            _loadSerialized();
        }

        void Update() {
            if (!started)
                return;
            if (selected)
                return;

            float minDistance = 0.0f;
            TROObject minDGO = null;
            RaycastHit[] RcHs = Physics.RaycastAll(new Ray(Cam.transform.position, Cam.transform.forward));
            for (int i = 0; i < RcHs.Length; i++) {
                RaycastHit RcH = RcHs[i];
                GameObject gol = RcH.transform.gameObject;
                TROObject tr = gol.GetComponentInParent<TROObject>();
                if (tr != null) {
                    float dist = RcH.distance;
                    if ((minDGO == null) || (dist < minDistance)) {
                        minDistance = dist;
                        minDGO = tr;
                    }
                }
            }

            TROObject newPossibleTRO = (minDGO == null) ? null : minDGO;

            if (_currentTRO != newPossibleTRO) {
                _currentTRO = newPossibleTRO;
                _setObjectTransformMarkerObject((_currentTRO == null) ? null : _currentTRO.gameObject);
            }
        }

        private void _setObjectTransformMarkerObject(GameObject _go) {
            Transform tr = _transformMarker.transform;
            if (_go == null) {
                tr.SetParent(Cam.transform);
                tr.localPosition = new Vector3(0.0f, 0.0f, -10.0f);
            } else {
                tr.SetParent(_go.transform);
                tr.localPosition = Vector3.zero;
            }
            tr.localEulerAngles = Vector3.zero;
            tr.localScale = Vector3.one;
        }

        public void F_StartChanged(bool val) {
            started = val;
            if (!started) {
                //Toggle_Select.isOn = false;
                if (redactedObjects != null) {
                    CorrectionsSerialization.serialize(redactedObjects.ToArray());
                }
                _setObjectTransformMarkerObject(null);
            }
            _transformMarker.SetActiveAxis(Axis.none);
            Toggle_Select.gameObject.SetActive(started);
            for (int i = 0; i < Ts.Length; i++)
                Ts[i].setReady(started);
        }

        private bool F_selectChanging = false;
        public void F_SelectChanged(bool val) {
            if (F_selectChanging)
                return;
            F_selectChanging = true;

            selected = val;
            if (selected) {
                if (_currentTRO == null) {
                    Toggle_Select.isOn = false;
                    selected = false;
                    F_selectChanging = false;
                    return;
                } else {
                    _chooseAxis(Axis.X);

                    if (!redactedObjects.Contains(_currentTRO.gameObject))
                        redactedObjects.Add(_currentTRO.gameObject);

                    //setObjectTransformMarkerObject (currentTRO.gameObject);
                }
                _transformMarker.SetActiveAxis(Axis.X);
                //XYZnum = 0;
            } else {
                _transformMarker.SetActiveAxis(Axis.none);
            }
            //else
            //setObjectTransformMarkerObject (null);
            //GameObject.Find ("Canvas/SceneName").GetComponent<UnityEngine.UI.Text> ().text = ObjectTransformMarker.transform.localScale.x.ToString();
            //--------
            m_axisSelection.gameObject.SetActive(selected);
            //--------
            m_positionManage.gameObject.SetActive(selected);
            m_rotationManage.gameObject.SetActive(selected);
            m_scaleManage.gameObject.SetActive(selected);

            F_selectChanging = false;
        }

        private void _initGui() {
            var TROGUI = transform.Find("TROGUI");
            //--------
            Toggle_Start = TROGUI.Find("OTBStart").GetComponent<Toggle>();
            Toggle_Start.isOn = started;
            Toggle_Start.gameObject.SetActive(true);
            Toggle_Start.onValueChanged.AddListener(F_StartChanged);
            Toggle_Select = TROGUI.Find("OTBSelect").GetComponent<Toggle>();
            Toggle_Select.isOn = selected;
            Toggle_Select.gameObject.SetActive(false);
            Toggle_Select.onValueChanged.AddListener(F_SelectChanged);
            //--------
            Button_X = m_axisSelection.Find("ButtonX").GetComponent<Button>();
            Button_X.onClick.AddListener(() => _chooseAxis(Axis.X));
            Button_Y = m_axisSelection.Find("ButtonY").GetComponent<Button>();
            Button_Y.onClick.AddListener(() => _chooseAxis(Axis.Y));
            Button_Z = m_axisSelection.Find("ButtonZ").GetComponent<Button>();
            Button_Z.onClick.AddListener(() => _chooseAxis(Axis.Z));

            m_axisSelection.gameObject.SetActive(false);

            // position
            m_positionManage.plusBtn.onClick.AddListener(
                () => _changeTransform(TransformField.positionLocal, _choosedAxis, true)
            );
            m_positionManage.minusBtn.onClick.AddListener(
                () => _changeTransform(TransformField.positionLocal, _choosedAxis, false)
            );
            m_positionManage.gameObject.SetActive(false);
            // rotation
            m_rotationManage.plusBtn.onClick.AddListener(
                () => _changeTransform(TransformField.rotationLocal, _choosedAxis, true)
            );
            m_rotationManage.minusBtn.onClick.AddListener(
                () => _changeTransform(TransformField.rotationLocal, _choosedAxis, false)
            );
            m_rotationManage.gameObject.SetActive(false);
            // scale
            m_scaleManage.plusBtn.onClick.AddListener(
                () => _changeTransform(TransformField.scaleLocal, _choosedAxis, true)
            );
            m_scaleManage.minusBtn.onClick.AddListener(
                () => _changeTransform(TransformField.scaleLocal, _choosedAxis, false)
            );
            m_scaleManage.gameObject.SetActive(false);
        }

        private void _chooseAxis(Axis p_axis) {
            _choosedAxis = p_axis;
            //Transform trm = currentTRO.transform;
            Button_X.interactable = (p_axis != Axis.X);
            Button_Y.interactable = (p_axis != Axis.Y);
            Button_Z.interactable = (p_axis != Axis.Z);

            _transformMarker.SetActiveAxis(p_axis);

            m_positionManage.infoText.text = _currentTRO.transform.GetValue(_choosedAxis, TransformField.positionLocal).ToString();
            m_rotationManage.infoText.text = _currentTRO.transform.GetValue(_choosedAxis, TransformField.rotationLocal).ToString();
            m_scaleManage.infoText.text = _currentTRO.transform.GetValue(_choosedAxis, TransformField.scaleLocal).ToString();
        }

        private void _changeTransform(TransformField field, Axis axis, bool positive) {
            string fieldInfo = "";
            float delta = 0;

            TROStepper fieldInfoFld = null;
            switch (field) {
                case TransformField.positionLocal:
                    fieldInfoFld = m_positionManage;
                    delta = 0.01f;
                    break;
                case TransformField.rotationLocal:
                    fieldInfoFld = m_rotationManage;
                    delta = 1f;
                    break;
                case TransformField.scaleLocal:
                    fieldInfoFld = m_scaleManage;
                    delta = 0.01f;
                    break;
            }
            delta *= positive ? 1 : -1;
            fieldInfo = _currentTRO.changeTransform(field, axis, delta).ToString();
            if (fieldInfoFld != null)
                fieldInfoFld.infoText.text = fieldInfo;
        }

        private void _loadSerialized() {
            CorrectionsSerialization.deserialize();
        }

        public void ExportChanges() {
            StartCoroutine(_DoExportChanges());
        }

        private IEnumerator _DoExportChanges () {
            if (TouchScreenKeyboard.visible) {
                yield break;
            }

            TouchScreenKeyboard kb = TouchScreenKeyboard.Open("/sdcard/", TouchScreenKeyboardType.Default, false);
            while (!kb.done && !kb.wasCanceled) {
                yield return null;
            }

            if (kb.done) {
                var fileLocation = kb.text;

                var originalPath = CorrectionsSerialization.k_serializePath;
                var destPath = System.IO.Path.Combine(fileLocation, CorrectionsSerialization.k_saveFileName);

                Debug.Log("copying " + originalPath + " to " + destPath);
                System.IO.File.Copy(originalPath, destPath);
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Import correction")]
        public static void Import() {
            var filePath = UnityEditor.EditorUtility.OpenFilePanel("choose mesh file", "", "");
            CorrectionsSerialization.deserialize(filePath);
        }
#endif
    }
}
