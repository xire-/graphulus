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
        // set the direction of the animation
        var t = select ? 0f : 1f;

        var startColor = GameSystem.Instance.Theme.nodeColor;
        var endColor = GameSystem.Instance.Theme.nodeSelectedColor;

        var startScale = _initialScale;
        var endScale = _initialScale * 2f;
        endScale.z = 1;

        const float duration = 0.2f;
        GameSystem.Instance.AnimateConditional(new AnimationConditional {
            OnStart = () => {
                _currentlyAnimated = true;
            },
            Update = (deltaTime) => {
                float delta = Mathf.Clamp01(deltaTime / duration);

                // if node is deselected, invert the animation
                if (!Selected) {
                    delta = -delta;
                }
                t = Mathf.Clamp01(t + delta);

                // interpolate
                GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
                transform.Find("Text").localScale = Vector3.Lerp(startScale, endScale, t);

                // continue animation until completion
                bool selectedAndCompleted = Selected && t >= 1f;
                bool deselectedAndCompleted = !Selected && t <= 0f;
                return !(selectedAndCompleted || deselectedAndCompleted);
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
        if (Pinched) {
            springyNode.Position = transform.localPosition * GameSystem.Instance.graph.Scale;
        }
        else {
            transform.localPosition = springyNode.Position / GameSystem.Instance.graph.Scale;
        }
    }
}
