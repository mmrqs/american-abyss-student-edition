using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class InputAxisEvent : UnityEvent<float>
{
}

[System.Serializable]
public class KeyEvent
{
    [SerializeField] protected KeyCode key = KeyCode.A;
    [SerializeField] public KeyCode Key { get { return key; } }
    [SerializeField] public UnityEvent onKeyDown = new UnityEvent();
    [SerializeField] public UnityEvent onKeyUp = new UnityEvent();
    [SerializeField] public UnityEvent onKey = new UnityEvent();

    public void AddEvent(PressType press, UnityAction callback)
    {
        switch (press)
        {
            case PressType.DOWN:
                onKeyDown.AddListener(callback);
                break;
            case PressType.UP:
                onKeyUp.AddListener(callback);
                break;
            case PressType.HOLD:
                onKey.AddListener(callback);
                break;
        }
    }
    public void RemoveEvent(PressType press, UnityAction callback)
    {
        switch (press)
        {
            case PressType.DOWN:
                onKeyDown.RemoveListener(callback);
                break;
            case PressType.UP:
                onKeyUp.RemoveListener(callback);
                break;
            case PressType.HOLD:
                onKey.RemoveListener(callback);
                break;
        }
    }

    public void Process()
    {
        if (Input.GetKey(key))
        {
            onKey.Invoke();
        }
        if (Input.GetKeyDown(key))
        {
            onKeyDown.Invoke();
        }
        if (Input.GetKeyUp(key))
        {
            onKeyUp.Invoke();
        }
    }

    public KeyEvent(KeyCode key = KeyCode.A)
    {
        this.key = key;
    }
}

public enum PressType
{
    DOWN,
    UP,
    HOLD
}

public enum MouseButtonType
{
    LEFT_BUTTON,
    RIGHT_BUTTON,
    MIDDLE_BUTTON
}
[System.Serializable]
public class MouseButtonEvent
{
    [HideInInspector] protected int button = 0;
    [SerializeField] public int Button { get { return button; } }
    [SerializeField] public UnityEvent onButtonDown = new UnityEvent();
    [SerializeField] public UnityEvent onButtonUp = new UnityEvent();
    [SerializeField] public UnityEvent onButton = new UnityEvent();

    public void AddEvent(PressType press, UnityAction callback)
    {
        switch (press)
        {
            case PressType.DOWN:
                onButtonDown.AddListener(callback);
                break;
            case PressType.UP:
                onButtonUp.AddListener(callback);
                break;
            case PressType.HOLD:
                onButton.AddListener(callback);
                break;
        }
    }
    public void RemoveEvent(PressType press, UnityAction callback)
    {
        switch (press)
        {
            case PressType.DOWN:
                onButtonDown.RemoveListener(callback);
                break;
            case PressType.UP:
                onButtonUp.RemoveListener(callback);
                break;
            case PressType.HOLD:
                onButton.RemoveListener(callback);
                break;
        }
    }
    public void Process()
    {
        if (Input.GetMouseButton(button))
        {
            onButton.Invoke();
        }
        if (Input.GetMouseButtonDown(button))
        {
            onButtonDown.Invoke();
        }
        if (Input.GetMouseButtonUp(button))
        {
            onButtonUp.Invoke();
        }
    }

    public MouseButtonEvent(int button = 0)
    {
        this.button = button;
    }
    public MouseButtonEvent(MouseButtonType button = 0)
    {
        this.button = (int)button;
    }
}

[System.Serializable]
public class AxisEvent
{
    [SerializeField] protected string axis = "Horizontal";
    [SerializeField] public string Axis { get { return axis; } }
    [SerializeField] protected bool raw = false;
    [SerializeField] public InputAxisEvent onAxisEvent = new InputAxisEvent();

    public void Process()
    {
        if (raw)
        {
            onAxisEvent.Invoke(Input.GetAxisRaw(axis));
        }
        else
        {
            onAxisEvent.Invoke(Input.GetAxis(axis));
        }
    }

    public AxisEvent(string axis = "", bool raw = false)
    {
        this.axis = axis;
        this.raw = raw;
    }
}


[System.Serializable]
public class MouseEvent
{
    [SerializeField] public MouseButtonEvent leftButtonEvents = new MouseButtonEvent(MouseButtonType.LEFT_BUTTON);
    [SerializeField] public MouseButtonEvent rightButtonEvents = new MouseButtonEvent(MouseButtonType.RIGHT_BUTTON);
    [SerializeField] public MouseButtonEvent middleButtonEvents = new MouseButtonEvent(MouseButtonType.MIDDLE_BUTTON);
    [SerializeField] public AxisEvent scrollEvents = new AxisEvent("Mouse ScrollWheel", true);

