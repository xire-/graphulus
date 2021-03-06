﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// https://forums.oculus.com/community/discussion/16710/new-unity-ui-ovr-look-based-input-howto
// slightly modified to add gaze input
public class LookInputModule : BaseInputModule {

    // name of axis to use for scrolling/sliders
    public string controlAxisName = "Horizontal";

    // the UI element to use for the cursor
    // the cursor will appear on the plane of the current UI element being looked at - so it adjusts to depth correctly
    // recommended to use a simple Image component (typical mouse cursor works pretty well) and you MUST add the
    // Unity created IgnoreRaycast component (script included in example) so that the cursor will not be see by the UI
    // event system
    public RectTransform cursor;

    // deselect when looking away from all UI elements
    // useful if you want to use axis for other controls
    public bool deselectWhenLookAway = false;

    public bool gazeEnabled;
    public float gazeTime;

    // ignore input when looking away from all UI elements
    // useful if you want to use buttons/axis for other controls
    public bool ignoreInputsWhenLookAway = true;

    // LookInputModule supports 2 modes:
    // 1 - Pointer
    //     Module acts a lot like a mouse with pointer locked where you look. Where you look is where
    //     pointerDown/pointerUp/pointerClick events are used
    //     useCursor is recommended for correct precision
    //     axis control of sliders/scrollbars/etc. is optional
    // 2 - Submit
    //     controls are selected and manipulated with axis control only
    //     submit/select events are used
    //     in this mode you can't click along a slider/scrollbar to set the slider/scroll value
    //     useLookDrag option is ignored
    public Mode mode = Mode.Pointer;

    public float normalCursorScale = 0.0005f;

    public bool scaleCursorWithDistance = true;

    public Color selectColor = Color.blue;

    // multiplier controls how fast slider/scrollbar moves with respect to input axis value
    public float smoothAxisMultiplier = 0.01f;

    // if useSmoothAxis is off, this next field controls how many steps per second are done when axis is on
    public float steppedAxisStepsPerSecond = 10f;

    // name of button to use for click/submit
    public string submitButtonName = "Fire1";

    // useCursor only applies when usePointerMethod is true
    // the cursor works like a mouse pointer so you can see exactly where you are clicking
    // not recommended to turn off
    public bool useCursor = true;

    // useLookDrag allows you to use look-based drag and drop (see example)
    // and also drag sliders/scrollbars based on where you are looking
    // only works if usePointerMethod is true
    public bool useLookDrag = true;

    public bool useLookDragScrollbar = false;

    public bool useLookDragSlider = true;

    // when UI element is selected this is the color it gets
    // useful for when want to use axis input to control sliders/scrollbars so you can see what is being
    // manipulated
    public bool useSelectColor = true;

    public bool useSelectColorOnButton = false;

    public bool useSelectColorOnToggle = false;

    // smooth axis - default UI move handlers do things in steps, meaning you can smooth scroll a slider or scrollbar
    // with axis control. This option allows setting value of scrollbar/slider directly as opposed to using move handler
    // to avoid this
    public bool useSmoothAxis = true;

    // singleton makes it easy to access the instanced fields from other code without needing a pointer
    // e.g.  if (LookInputModule.singleton != null && LookInputModule.singleton.controlAxisUsed) ...
    private static LookInputModule _singleton;

    // buttonUsed is helpful if you use same button elsewhere
    // you can use this boolean to see if the UI used the button press or not
    private bool _buttonUsed;

    // controlAxisUsed is helpful if you use same axis elsewhere
    // you can use this boolean to see if the UI used the axis control or not
    // if something is selected and takes move event, then this will be set
    private bool _controlAxisUsed;

    // guiRaycastHit is helpful if you have other places you want to use look input outside of UI system
    // you can use this to tell if the UI raycaster hit a UI element
    private bool _guiRaycastHit;

    private GameObject currentDragging;

    private GameObject currentLook;

    private float currentLookAtHandlerClickTime = 0f;
    private GameObject currentPressed;

    private Color currentSelectedHighlightedColor;

    private Color currentSelectedNormalColor;

    private bool currentSelectedNormalColorValid;

    // interal vars
    private PointerEventData lookData;

    private float nextAxisActionTime;

    public enum Mode { Pointer, Submit };

    public static LookInputModule singleton {
        get { return _singleton; }
    }

    public bool buttonUsed {
        get { return _buttonUsed; }
    }

    public bool controlAxisUsed {
        get { return _controlAxisUsed; }
    }

    public bool guiRaycastHit {
        get { return _guiRaycastHit; }
    }

    // clear the current selection
    public void ClearSelection() {
        if (eventSystem.currentSelectedGameObject) {
            RestoreColor(eventSystem.currentSelectedGameObject);
            eventSystem.SetSelectedGameObject(null);
        }
    }

