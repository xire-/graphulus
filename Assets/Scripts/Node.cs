using UnityEngine;

public class Node : MonoBehaviour {
    public Springy.Node springyNode;

    private bool _selected;

    public bool Pinched { get; set; }

    public bool Selectable { get; private set; }

    public bool Selected {
        get { return _selected; }
        set {
            if (Selectable) {
                if (value && !_selected) {
                    OnSelect();
                }
                else if (!value && _selected) {
                    OnDeselect();
                }
                _selected = value;
            }
        }
    }

    public string Text {
        get { return transform.Find("Text").GetComponent<TextMesh>().text; }
        set { transform.Find("Text").GetComponent<TextMesh>().text = value; }
    }

    private void Awake() {
        Selectable = true;
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
        SelectAnim(GameSystem.Instance.Theme.nodeSelectedColor, 2);
    }

    private void OnDeselect() {
        SelectAnim(GameSystem.Instance.Theme.nodeColor, 1f / 2f);
    }

    private void SelectAnim(Color endColor, float scaleFactor) {
        var startColor = GetComponent<Renderer>().material.color;
        var startScale = transform.Find("Text").localScale;
        var endScale = startScale * scaleFactor;
        GameSystem.Instance.GetComponent<GameSystem>().Animate(new Animation {
            OnStart = () => {
                Selectable = false;
            },
            Update = t => {
                GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
                transform.Find("Text").localScale = Vector3.Lerp(startScale, endScale, t);
            },
            OnEnd = () => {
                Selectable = true;
            },
            duration = 0.3f,
            Ease = Easing.EaseOutCubic
        });
    }
}
