using GravityBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GravityBox.PreviewProvider
{
    public abstract class PreviewRenderer : ScriptableObject
    {
        [System.Serializable]
        public class Orientation 
        {
            public Vector3 rotation;
            public float distance;
        }

        public Texture2D defaultAssetIcon;
        public bool overrideCameraPosition;
        public TextureFormat textureFormat = TextureFormat.RGB24;

        [SerializeField]
        private Orientation cameraPosition;
        
        public virtual Texture2D Render(object obj, PreviewScene previewScene, int previewSize)
        {
            if (overrideCameraPosition)
                previewScene.SetCameraOrientation(cameraPosition.rotation, cameraPosition.distance);

            SetupPreviewObject(obj, previewScene.layer);

            Texture2D image;
            try
            {
                previewScene.camera.Render();
                image = new Texture2D(previewSize, previewSize, textureFormat, false);
                image.wrapMode = TextureWrapMode.Clamp;
                image.ReadPixels(new Rect(0, 0, previewSize, previewSize), 0, 0, false);
                image.Apply(false);
            }
            catch
            {
                Debug.LogError(string.Format("Object {0} Preview rendering failed", obj.ToString()));
                image = defaultAssetIcon;
            }

            if (overrideCameraPosition) 
                previewScene.ResetCameraOrientation();

            OnRenderingDone();
            return image;
        }

        protected virtual void SetupPreviewObject(object obj, int layer) { }

        protected virtual void OnRenderingDone() { }

        protected static void AlignPreviewObject(GameObject gameObject, int previewLayer)
        {
            Bounds bounds = new Bounds();

            Renderer[] _renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in _renderers)
            {
                r.gameObject.layer = previewLayer;
                bounds.Encapsulate(r.bounds);
            }

            if (bounds.size == Vector3.zero)
                bounds.size = Vector3.one;

            float size = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
            gameObject.transform.position = -bounds.center * (1f / size);
            gameObject.transform.localScale = Vector3.one * (1f / size);
        }
    }
}