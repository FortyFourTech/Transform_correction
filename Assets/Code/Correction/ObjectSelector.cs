using UnityEngine;

namespace Correction {
    /// <summary>
    /// Casts a ray in front of it. Finds all objects with TROObject component and any collider. Chooses the closest one.
    /// Usually it's convenient to attach it to the main camera.
    /// </summary>
    public class ObjectSelector : MonoBehaviour {
#region PUBLIC_PROPERTIES
        public bool HasSelection { get { return _currentSelection != null; } }
        public TROObject Selection { get { return _currentSelection; } }
        public bool LockMarker { get; set; }
#endregion

#region EDITOR_FIELDS
        [SerializeField] private GameObject _transformMarkPrefab;
#endregion

#region PRIVATE_FIELDS
        private TROMarker _transformMarker;
        private TROObject _currentSelection;
#endregion

#region UNITY_FUNCTIONS
        private void Start() {
            _transformMarker = (Instantiate(_transformMarkPrefab) as GameObject).GetComponent<TROMarker>();
            _transformMarker.SetParentObject(null);

            LockMarker = true;
        }

        private void Update() {
            _CheckVisibleTRO();

            _CheckMarkerLocation();
        }
#endregion

#region PUBLIC_FUNCTIONS
        public bool ShowMarker() {
            if (_currentSelection != null)
                _transformMarker.SetParentObject(_currentSelection.gameObject);
            
            return _currentSelection != null;
        }

        public void RemoveMarker() {
            _transformMarker.SetParentObject(null);
        }

        public void SetMarkerAxis(Axis axis) {
            _transformMarker.SetActiveAxis(axis);
        }

        public float ChangeObjectTransform(TransformField field, Axis axis, float delta) {
            return _currentSelection.ChangeTransform(field, axis, delta);
        }
#endregion

#region PRIVATE_FUNCTIONS
        private void _CheckVisibleTRO() {
            float closestDistance = float.MaxValue;
            TROObject closestTRO = null;

            var hits = Physics.RaycastAll(new Ray(transform.position, transform.forward));
            for (int i = 0; i < hits.Length; i++) {
                var hit = hits[i];
                var hitGO = hit.transform.gameObject;
                var hitObject = hitGO.GetComponentInParent<TROObject>();
                if (hitObject != null) {
                    var dist = hit.distance;
                    if ((closestTRO == null) || (dist < closestDistance)) {
                        closestDistance = dist;
                        closestTRO = hitObject;
                    }
                }
            }

            var newPossibleTRO = (closestTRO == null) ? null : closestTRO;

            if (_currentSelection != newPossibleTRO) {
                _currentSelection = newPossibleTRO;
            }
        }

        private void _CheckMarkerLocation () {
            if (!LockMarker) {
                if (_currentSelection != null) {
                    if (_currentSelection.transform != _transformMarker.transform.parent)
                        _transformMarker.SetParentObject(_currentSelection.gameObject);
                } else {
                    _transformMarker.SetParentObject(null);
                }
            }
        }
#endregion

    }
}