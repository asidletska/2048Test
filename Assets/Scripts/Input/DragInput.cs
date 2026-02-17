using System;
using UnityEngine;

public sealed class DragInput : MonoBehaviour, IInputSource
    {
        public event Action OnPress;
        public event Action<float> OnDragDeltaX;
        public event Action OnRelease;

        [field: SerializeField] public bool Enabled { get; set; } = true;

        private bool _pressed;
        private Vector2 _last;

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
                _last = t.position;
                OnPress?.Invoke();
                return;
            }

            if (!_pressed) return;

            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                var dx = t.position.x - _last.x;
                _last = t.position;
                if (Mathf.Abs(dx) > 0.01f) OnDragDeltaX?.Invoke(dx);
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
                _last = Input.mousePosition;
                OnPress?.Invoke();
                return;
            }

            if (!_pressed) return;

            if (Input.GetMouseButton(0))
            {
                var pos = (Vector2)Input.mousePosition;
                var dx = pos.x - _last.x;
                _last = pos;
                if (Mathf.Abs(dx) > 0.01f) OnDragDeltaX?.Invoke(dx);
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _pressed = false;
                OnRelease?.Invoke();
            }
        }
    }
