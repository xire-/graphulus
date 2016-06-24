using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour {
    void OnEnable() {
        GetComponent<Image>().color = GameSystem.Instance.Theme.nodeColor;
    }
}
