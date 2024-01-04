using UnityEngine;

namespace GravityBox.PreviewProvider
{
	/// <summary>
	/// Class responsible for rendering of all GameObjects
	/// If you need to render concrete type of objects like Character Weapon Vehicle
	/// implement some wrapper class for it and a PreviewRenderer for that class
	/// </summary>
	public class PreviewRendererGameObject : PreviewRenderer
	{
		private GameObject previewGO;

		protected override void SetupPreviewObject(object gameObject, int layer)
		{
			previewGO = Instantiate((GameObject)gameObject, Vector3.zero, Quaternion.identity);
			AlignPreviewObject(previewGO, layer);
		}

		//since all gameobjects are different and simple destroy is too slow
		//removing objects with Immediate destroy call
		protected override void OnRenderingDone()
		{
			DestroyImmediate(previewGO);
		}
	}
}
