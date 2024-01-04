using UnityEngine;

namespace GravityBox.PreviewProvider
{
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