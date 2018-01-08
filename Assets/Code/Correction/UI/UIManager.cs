﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Correction;

namespace Correction.UI {
    public class UIManager : MonoBehaviour {

#region EDITOR_FIELDS
        [SerializeField] private Toggle _startToggle, _selectToggle;
        
        [SerializeField] private AxisSelector _axisSelector;

        [SerializeField] private Stepper _positionStepper, _rotationStepper, _scaleStepper;

        [Space]
        [SerializeField] private ObjectSelector _objectSelector;
#endregion

#region PRIVATE_FIELDS
        private State _state = State.off;

        private Axis _choosedAxis = Axis.none;

        private List<GameObject> _redactedObjects = new List<GameObject>();
#endregion

#region UNITY_FUNCTIONS
        private void Start() {
            _InitGui();

            _LoadSerialized();
        }
#endregion

#region PUBLIC_FUNCTIONS
        public void ExportChanges() {
            StartCoroutine(_DoExportChanges());
        }
#endregion

#region BUTTON_FUNCTIONS
        private void _OnStartChanged(bool newValue) {
            if (!newValue) {
                _ChangeState(State.off);
                
                if (_redactedObjects != null) {
                    CorrectionsSerialization.serialize(_redactedObjects.ToArray());
                }
            } else {
                _ChangeState(State.started);
            }
        }

        private void _OnSelectChanged(bool newValue) {
            if (newValue) {
                if (!_objectSelector.HasSelection) {
                    _ChangeState(State.started);
                } else {
                    _ChangeState(State.selected);

                    var selection = _objectSelector.Selection;
                    if (!_redactedObjects.Contains(selection.gameObject))
                        _redactedObjects.Add(selection.gameObject);
                }
            } else {
                _ChangeState(State.started);
            }
        }

        private void _ChooseAxis(Axis axis) {
            _choosedAxis = axis;

            _axisSelector.SetActiveAxis(axis);

            _objectSelector.SetMarkerAxis(axis);

            var currentSelection = _objectSelector.Selection;
            if (axis != Axis.none) {
                _positionStepper.SetText(currentSelection.transform.GetValue(_choosedAxis, TransformField.positionLocal).ToString());
                _rotationStepper.SetText(currentSelection.transform.GetValue(_choosedAxis, TransformField.rotationLocal).ToString());
                _scaleStepper.SetText(currentSelection.transform.GetValue(_choosedAxis, TransformField.scaleLocal).ToString());
            }
        }

        private void _ChangeTransform(TransformField field, Axis axis, bool positive) {
            string fieldInfo = "";
            float delta = 0;

            Stepper fieldInfoFld = null;
            switch (field) {
                case TransformField.positionLocal:
                    fieldInfoFld = _positionStepper;
                    delta = 0.01f;
                    break;
                case TransformField.rotationLocal:
                    fieldInfoFld = _rotationStepper;
                    delta = 1f;
                    break;
                case TransformField.scaleLocal:
                    fieldInfoFld = _scaleStepper;
                    delta = 0.01f;
                    break;
            }
            delta *= positive ? 1 : -1;
            fieldInfo = _objectSelector.ChangeObjectTransform(field, axis, delta).ToString();
            if (fieldInfoFld != null)
                fieldInfoFld.SetText(fieldInfo);
        }
#endregion

#region PRIVATE_FUNCTIONS
        private void _InitGui() {
            _startToggle.isOn = false;
            _startToggle.onValueChanged.AddListener(_OnStartChanged);
            _startToggle.gameObject.SetActive(true);

            _selectToggle.isOn = false;
            _selectToggle.onValueChanged.AddListener(_OnSelectChanged);
            _selectToggle.gameObject.SetActive(false);

            _axisSelector.AddAxisBtnAction(Axis.X, () => _ChooseAxis(Axis.X));
            _axisSelector.AddAxisBtnAction(Axis.Y, () => _ChooseAxis(Axis.Y));
            _axisSelector.AddAxisBtnAction(Axis.Z, () => _ChooseAxis(Axis.Z));
            _axisSelector.gameObject.SetActive(false);

            _positionStepper.AddPlusAction(
                () => _ChangeTransform(TransformField.positionLocal, _choosedAxis, true)
            );
            _positionStepper.AddMinusAction(
                () => _ChangeTransform(TransformField.positionLocal, _choosedAxis, false)
            );
            _positionStepper.gameObject.SetActive(false);

            _rotationStepper.AddPlusAction(
                () => _ChangeTransform(TransformField.rotationLocal, _choosedAxis, true)
            );
            _rotationStepper.AddMinusAction(
                () => _ChangeTransform(TransformField.rotationLocal, _choosedAxis, false)
            );
            _rotationStepper.gameObject.SetActive(false);

            _scaleStepper.AddPlusAction(
                () => _ChangeTransform(TransformField.scaleLocal, _choosedAxis, true)
            );
            _scaleStepper.AddMinusAction(
                () => _ChangeTransform(TransformField.scaleLocal, _choosedAxis, false)
            );
            _scaleStepper.gameObject.SetActive(false);
        }

        private void _ChangeState(State newState) {
            _state = newState;

            bool started, selected;
            switch (_state) {
                case State.off:
                    started = false;
                    selected = false;

                    _objectSelector.LockMarker = true;
                    _objectSelector.RemoveMarker();
                    _ChooseAxis(Axis.none);
                    
                    break;
                case State.started:
                    started = true;
                    selected = false;

                    _objectSelector.LockMarker = false;
                    _ChooseAxis(Axis.none);
                    
                    break;
                case State.selected:
                    started = true;
                    selected = true;

                    _objectSelector.LockMarker = true;
                    _objectSelector.ShowMarker();
                    _ChooseAxis(Axis.X);

                    break;                    
                default:
                    throw new System.Exception("unknown TROManager state");
            }

            // _startToggle.isOn = _started;
            _selectToggle.onValueChanged.RemoveAllListeners(); // this is for callback not being called
            _selectToggle.isOn = selected;
            _selectToggle.onValueChanged.AddListener(_OnSelectChanged);
            _selectToggle.gameObject.SetActive(started);

            _axisSelector.gameObject.SetActive(selected);

            _positionStepper.gameObject.SetActive(selected);
            _rotationStepper.gameObject.SetActive(selected);
            _scaleStepper.gameObject.SetActive(selected);
        }

        private void _LoadSerialized() {
            CorrectionsSerialization.deserialize();
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
#endregion
    
        enum State {
            off, started, selected
        }
    }
}
