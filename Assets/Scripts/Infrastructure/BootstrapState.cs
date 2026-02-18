using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public sealed class BootstrapState : IState
    {
        private readonly StateMachine _sm;
        private readonly IInputSource _input;
        private readonly IScoreService _score;

        public BootstrapState(StateMachine sm, IInputSource input, IScoreService score)
        {
            _sm = sm;
            _input = input;
            _score = score;
        }

        public void Enter()
        {
            _score.Reset();
            _input.Enabled = true;
            _sm.Set<SpawnState>();
        }

        public void Exit() { }
        public void Tick(float dt) { }
        public void FixedTick(float fdt) { }
    }

 public sealed class SpawnState : IState
    {
        private readonly StateMachine _sm;
        private readonly GameController _game;
        private readonly IInputSource _input;

        public SpawnState(StateMachine sm, GameController game, IInputSource input)
        {
            _sm = sm;
            _game = game;
            _input = input;
        }

        public void Enter()
        {
            _input.Enabled = true;
            _game.Spawn();
            _sm.Set<AimState>();
        }

        public void Exit() { }
        public void Tick(float dt) { }
        public void FixedTick(float fdt) { }
    }

    public sealed class AimState : IState
    {
        private readonly StateMachine _sm;
        private readonly GameController _game;
        private readonly IInputSource _input;

        private bool _aiming;

        public AimState(StateMachine sm, GameController game, IInputSource input)
        {
            _sm = sm;
            _game = game;
            _input = input;
        }

        public void Enter()
        {
            _aiming = false;
            _input.OnPress += OnPress;
            _input.OnDragDeltaX += OnDrag;
            _input.OnRelease += OnRelease;

            // Ensure we can drag immediately (even if press is missed)
            _game.BeginAim();
        }

        public void Exit()
        {
            _input.OnPress -= OnPress;
            _input.OnDragDeltaX -= OnDrag;
            _input.OnRelease -= OnRelease;
        }

        public void Tick(float dt) { }

        public void FixedTick(float fdt)
        {
            // Always update aim smoothing while in Aim state
            _game.FixedAimUpdate(fdt);
        }

        private void OnPress()
        {
            _aiming = true;
            _game.BeginAim();
        }

        private void OnDrag(float dx)
        {
            // If user starts dragging without a Began event, still allow aiming.
            if (!_aiming)
            {
                _aiming = true;
                _game.BeginAim();
            }

            _game.Drag(dx);
        }

        private void OnRelease()
        {
            if (!_aiming) return;
            _aiming = false;
            _sm.Set<LaunchState>();
        }
    }

    public sealed class LaunchState : IState
    {
        private readonly StateMachine _sm;
        private readonly GameController _game;
        private readonly IInputSource _input;

        public LaunchState(StateMachine sm, GameController game, IInputSource input)
        {
            _sm = sm;
            _game = game;
            _input = input;
        }

        public void Enter()
        {
            _input.Enabled = false;
            _game.Launch();
            _sm.Set<ResolveState>();
        }

        public void Exit() { }
        public void Tick(float dt) { }
        public void FixedTick(float fdt) { }
    }

    public sealed class ResolveState : IState
    {
        private readonly StateMachine _sm;
        private readonly GameController _game;

        public ResolveState(StateMachine sm, GameController game)
        {
            _sm = sm;
            _game = game;
        }

        public void Enter() { }
        public void Exit() { }

        public void Tick(float dt)
        {
            if (_game.IsSettled(dt))
                _sm.Set<SpawnState>();
        }

        public void FixedTick(float fdt) { }
    }