    public void AddEvent(MouseButtonType mouseButton, PressType press, UnityAction callback)
    {
        switch (mouseButton)
        {
            case MouseButtonType.LEFT_BUTTON:
                leftButtonEvents.AddEvent(press, callback);
                break;
            case MouseButtonType.RIGHT_BUTTON:
                rightButtonEvents.AddEvent(press, callback);
                break;
            case MouseButtonType.MIDDLE_BUTTON:
                middleButtonEvents.AddEvent(press, callback);
                break;
        }
    }
    public void RemoveEvent(MouseButtonType mouseButton, PressType press, UnityAction callback)
    {
        switch (mouseButton)
        {
            case MouseButtonType.LEFT_BUTTON:
                leftButtonEvents.RemoveEvent(press, callback);
                break;
            case MouseButtonType.RIGHT_BUTTON:
                rightButtonEvents.RemoveEvent(press, callback);
                break;
            case MouseButtonType.MIDDLE_BUTTON:
                middleButtonEvents.RemoveEvent(press, callback);
                break;
        }
    }
    public void Process()
    {
        leftButtonEvents.Process();
        rightButtonEvents.Process();
        middleButtonEvents.Process();
        scrollEvents.Process();
    }
}

public class InputManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField]
    public MouseEvent mouseEvents = new MouseEvent();
    [SerializeField]
    public List<KeyEvent> keyEvents = new List<KeyEvent>();
    [SerializeField]
    public List<AxisEvent> axisEvents = new List<AxisEvent>();

    // Update is called once per frame
    void Update()
    {
        mouseEvents.Process();
        foreach (KeyEvent item in keyEvents)
        {
            item.Process();
        }
        foreach (AxisEvent item in axisEvents)
        {
            item.Process();
        }
    }

    private static InputManager instance = null;
    public static InputManager Instance
    {
        get
        {
            if (!instance)
            {
                Debug.LogException(new MissingReferenceException("InputManager not initialized. Be careful to ask for instance AFTER Awake method."));
            }
            return instance;
        }
        set
        {
            if (instance != null)
                Debug.LogException(new System.Exception("More than one InputManager in the scene. Please remove the excess"));
            instance = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private KeyEvent FindKeyEvent(KeyCode key)
    {
        KeyEvent evt = keyEvents.Find(keyEvent => { return keyEvent.Key == key; });
        if (evt == null)
        {
            evt = new KeyEvent(key);
            keyEvents.Add(evt);
        }
        return evt;
    }

    private AxisEvent FindAxisEvent(string axis)
    {
        AxisEvent evt = axisEvents.Find(axisEvent => { return axisEvent.Axis == axis; });
        if (evt == null)
        {
            evt = new AxisEvent(axis);
            axisEvents.Add(evt);
        }
        return evt;
    }

    public void AddKeyEvent(KeyCode key, PressType press, UnityAction callback)
    {
        KeyEvent evt = FindKeyEvent(key);

        evt.AddEvent(press, callback);
    }
    public void RemoveKeyEvent(KeyCode key, PressType press, UnityAction callback)
    {
        KeyEvent evt = keyEvents.Find(keyEvent => { return keyEvent.Key == key; });

        if (evt == null)
            throw new Exception("no event for key " + key.ToString() + " can't remove listener.");
        evt.RemoveEvent(press, callback);
    }

    public void AddAxisEvent(string axis, UnityAction<float> callback)
    {
        AxisEvent evt = FindAxisEvent(axis);

        evt.onAxisEvent.AddListener(callback);
    }
    public void RemoveAxisEvent(string axis, UnityAction<float> callback)
    {
        AxisEvent evt = axisEvents.Find(axisEvent => { return axisEvent.Axis == axis; });

        if (evt == null)
            throw new Exception("no event for axis " + axis + " can't remove listener.");
        evt.onAxisEvent.RemoveListener(callback);
    }

    public void AddMouseButtonEvent(MouseButtonType mouseButton, PressType press, UnityAction callback)
    {
        mouseEvents.AddEvent(mouseButton, press, callback);
    }
    public void RemoveMouseButtonEvent(MouseButtonType mouseButton, PressType press, UnityAction callback)
    {
        mouseEvents.RemoveEvent(mouseButton, press, callback);
    }

    public void AddMouseScrollWheelEvent(UnityAction<float> callback)
    {
        mouseEvents.scrollEvents.onAxisEvent.AddListener(callback);
    }
    public void RemoveMouseScrollWheelEvent(UnityAction<float> callback)
    {
        mouseEvents.scrollEvents.onAxisEvent.RemoveListener(callback);
    }
}