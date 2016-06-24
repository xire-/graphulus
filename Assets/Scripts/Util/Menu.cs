using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    private Transform _panelTransform;

    public void ChangeAutoRotationEnabled() {
        GameSystem.Instance.AutoRotationEnabled = _panelTransform.Find("ToggleAutoRotationEnabled").GetComponent<Toggle>().isOn;
        _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().interactable = GameSystem.Instance.AutoRotationEnabled;
    }

    public void ChangeAutoRotationSpeed() {
        GameSystem.Instance.AutoRotationSpeed = _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().value;
    }

    public void ChangeEdgesActive() {
        GameSystem.Instance.EdgesActive = _panelTransform.Find("ToggleEdgesActive").GetComponent<Toggle>().isOn;
    }

    public void ChangeGraphScale() {
        GameSystem.Instance.graph.Scale = _panelTransform.Find("SliderGraphScale").GetComponent<Slider>().value;
    }

    public void ChangeTextsActive() {
        GameSystem.Instance.EdgesActive = _panelTransform.Find("ToggleTextsActive").GetComponent<Toggle>().isOn;
    }

    public void SwitchTheme() {
        GameSystem.Instance.SwitchTheme();
    }

    private void Start() {
        _panelTransform = transform.Find("Panel");

        _panelTransform.Find("ToggleEdgesActive").GetComponent<Toggle>().isOn = GameSystem.Instance.EdgesActive;

        _panelTransform.Find("ToggleTextsActive").GetComponent<Toggle>().isOn = GameSystem.Instance.TextsActive;

        _panelTransform.Find("ToggleAutoRotationEnabled").GetComponent<Toggle>().isOn = GameSystem.Instance.AutoRotationEnabled;

        _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().interactable = GameSystem.Instance.AutoRotationEnabled;
        _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().value = GameSystem.Instance.AutoRotationSpeed;

        _panelTransform.Find("SliderGraphScale").GetComponent<Slider>().value = GameSystem.Instance.graph.Scale;
    }

    private void Update() {
        var angle = Camera.main.transform.eulerAngles.x;
        if (angle >= 25 && angle <= 90) {
            if (!_panelTransform.gameObject.activeSelf) {
                _panelTransform.gameObject.SetActive(true);

                // move the menu in front of the camera
                var vec = Camera.main.transform.forward;
                vec.y = 0;
                vec.Normalize();
                vec.y = -1f;
                transform.position = Camera.main.transform.position + vec;
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
            }

            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, (angle - 25f) / (5f));
        }
        else {
            if (_panelTransform.gameObject.activeSelf) {
                _panelTransform.gameObject.SetActive(false);
            }
        }
    }
}
