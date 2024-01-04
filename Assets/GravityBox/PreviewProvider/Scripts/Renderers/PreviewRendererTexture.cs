using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GravityBox.PreviewProvider
{
    public class PreviewRendererTexture : PreviewRenderer
    {
        public GameObject previewPrefab;
        public string normalMapSuffix = "_norm";

        private static GameObject previewGO;
        private static Material previewMaterial;

        protected override void SetupPreviewObject(object obj, int layer)
        {
            if (previewGO != null)
                previewGO.SetActive(true);
            else
            {
                previewGO = Instantiate(previewPrefab);
                previewMaterial = previewGO.GetComponentInChildren<Renderer>().sharedMaterial;
                AlignPreviewObject(previewGO, layer);
            }

            Texture texture = (Texture)obj;
            if (texture.name.Contains(normalMapSuffix))
                previewMaterial.EnableKeyword("_NORMALMAP");
            else
                previewMaterial.DisableKeyword("_NORMALMAP");
            
            previewMaterial.SetTexture("_MainTex", (Texture)obj);
        }

        protected override void OnRenderingDone()
        {
            previewGO.SetActive(false);
        }
    }
}
