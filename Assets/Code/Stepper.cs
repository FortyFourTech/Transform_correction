using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI {
    public class Stepper : MonoBehaviour {
        [SerializeField] private Text _infoText;
        [SerializeField] private Button _plusBtn;
        [SerializeField] private Button _minusBtn;

        public void SetText(string text) {
            _infoText.text = text;
        }

        public void AddPlusAction(Events.UnityAction action) {
            _plusBtn.onClick.AddListener(action);
        }

        public void AddMinusAction(Events.UnityAction action) {
            _plusBtn.onClick.AddListener(action);
        }

        public void RemovePlusAction(Events.UnityAction action) {
            _plusBtn.onClick.RemoveListener(action);
        }

        public void RemoveMinusAction(Events.UnityAction action) {
            _plusBtn.onClick.RemoveListener(action);
        }

        private void OnDestroy() {
            _plusBtn.onClick.RemoveAllListeners();
            _minusBtn.onClick.RemoveAllListeners();
        }
    }
}
