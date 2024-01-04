using UnityEngine;

namespace GravityBox.PreviewProvider
{
    /// <summary>
    /// interface that objects providing you with objects preview should implement
    /// Obj object needed to get rendering of. Can be of almost any type
    /// (as long as it is supported by implemented PreviewRenderer)
    /// Callback is used to supply resulting generated image
    /// </summary>
    public interface IPreviewProvider
    {
        void RequestPreview(object obj, System.Action<Texture> callback);
    }
}