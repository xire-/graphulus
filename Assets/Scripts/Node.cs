using UnityEngine;

public class Node : MonoBehaviour {
    public Springy.Node springyNode;

    private bool _currentlyAnimated;
    private Vector3 _initialScale;
    private bool _selected;

    public bool Pinched { get; set; }

    public bool Selected {
        get { return _selected; }
        set {
            if ((value && !_selected) || (!value && _selected)) {
                _selected = value;
                if (!_currentlyAnimated) {
                    AnimateSelection(value);
                }
            }
        }
    }

    public string Text {
        get { return transform.Find("Text").GetComponent<TextMesh>().text; }
        set { transform.Find("Text").GetComponent<TextMesh>().text = value; }
    }

    private void AnimateSelection(bool select) {
        var status = select ? 0f : 1f;

        var startColor = GameSystem.Instance.Theme.nodeColor;
        var endColor = GameSystem.Instance.Theme.nodeSelectedColor;

        var startScale = _initialScale;
        var endScale = _initialScale * 2f;
        endScale.z = 1;

        var duration = 0.3f;

        GameSystem.Instance.TestCor(new Test {
            OnStart = () => {
                _currentlyAnimated = true;
            },
            Update = (timeSinceStart, dt) => {
                float delta = dt / duration;
                if (!Selected) {
                    delta = -delta;
                }
                status += delta;

                var t = Mathf.Clamp(status, 0f, 1f);
                GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
                transform.Find("Text").localScale = Vector3.Lerp(startScale, endScale, t);

                bool exit1 = Selected && t == 1f;
                bool exit2 = !Selected && t == 0f;
                return !(exit1 || exit2);
            },
            OnEnd = () => {
                _currentlyAnimated = false;
            },
        });
    }

    private void Awake() {
        _initialScale = transform.Find("Text").localScale;
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
}
