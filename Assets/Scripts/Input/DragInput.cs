using System;
using UnityEngine;

public sealed class DragInput : MonoBehaviour, IInputSource
    {
      public event Action OnPress;
    public event Action<float> OnDragDeltaX; 
    public event Action OnRelease;

    [SerializeField] private float dragPlaneZ = 0f;

    public bool Enabled { get; set; } = true;

    private bool _pressed;
    private float _lastWorldX;
    private Camera _cam;

    private void OnEnable()
    {
        ResetInputState();
    }

    private void Awake()
    {
        ResetInputState();
    }

    public void ResetInputState()
    {
        Enabled = true;
        _pressed = false;
        _lastWorldX = 0f;
        _cam = Camera.main;
    }

    private void Update()
    {
        if (!Enabled) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    private void HandleTouch()
    {
        if (Input.touchCount <= 0) return;
        var t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            _pressed = true;

            if (TryGetWorldX(t.position, out var wx))
                _lastWorldX = wx;

            OnPress?.Invoke();
            return;
        }

        if (!_pressed) return;

        if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
        {
            if (TryGetWorldX(t.position, out var wx))
            {
                float dx = wx - _lastWorldX;     
                _lastWorldX = wx;

                if (Mathf.Abs(dx) > 0.0001f)
                    OnDragDeltaX?.Invoke(dx);
            }
            return;
        }

        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            _pressed = false;
            OnRelease?.Invoke();
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _pressed = true;

            if (TryGetWorldX(Input.mousePosition, out var wx))
                _lastWorldX = wx;

            OnPress?.Invoke();
            return;
        }

        if (!_pressed) return;

        if (Input.GetMouseButton(0))
        {
            if (TryGetWorldX(Input.mousePosition, out var wx))
            {
                float dx = wx - _lastWorldX;    
                _lastWorldX = wx;

                if (Mathf.Abs(dx) > 0.0001f)
                    OnDragDeltaX?.Invoke(dx);
            }
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _pressed = false;
            OnRelease?.Invoke();
        }
    }

    private bool TryGetWorldX(Vector2 screenPos, out float worldX)
    {
        if (_cam == null) _cam = Camera.main;
        if (_cam == null)
        {
            worldX = 0f;
            return false;
        }

        Ray ray = _cam.ScreenPointToRay(screenPos);

        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, dragPlaneZ));

        if (plane.Raycast(ray, out float enter))
        {
            worldX = ray.GetPoint(enter).x;
            return true;
        }

        worldX = 0f;
        return false;
    }
    }
