using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour {
    private Dictionary<KeyCode, Action> _keyToActionMap = new Dictionary<KeyCode, Action>();

    private void Start() {
        _keyToActionMap.Add(KeyCode.R, () => { GameSystem.Instance.EdgesActive = !GameSystem.Instance.EdgesActive; });

        _keyToActionMap.Add(KeyCode.T, () => { GameSystem.Instance.LabelsActive = !GameSystem.Instance.LabelsActive; });

        _keyToActionMap.Add(KeyCode.N, () => { GameSystem.Instance.AutoRotationEnabled = !GameSystem.Instance.AutoRotationEnabled; });
        _keyToActionMap.Add(KeyCode.B, () => { GameSystem.Instance.AutoRotationSpeed += 10f; });
        _keyToActionMap.Add(KeyCode.V, () => { GameSystem.Instance.AutoRotationSpeed -= 10f; });

        _keyToActionMap.Add(KeyCode.L, GameSystem.Instance.SwitchTheme);
    }

    private void Update() {
        foreach (KeyCode keyCode in _keyToActionMap.Keys) {
            if (Input.GetKeyDown(keyCode)) {
                _keyToActionMap[keyCode]();
            }
        }
    }
}
