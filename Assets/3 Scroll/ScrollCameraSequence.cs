using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadersMagic.AnimateScroll
{
    public class ScrollCameraSequence : MonoBehaviour
    {
        [Header("Camera settings")] 
        [SerializeField] private Vector3 startOffset = new Vector3(0f, 2f, -5f);
        [SerializeField] private Vector3 endOffset = new Vector3(0f, 5f, -1f);
        
        [Header("Timing")]
        [SerializeField] private float duration = 1.5f;
        [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Extra Motion")]
        [SerializeField] private float heightArc = 1.0f;
        
        private Coroutine _coroutine;
        private Transform _camera;
        private Transform _target;

        private void Awake()
        {
            var camera = Camera.main;
            _camera = camera.transform;
        }
        
        public void Play(Transform target)
        {
            _target = target;
            if (_coroutine != null) StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(AnimateCamera());
        }

        private IEnumerator AnimateCamera()
        {
            yield return new WaitForSeconds(0.1f);
            
            Vector3 startPos = _target.TransformPoint(startOffset);
            Vector3 endPos = _target.TransformPoint(endOffset);
            
            Quaternion start = Quaternion.LookRotation(_target.position - startPos);
            Quaternion end = Quaternion.LookRotation(_target.position - endPos);

            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(t / duration);
                float eased = easing.Evaluate(normalizedTime);
                
                Vector3 pos = Vector3.Lerp(startPos, endPos, eased);
                pos.y += Mathf.Sin(eased * Mathf.PI) * heightArc;
                _camera.position = pos;
                
                Quaternion rotation = Quaternion.Slerp(start, end, eased);
                _camera.rotation = rotation;
                
                yield return null;
            }
            
            _camera.position = endPos;
            _camera.rotation = end;
        }
    }
}
