using UnityEngine;

namespace GravityBox.PreviewProvider
{
    /// <summary>
    /// Renderer for materials, in theory can use different shapes (box, sphere, donut) for rendering
    /// but that will require preview cache fixing
    /// </summary>
    public class PreviewRendererMaterial : PreviewRenderer
    {
        public GameObject previewPrefab;

        private static GameObject previewGO;

        protected override void SetupPreviewObject(object obj, int layer)
        {
            if (previewGO != null)
                previewGO.SetActive(true);
            else
            {
                previewGO = Instantiate(previewPrefab);
                AlignPreviewObject(previewGO, layer);
            }

            previewGO.GetComponent<Renderer>().sharedMaterial = (Material)obj;
        }

        protected override void OnRenderingDone()
        {
            previewGO.SetActive(false);
        }
    }
}