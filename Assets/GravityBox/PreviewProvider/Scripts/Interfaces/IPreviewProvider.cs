using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GravityBox.PreviewProvider
{
    //interface that objects providing you with objects preview should implement
    public interface IPreviewProvider
    {
        void RequestPreview(object obj, System.Action<Texture> callback);
    }
}