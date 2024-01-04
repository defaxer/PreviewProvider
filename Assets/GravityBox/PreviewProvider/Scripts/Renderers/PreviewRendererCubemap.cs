using UnityEngine;

namespace GravityBox.PreviewProvider
{
    public class PreviewRendererCubemap : PreviewRenderer
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

            previewGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_Cubemap", (Cubemap)obj);
        }

        protected override void OnRenderingDone()
        {
            previewGO.SetActive(false);
        }
    }
}
