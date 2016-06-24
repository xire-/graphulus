using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour {

    private void OnEnable() {
        GetComponent<Image>().color = GameSystem.Instance.Theme.nodeColor;
    }
}
