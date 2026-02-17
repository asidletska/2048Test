using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct MergeResult
    {
        public readonly int FromValue;
        public readonly int ToValue;
        public readonly Vector3 Position;
        public readonly CubeActor Winner;

        public MergeResult(int fromValue, int toValue, Vector3 position, CubeActor winner)
        {
            FromValue = fromValue;
            ToValue = toValue;
            Position = position;
            Winner = winner;
        }
    }

    public interface IMergeService
    {
        event Action<MergeResult> Merged;
        void Register(CubeCollision relay);
        void Unregister(CubeCollision relay);
    }

    public sealed class MergeService : IMergeService
    {
        public event Action<MergeResult> Merged;

        private readonly GameConfig _config;
        private readonly ICubeRegistry _registry;
        private readonly IPool<CubeActor> _cubePool;
        private readonly ICoroutineRunner _runner;

        private readonly Dictionary<ulong, float> _pairCooldownUntil = new(256);

        public MergeService(GameConfig config, ICubeRegistry registry, IPool<CubeActor> cubePool, ICoroutineRunner runner)
        {
            _config = config;
            _registry = registry;
            _cubePool = cubePool;
            _runner = runner;
        }

        public void Register(CubeCollision relay)
        {
            if (relay == null) return;
            relay.CollisionEntered += OnCollision;
        }

        public void Unregister(CubeCollision relay)
        {
            if (relay == null) return;
            relay.CollisionEntered -= OnCollision;
        }

        private void OnCollision(CubeActor a, Collision c)
        {
            if (a == null || a.IsMerging) return;

            var other = c.collider.GetComponentInParent<CubeActor>();
            if (other == null || other == a || other.IsMerging) return;

            // same value
            if (!a.Value.CanMergeWith(other.Value)) return;

            // pair anti-double
            var key = MakePairKey(a.GetInstanceID(), other.GetInstanceID());
            float now = Time.time;
            if (_pairCooldownUntil.TryGetValue(key, out var until) && now < until) return;
            _pairCooldownUntil[key] = now + _config.pairCooldown;

            // impulse directed to other (approach along contact direction)
            var contact = c.GetContact(0);
            var dir = (contact.point - a.Rigidbody.worldCenterOfMass).normalized;

            var relativeVelocity = a.Rigidbody.velocity - other.Rigidbody.velocity;
            var approachSpeed = Vector3.Dot(relativeVelocity, dir);

            // "impulse-like": speed * mass (stable and simple)
            var impactLike = approachSpeed * Mathf.Max(0.001f, a.Rigidbody.mass);
            if (impactLike < _config.minMergeImpulse) return;

            // choose winner: the cube that "pushed" into the other (positive approachSpeed)
            var winner = approachSpeed >= 0f ? a : other;
            var loser = winner == a ? other : a;

            if (winner.IsMerging || loser.IsMerging) return;

            _runner.Run(MergeRoutine(winner, loser, contact.point));
        }

        private IEnumerator MergeRoutine(CubeActor winner, CubeActor loser, Vector3 mergePoint)
        {
            winner.SetMerging(true);
            loser.SetMerging(true);

            // Freeze loser physics for clean absorb
            loser.Rigidbody.isKinematic = true;
            loser.Rigidbody.velocity = Vector3.zero;
            loser.Rigidbody.angularVelocity = Vector3.zero;

            // Smooth absorb (move+scale down)
            var startPos = loser.transform.position;
            var endPos = winner.transform.position;
            var startScale = loser.transform.localScale;

            float t = 0f;
            float dur = Mathf.Max(0.01f, _config.absorbDuration);

            while (t < dur)
            {
                t += Time.deltaTime;
                float k = t / dur;
                loser.transform.position = Vector3.Lerp(startPos, endPos, k);
                loser.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, k);
                yield return null;
            }

            // Apply new value
            int from = winner.Value.Value;
            int to = from * 2;
            winner.SetValue(new Po2Value(to));

            // cleanup loser
            _registry.Remove(loser);
            loser.transform.localScale = startScale; // reset before pool
            _cubePool.Release(loser);

            winner.SetMerging(false);

            Merged?.Invoke(new MergeResult(from, to, mergePoint, winner));
        }

        private static ulong MakePairKey(int idA, int idB)
        {
            uint a = (uint)Mathf.Min(idA, idB);
            uint b = (uint)Mathf.Max(idA, idB);
            return ((ulong)a << 32) | b;
        }
    }
