using System;
using System.Collections;
using UnityEngine;

namespace ShadersMagic.AnimateScroll
{
    public class Scroll : MonoBehaviour
    {
        [SerializeField] private ScrollCameraSequence cameraSequence;
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float duration;

        private void Start()
        {
            mesh.sharedMaterial.SetFloat("_ScrolledLength", 1.88f);
            
            cameraSequence.Play(targetTransform);
            StartCoroutine(OnAnimationScrolling(false));
        }

        private IEnumerator OnAnimationScrolling(bool rollUp)
        {
            yield return new WaitForSeconds(0.5f);

            float t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                float normalized = t / duration;
                float length = rollUp
                    ? Mathf.Lerp(0, 1.88f, normalized)
                    : Mathf.Lerp(1.88f, 0, normalized);
                
                mesh.sharedMaterial.SetFloat("_ScrolledLength", length);
                yield return null;
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 100, 60), "Roll up"))
            {
                StartCoroutine(OnAnimationScrolling(true));
            }
            
            if (GUI.Button(new Rect(10, 80, 100, 60), "Unroll"))
            {
                StartCoroutine(OnAnimationScrolling(false));
            }
        }
    }
}

