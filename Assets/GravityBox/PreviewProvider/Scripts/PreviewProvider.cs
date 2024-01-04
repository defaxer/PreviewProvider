using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GravityBox.PreviewProvider
{
	/// <summary>
	/// Base Implementation of main rendering manager that manages all processes involved
	/// </summary>
	public class PreviewProvider : ScriptableObject, IPreviewProvider
	{
		/// <summary>
		/// type of objects to render and a renderer handling this
		/// </summary>
		[System.Serializable]
		public class TypeRenderer
		{
			public TypeReference typeReference;
			public PreviewRenderer previewRenderer;
		}

		class ObjectInfo
		{
			public object asset;
			public System.Action<Texture> callback;
		}

		public int previewSize = 64;
		public TypeRenderer[] renderers;
		
		/// <summary>
		/// Rendering Scene prefab with lights and camera
		/// </summary>
		public GameObject previewScenePrefab;

		/// <summary>
		/// default asset icon before rendering done
		/// </summary>
		[SerializeField]
		private Texture2D defaultAssetIcon;

		/// <summary>
		/// supply render texture here through Inspector or it will fallback to default
		/// </summary>
		[SerializeField]
		private RenderTexture _renderTexture;

		private Dictionary<int, Texture2D> previewsCache = new Dictionary<int, Texture2D>();
		private Queue<ObjectInfo> _objects = new Queue<ObjectInfo>();

		//working rendering scene
		private PreviewScene previewScene;

		//multithreading lock
		private object _lockObject = new object();
		private bool _isRendering;

		private RenderTexture renderTexture
		{
			get
			{
				if (_renderTexture == null)
				{
					_renderTexture = new RenderTexture(previewSize, previewSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
					_renderTexture.depth = 16;
					_renderTexture.antiAliasing = 8;
					_renderTexture.autoGenerateMips = false;
					_renderTexture.useMipMap = false;
				}

				return _renderTexture;
			}
		}

        private void OnEnable()
        {
			_isRendering = false;
        }

		//remove resources generated at runtime
		//when not needed
        private void Cleanup()
		{
			foreach (var preview in previewsCache)
			{
				//check if texture is runtime created texture 
				//and not an asset (assets have instance ids > 0)
				if (preview.Value.GetInstanceID() < 0)
					Destroy(preview.Value);
			}

			previewsCache.Clear();
		}

		public void RequestPreview(object obj, System.Action<Texture> callback)
		{
			if (previewsCache.ContainsKey(obj.GetHashCode()))
				callback.Invoke(previewsCache[obj.GetHashCode()]);
			else
			{
				_objects.Enqueue(new ObjectInfo() { asset = obj, callback = callback });
				StartRenderingAsync();
			}
		}

		/// <summary>
		/// Async rendering of all objects currently in a list
		/// </summary>
		private async void StartRenderingAsync()
		{
			lock (_lockObject)
			{
				if (_isRendering)
					return;
				else
					_isRendering = true;
			}

			if (previewScene == null) 
			{
				GameObject scene = Instantiate(previewScenePrefab);
				previewScene = scene.GetComponent<PreviewScene>();
				previewScene.OnSceneDestroy += Cleanup;
			}

			previewScene.DisableSceneLights();

			while (_objects.Count > 0)
			{
				ObjectInfo obj = _objects.Dequeue();
				if (obj != null)
					RenderPreview(obj);
				await Task.Yield();
			}

			renderTexture.DiscardContents();
			RenderTexture.active = null;
			renderTexture.Release();

			previewScene.camera.enabled = false;
			previewScene.RestoreSceneLights();

			_isRendering = false;
		}

		private void RenderPreview(ObjectInfo obj)
		{
			if (obj.asset == null) return;

			if (RenderTexture.active != renderTexture)
				RenderTexture.active = renderTexture;

			if (previewScene.camera.targetTexture != renderTexture)
				previewScene.camera.targetTexture = renderTexture;

			Texture2D result = defaultAssetIcon;

			foreach (var r in renderers)
			{
				if (r.typeReference.Type.Equals(obj.asset.GetType()))
				{
					result = r.previewRenderer.Render(obj.asset, previewScene, previewSize);
					obj.callback?.Invoke(result);
				}
			}

			previewsCache[obj.asset.GetHashCode()] = result;
		}
	}
}
