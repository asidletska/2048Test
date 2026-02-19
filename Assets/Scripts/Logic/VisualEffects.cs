using System.Collections;
using UnityEngine;

public sealed class VisualEffects : IMergeFxPlayer
    {
        private readonly ICoroutineRunner _runner;
        private readonly ParticleSystem _mergePrefab;
        private readonly IPool<ParticleSystem> _fxPool;

        public VisualEffects(ICoroutineRunner runner, ParticleSystem mergePrefab, IPool<ParticleSystem> fxPool)
        {
            _runner = runner;
            _mergePrefab = mergePrefab;
            _fxPool = fxPool;
        }

        public void PlayMergeFx(Vector3 position)
        {
            if (_mergePrefab == null || _fxPool == null) return;

            var ps = _fxPool.Get();
            ps.transform.position = position;
            ps.Play(true);

            _runner.Run(ReturnWhenDone(ps));
        }

        public void PlayPop(Transform target, float popScale, float duration)
        {
            if (target == null) return;
            _runner.Run(PopRoutine(target, popScale, duration));
        }

        private IEnumerator ReturnWhenDone(ParticleSystem ps)
        {
            while (ps != null && ps.IsAlive(true))
                yield return null;

            if (ps != null)
                _fxPool.Release(ps);
        }

        private static IEnumerator PopRoutine(Transform t, float popScale, float duration)
        {
            var start = t.localScale;
            var peak = start * popScale;

            float half = Mathf.Max(0.01f, duration) * 0.5f;

            float time = 0f;
            while (time < half)
            {
                time += Time.deltaTime;
                float k = time / half;
                t.localScale = Vector3.Lerp(start, peak, k);
                yield return null;
            }

            time = 0f;
            while (time < half)
            {
                time += Time.deltaTime;
                float k = time / half;
                t.localScale = Vector3.Lerp(peak, start, k);
                yield return null;
            }

            t.localScale = start;
        }
    }