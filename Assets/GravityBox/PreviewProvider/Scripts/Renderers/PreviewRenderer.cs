using UnityEngine;

namespace GravityBox.PreviewProvider
{
    /// <summary>
    /// Base class for all Renderers responsible for creating preview object and 
    /// positioning it on a Preview scene. Can have per Renderer configured settings,
    /// like camera rotation or texture format
    /// </summary>
    public abstract class PreviewRenderer : ScriptableObject
    {
        /// <summary>
        /// positioning camera around rendered object with vertical horizontal rotation and distance from object
        /// </summary>
        [System.Serializable]
        public class Orientation 
        {
            public Vector3 rotation;
            public float distance;
        }

        /// <summary>
        /// default icon in case something failed to render object
        /// </summary>
        public Texture2D defaultAssetIcon;
        
        /// <summary>
        /// texture format for resulting image, if transparency needed use RGBA32 or ARGB32
        /// </summary>
        public TextureFormat textureFormat = TextureFormat.RGB24;
        public bool overrideCameraPosition;

        [SerializeField]
        private Orientation cameraPosition;
        
        /// <summary>
        /// creating resulting preview texture for given object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="previewScene">scene containing preview object</param>
        /// <param name="previewSize">size of resulting texture</param>
        /// <returns></returns>
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

        /// <summary>
        /// Create preview object by code or from prefab here
        /// layer of camera's culling mask 
        /// as in most cases camera should use layer invisible to other game cameras
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        protected virtual void SetupPreviewObject(object obj, int layer) { }

        /// <summary>
        /// make cleanup code here, like hiding or removing preview object from scene
        /// </summary>
        protected virtual void OnRenderingDone() { }

        /// <summary>
        /// used to position preview game object just before camera
        /// Note: camera is concidered static here, in case closeup render needed move camera
        /// with position override, will affect all objects of same type
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="previewLayer"></param>
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