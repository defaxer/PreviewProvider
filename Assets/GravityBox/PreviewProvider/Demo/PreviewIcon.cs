using UnityEngine;
using UnityEngine.UI;

namespace GravityBox.PreviewProvider.Demo
{
    /// <summary>
    /// Simple example usage of Preview Provider
    /// Attach to GameObject with RawImage
    /// Use RequestPreview function with callback to update a RawImage.texture
    /// </summary>
    public class PreviewIcon : MonoBehaviour
    {
        //Object to generate preview for
        public Object renderable;
        //Preview generator (ScriptableObject)
        public Object previewGenerator;

        //Can't assign Interface references to scripts so using it like that
        //Might as well use PreviewProvider directly
        public IPreviewProvider previewProvider => (IPreviewProvider)previewGenerator;

        void Start()
        {
            previewProvider.RequestPreview(renderable, SetPreview);
        }

        void SetPreview(Texture preview)
        {
            //when preview is rendered update texture
            GetComponent<RawImage>().texture = preview;
        }
    }
}
