using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    private bool visible = false;

	void Update () {
        var angle = Camera.main.transform.eulerAngles.x;
        if (angle >= 25 && angle <= 90) {
            if (!visible) {
                var v = Camera.main.transform.forward;
                v.y = 0;
                v.Normalize();
                v.y = -1f;
                transform.position = Camera.main.transform.position + v;

                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
                visible = true;
            }
            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, (angle - 25f) / (5f));
        }
        else
        {
            visible = false;
            transform.position = Vector3.one * 100;
        }
    }
}
