using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    private Dictionary<KeyCode, Action> _keyToActionMap = new Dictionary<KeyCode, Action>();

    private void Start() {
        _keyToActionMap.Add(KeyCode.L, GameSystem.Instance.SwitchTheme);
        _keyToActionMap.Add(KeyCode.R, GameSystem.Instance.ToggleEdgesActive);
        _keyToActionMap.Add(KeyCode.T, GameSystem.Instance.ToggleTextsActive);

        _keyToActionMap.Add(KeyCode.B, () => {
            GameSystem.Instance.AutoRotationSpeed += 10f;
        });
        _keyToActionMap.Add(KeyCode.V, () => {
            GameSystem.Instance.AutoRotationSpeed -= 10f;
        });
    }

    private void Update() {
        foreach (KeyCode keyCode in _keyToActionMap.Keys) {
            if (Input.GetKeyDown(keyCode)) {
                _keyToActionMap[keyCode]();
            }
        }
    }
}