    // Process is called by UI system to process events
    public override void Process() {
        _singleton = this;

        // send update events if there is a selected object - this is important for InputField to receive keyboard events
        SendUpdateEventToSelectedObject();

        // see if there is a UI element that is currently being looked at
        PointerEventData lookData = GetLookPointerEventData();
        var newLook = lookData.pointerCurrentRaycast.gameObject;
        if (newLook != currentLook) {
            currentLook = newLook;
            currentLookAtHandlerClickTime = Time.realtimeSinceStartup + gazeTime;
        }

        // deselect when look away
        if (deselectWhenLookAway && currentLook == null) {
            ClearSelection();
        }

        // handle enter and exit events (highlight)
        // using the function that is already defined in BaseInputModule
        HandlePointerExitAndEnter(lookData, currentLook);

        // update cursor
        UpdateCursor(lookData);

        if (!ignoreInputsWhenLookAway || ignoreInputsWhenLookAway && currentLook != null) {
            // button down handling
            _buttonUsed = false;
            if ((gazeEnabled && Time.realtimeSinceStartup > currentLookAtHandlerClickTime) || Input.GetButtonDown(submitButtonName)) {
                currentLookAtHandlerClickTime = float.PositiveInfinity;

                ClearSelection();
                lookData.pressPosition = lookData.position;
                lookData.pointerPressRaycast = lookData.pointerCurrentRaycast;
                lookData.pointerPress = null;
                if (currentLook != null) {
                    currentPressed = currentLook;
                    GameObject newPressed = null;
                    if (mode == Mode.Pointer) {
                        newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.pointerDownHandler);
                        if (newPressed == null) {
                            // some UI elements might only have click handler and not pointer down handler
                            newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.pointerClickHandler);
                            if (newPressed != null) {
                                currentPressed = newPressed;
                            }
                        }
                        else {
                            currentPressed = newPressed;
                            // we want to do click on button down at same time, unlike regular mouse processing
                            // which does click when mouse goes up over same object it went down on
                            // reason to do this is head tracking might be jittery and this makes it easier to click buttons
                            ExecuteEvents.Execute(newPressed, lookData, ExecuteEvents.pointerClickHandler);
                        }
                    }
                    else if (mode == Mode.Submit) {
                        newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.submitHandler);
                        if (newPressed == null) {
                            // try select handler instead
                            newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.selectHandler);
                        }
                    }
                    if (newPressed != null) {
                        lookData.pointerPress = newPressed;
                        currentPressed = newPressed;
                        Select(currentPressed);
                        _buttonUsed = true;
                    }
                    if (mode == Mode.Pointer) {
                        if (useLookDrag) {
                            bool useLookTest = true;
                            if (!useLookDragSlider && currentPressed.GetComponent<Slider>()) {
                                useLookTest = false;
                            }
                            else if (!useLookDragScrollbar && currentPressed.GetComponent<Scrollbar>()) {
                                useLookTest = false;
                                // the following is for scrollbars to work right
                                // apparently they go into an odd drag mode when pointerDownHandler is called
                                // a begin/end drag fixes that
                                if (ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.beginDragHandler)) {
                                    ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.endDragHandler);
                                }
                            }
                            if (useLookTest) {
                                ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.beginDragHandler);
                                lookData.pointerDrag = currentPressed;
                                currentDragging = currentPressed;
                            }
                        }
                        else if (currentPressed.GetComponent<Scrollbar>()) {
                            // the following is for scrollbars to work right
                            // apparently they go into an odd drag mode when pointerDownHandler is called
                            // a begin/end drag fixes that
                            if (ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.beginDragHandler)) {
                                ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.endDragHandler);
                            }
                        }
                    }
                }
            }
        }

        // have to handle button up even if looking away
        if (Input.GetButtonUp(submitButtonName)) {
            if (currentDragging) {
                ExecuteEvents.Execute(currentDragging, lookData, ExecuteEvents.endDragHandler);
                if (currentLook != null) {
                    ExecuteEvents.ExecuteHierarchy(currentLook, lookData, ExecuteEvents.dropHandler);
                }
                lookData.pointerDrag = null;
                currentDragging = null;
            }
            if (currentPressed) {
                ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.pointerUpHandler);
                lookData.rawPointerPress = null;
                lookData.pointerPress = null;
                currentPressed = null;
            }
        }

        // drag handling
        if (currentDragging != null) {
            ExecuteEvents.Execute(currentDragging, lookData, ExecuteEvents.dragHandler);
        }

        if (!ignoreInputsWhenLookAway || ignoreInputsWhenLookAway && currentLook != null) {
            // control axis handling
            _controlAxisUsed = false;
            if (eventSystem.currentSelectedGameObject && controlAxisName != null && controlAxisName != "") {
                float newVal = Input.GetAxis(controlAxisName);
                if (newVal > 0.01f || newVal < -0.01f) {
                    if (useSmoothAxis) {
                        Slider sl = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
                        if (sl != null) {
                            float mult = sl.maxValue - sl.minValue;
                            sl.value += newVal * smoothAxisMultiplier * mult;
                            _controlAxisUsed = true;
                        }
                        else {
                            Scrollbar sb = eventSystem.currentSelectedGameObject.GetComponent<Scrollbar>();
                            if (sb != null) {
                                sb.value += newVal * smoothAxisMultiplier;
                                _controlAxisUsed = true;
                            }
                        }
                    }
                    else {
                        _controlAxisUsed = true;
                        float time = Time.unscaledTime;
                        if (time > nextAxisActionTime) {
                            nextAxisActionTime = time + 1f / steppedAxisStepsPerSecond;
                            AxisEventData axisData = GetAxisEventData(newVal, 0.0f, 0.0f);
                            if (!ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisData, ExecuteEvents.moveHandler)) {
                                _controlAxisUsed = false;
                            }
                        }
                    }
                }
            }
        }
    }

    // use screen midpoint as locked pointer location, enabling look location to be the "mouse"
    private PointerEventData GetLookPointerEventData() {
        Vector2 lookPosition;
        lookPosition.x = Screen.width / 2;
        lookPosition.y = Screen.height / 2;
        if (lookData == null) {
            lookData = new PointerEventData(eventSystem);
        }
        lookData.Reset();
        lookData.delta = Vector2.zero;
        lookData.position = lookPosition;
        lookData.scrollDelta = Vector2.zero;
        eventSystem.RaycastAll(lookData, m_RaycastResultCache);
        lookData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        if (lookData.pointerCurrentRaycast.gameObject != null) {
            _guiRaycastHit = true;
        }
        else {
            _guiRaycastHit = false;
        }
        m_RaycastResultCache.Clear();
        return lookData;
    }

    // restore color of previously selected UI element
    private void RestoreColor(GameObject go) {
        if (useSelectColor && currentSelectedNormalColorValid) {
            Selectable s = go.GetComponent<Selectable>();
            if (s != null) {
                ColorBlock cb = s.colors;
                cb.normalColor = currentSelectedNormalColor;
                cb.highlightedColor = currentSelectedHighlightedColor;
                s.colors = cb;
            }
        }
    }

    // select a game object
    private void Select(GameObject go) {
        ClearSelection();
        if (ExecuteEvents.GetEventHandler<ISelectHandler>(go)) {
            SetSelectedColor(go);
            eventSystem.SetSelectedGameObject(go);
        }
    }

    // send update event to selected object
    // needed for InputField to receive keyboard input
    private bool SendUpdateEventToSelectedObject() {
        if (eventSystem.currentSelectedGameObject == null) {
            return false;
        }
        BaseEventData data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    // sets color of selected UI element and saves current color so it can be restored on deselect
    private void SetSelectedColor(GameObject go) {
        if (useSelectColor) {
            if (!useSelectColorOnButton && go.GetComponent<Button>()) {
                currentSelectedNormalColorValid = false;
                return;
            }
            if (!useSelectColorOnToggle && go.GetComponent<Toggle>()) {
                currentSelectedNormalColorValid = false;
                return;
            }
            Selectable s = go.GetComponent<Selectable>();
            if (s != null) {
                ColorBlock cb = s.colors;
                currentSelectedNormalColor = cb.normalColor;
                currentSelectedNormalColorValid = true;
                currentSelectedHighlightedColor = cb.highlightedColor;
                cb.normalColor = selectColor;
                cb.highlightedColor = selectColor;
                s.colors = cb;
            }
        }
    }

    // update the cursor location and whether it is enabled
    // this code is based on Unity's DragMe.cs code provided in the UI drag and drop example
    private void UpdateCursor(PointerEventData lookData) {
        if (cursor != null) {
            if (useCursor) {
                if (lookData.pointerEnter != null) {
                    RectTransform draggingPlane = lookData.pointerEnter.GetComponent<RectTransform>();
                    Vector3 globalLookPos;
                    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, lookData.position, lookData.enterEventCamera, out globalLookPos)) {
                        cursor.gameObject.SetActive(true);
                        cursor.position = globalLookPos;
                        cursor.rotation = draggingPlane.rotation;
                        if (scaleCursorWithDistance) {
                            // scale cursor with distance
                            float lookPointDistance = (globalLookPos - lookData.enterEventCamera.transform.position).magnitude;
                            float cursorScale = lookPointDistance * normalCursorScale;
                            if (cursorScale < normalCursorScale) {
                                cursorScale = normalCursorScale;
                            }
                            Vector3 cursorScaleVector;
                            cursorScaleVector.x = cursorScale;
                            cursorScaleVector.y = cursorScale;
                            cursorScaleVector.z = cursorScale;
                            cursor.localScale = cursorScaleVector;
                        }
                    }
                    else {
                        cursor.gameObject.SetActive(false);
                    }
                }
                else {
                    cursor.gameObject.SetActive(false);
                }
            }
            else {
                cursor.gameObject.SetActive(false);
            }
        }
    }
}
