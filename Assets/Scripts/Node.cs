using UnityEngine;

public class Node : MonoBehaviour {
    public Springy.Node springyNode;

    private bool _selected;

    public bool Pinched { get; set; }

    public bool Selected {
        get { return _selected; }
        set {
            if (value && !_selected) {
                OnSelect();
            }
            else if (!value && _selected) {
                OnDeselect();
            }
            _selected = value;
        }
    }

    public string Text {
        get { return transform.Find("Text").GetComponent<TextMesh>().text; }
        set { transform.Find("Text").GetComponent<TextMesh>().text = value; }
    }

    private void LateUpdate() {
        const float scale = 30f;
        if (Pinched) {
            springyNode.Position = transform.localPosition * scale;
        }
        else {
            transform.localPosition = springyNode.Position / scale;
        }
    }

    private void OnSelect() {
        GetComponent<Renderer>().material.color = GameSystem.Instance.Theme.nodeSelectedColor;
        transform.Find("Text").localScale *= 2f;
    }

    private void OnDeselect() {
        GetComponent<Renderer>().material.color = GameSystem.Instance.Theme.nodeColor;
        transform.Find("Text").localScale /= 2f;
    }
}
