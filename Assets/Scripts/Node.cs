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
        var startColor = GetComponent<Renderer>().material.color;
        var endColor = GameSystem.Instance.Theme.nodeSelectedColor;
        var startScale = transform.Find("Text").localScale;
        var endScale = startScale * 2;
        GameSystem.Instance.GetComponent<GameSystem>().Animate(new Animation {
            Update = t => {
                GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
                transform.Find("Text").localScale = Vector3.Lerp(startScale, endScale, t);
            },
            duration = 0.3f,
            Ease = Easing.EaseOutCubic
        });
    }

    private void OnDeselect()
    {
        var startColor = GetComponent<Renderer>().material.color;
        var endColor = GameSystem.Instance.Theme.nodeColor;
        var startScale = transform.Find("Text").localScale;
        var endScale = startScale / 2;
        GameSystem.Instance.GetComponent<GameSystem>().Animate(new Animation
        {
            Update = t => {
                GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
                transform.Find("Text").localScale = Vector3.Lerp(startScale, endScale, t);
            },
            duration = 0.3f,
            Ease = Easing.EaseOutCubic
        });
    }
}
