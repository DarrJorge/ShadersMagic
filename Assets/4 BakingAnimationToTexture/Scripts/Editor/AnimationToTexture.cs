using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ShadersMagic.BakingAnimationToTexture
{
    public class AnimationToTexture : EditorWindow
    {
        private SkinnedMeshRenderer _context;
        private AnimationClip _clip;
        private static int _frameRate = 24;
        
        [MenuItem("CONTEXT/SkinnedMeshRenderer/Bake Animation")]
        private static void Open(MenuCommand command)
        {
            var window = GetWindow<AnimationToTexture>();
            window._context = (SkinnedMeshRenderer)command.context;
            window.ShowUtility();
        }

        private void CreateGUI()
        {
            var frameCountField = new IntegerField("FrameRate")
            {
                value = _frameRate
            };
            frameCountField.RegisterValueChangedCallback(OnFrameRateChanged);
            

            var clipField = new ObjectField("Clip")
            {
                objectType = typeof(AnimationClip),
                allowSceneObjects = false,
            };
            clipField.RegisterValueChangedCallback(OnClipChanged);
            
            rootVisualElement.Add(frameCountField);
            rootVisualElement.Add(clipField);
            rootVisualElement.Add(new Button(CreateAnimationTexture) { text = "Record" });
        }

        private void OnFrameRateChanged(ChangeEvent<int> evt)
        {
            _frameRate = Mathf.Clamp(evt.newValue, 1, 60);
        }

        private void OnClipChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            _clip = evt.newValue as AnimationClip;
        }

        private void CreateAnimationTexture()
        {
            Close();
            Contract.Assert(_clip != null, "Animation clip is null");
            
            var duration = _clip.length;
            var frameCount = Mathf.Max((int)(duration * _frameRate), 1);
            var vertexCount = _context.sharedMesh.vertexCount;
            
            var texture = new Texture2D(
                frameCount,
                vertexCount * 2,
                TextureFormat.RGBAHalf,
                false,
                false);
            texture.wrapMode = TextureWrapMode.Clamp;
            
            var targetGO = _context.GetComponentInParent<Animation>().gameObject;
            BakeAnimation(targetGO, frameCount, duration, texture);
            CreateTextureAsset(texture);
        }

        private void BakeAnimation(GameObject target, int frameCount, float duration, Texture2D texture)
        {
            var mesh = new Mesh();
            
            var lastFrameIndex = frameCount - 1;
            for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
                _clip.SampleAnimation(target, (float)frameIndex / lastFrameIndex *  duration);
                _context.BakeMesh(mesh);
                
                var vertices = mesh.vertices;
                var normals = mesh.normals;

                for (int i = 0; i < vertices.Length; i++)
                {
                    var position = vertices[i];
                    var normal = normals[i];
                    var posColor = new Color(position.x, position.y, position.z);
                    var normalColor = new Color(normal.x, normal.y, normal.z);
                    texture.SetPixel(frameIndex, i * 2, posColor);
                    texture.SetPixel(frameIndex, i * 2 + 1, normalColor);
                }
            }
            DestroyImmediate(mesh);
        }

        private void CreateTextureAsset(Texture2D texture)
        {
            var path = EditorUtility.SaveFilePanelInProject("Save animation texture", "Animation", "asset", "Select animation asset path");
            if (string.IsNullOrEmpty(path))
            {
                DestroyImmediate(texture);
                return;
            }
            
            AssetDatabase.CreateAsset(texture, path);
            AssetDatabase.Refresh();
        }
    }
}
