using UnityEngine;

public class Node : MonoBehaviour
{
    public Springy.Node springyNode;

    private bool _selected;

    public bool Pinched { get; set; }

    public bool Selected
    {
        get { return _selected; }
        set
        {
            if (value && !_selected) {
                _selected = value;
                OnSelect();
            }
            else if (!value && _selected) {
                _selected = value;
                OnDeselect();
            }
        }
    }


    private Vector3 _initialScale;
    private void Awake() {
        _initialScale = transform.Find("Text").localScale;
    }

    public string Text
    {
        get { return transform.Find("Text").GetComponent<TextMesh>().text; }
        set { transform.Find("Text").GetComponent<TextMesh>().text = value; }
    }

    private void LateUpdate()
    {
        const float scale = 30f;
        if (Pinched)
        {
            springyNode.Position = transform.localPosition * scale;
        }
        else {
            transform.localPosition = springyNode.Position / scale;
        }
    }

    private void OnSelect()
    {
        if (!Animated) {
            var endscale = _initialScale * 2f;
            endscale.z = 1;
            SelectAnim(0f, GameSystem.Instance.Theme.nodeColor, GameSystem.Instance.Theme.nodeSelectedColor, _initialScale, endscale);
        }
    }

    private void OnDeselect()
    {
        if (!Animated) {
            var endscale = _initialScale * 2f;
            endscale.z = 1;
            SelectAnim(1f, GameSystem.Instance.Theme.nodeColor, GameSystem.Instance.Theme.nodeSelectedColor, _initialScale, endscale);
        }
    }

    public bool Animated { get; set; }

    private void SelectAnim(float status, Color startColor, Color endColor, Vector3 startScale, Vector3 endScale) {
        var duration = 0.3f;
        GameSystem.Instance.TestCor(new Test
        {
            OnStart = () => {
                Animated = true;
            },
            Update = (timeSinceStart, dt) =>
            {
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
                Animated = false;
            },
        });
    }
}
