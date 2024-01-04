using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GravityBox.PreviewProvider;
using UnityEngine.UI;

namespace GravityBox.PreviewProvider.Demo
{
    public class PreviewIcon : MonoBehaviour
    {
        public Object renderable;
        public Object previewGenerator;

        public IPreviewProvider previewProvider => (IPreviewProvider)previewGenerator;

        void Start()
        {
            previewProvider.RequestPreview(renderable, SetPreview);
        }

        void SetPreview(Texture preview)
        {
            GetComponent<RawImage>().texture = preview;
        }
    }
}
