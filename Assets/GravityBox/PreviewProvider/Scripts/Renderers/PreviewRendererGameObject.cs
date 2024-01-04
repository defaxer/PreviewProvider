using UnityEngine;

namespace GravityBox.PreviewProvider
{
	public class PreviewRendererGameObject : PreviewRenderer
	{
		public float previewAngle = 0;

		private GameObject previewGO;

		protected override void SetupPreviewObject(object gameObject, int layer)
		{
			previewGO = Instantiate((GameObject)gameObject, Vector3.zero, Quaternion.Euler(0, previewAngle, 0));
			AlignPreviewObject(previewGO, layer);
		}

		protected override void OnRenderingDone()
		{
			DestroyImmediate(previewGO);
		}
	}
}
