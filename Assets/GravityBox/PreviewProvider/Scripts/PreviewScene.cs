using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GravityBox.PreviewProvider
{
	public class PreviewScene : MonoBehaviour
	{
		public event System.Action OnSceneDestroy;

		public Vector3 cameraAngles = new Vector3(35, -150, 0);
		public float cameraDistance = 4.7f;
		public int layerOverride = -1;

		[SerializeField]
		private Camera _camera;
		private Light[] _lights;

		private List<Light> disabled;

		private Vector3 cameraDefaultPosition;
		private Quaternion cameraDefaultRotation;

		public int layer
		{
			get => gameObject.layer;
			set => gameObject.layer = value;
		}

		public new Camera camera => _camera;

		private void Awake()
		{
			_camera = gameObject.GetComponentInChildren<Camera>();
			_lights = gameObject.GetComponentsInChildren<Light>();

			cameraDefaultPosition = _camera.transform.localPosition;
			cameraDefaultRotation = _camera.transform.localRotation;

			if (layerOverride >= 0)
			{
				gameObject.layer = layerOverride;
				SetLayers(new int[] { layerOverride });
			}
		}

		private void OnDestroy() { OnSceneDestroy?.Invoke(); }

        private void OnValidate()
        {
			if (_camera != null)
				SetCameraOrientation(cameraAngles, cameraDistance);
        }

        public void SetLayers(int[] layers)
		{
			int mask = 0;
			foreach (var l in layers)
			{
				mask = BitMask.AddLayer(mask, l);
			}

			SetCullingMask(mask);
		}

		void SetCullingMask(int cullingMask)
		{
			camera.cullingMask = cullingMask;

			foreach (var l in _lights)
				l.cullingMask = cullingMask;
		}
		
		public void SetCameraOrientation(Vector3 rotation, float distance)
		{
			_camera.transform.localRotation = Quaternion.Euler(rotation);
			_camera.transform.localPosition = Vector3.zero - _camera.transform.forward * distance;
		}

		public void ResetCameraOrientation()
		{
			_camera.transform.localPosition = cameraDefaultPosition;
			_camera.transform.localRotation = cameraDefaultRotation;
		}

		public void DisableSceneLights()
		{
			disabled = new List<Light>();
			Light[] lights = FindObjectsOfType<Light>();
			
			foreach (Light l in lights)
			{
				if (l.enabled && System.Array.IndexOf(_lights, l) < 0)
				{
					if (BitMask.HasLayer(l.cullingMask, layer))
					{
						l.cullingMask = BitMask.RemoveLayer(l.cullingMask, layer);
						disabled.Add(l);
					}
				}
			}
		}

		public void RestoreSceneLights()
		{
			foreach (Light l in disabled)
				l.cullingMask = BitMask.AddLayer(l.cullingMask, layer);
		}
	}

	public static class BitMask
	{
		public static bool HasLayer(int mask, int layer)
		{
			return (mask & 1 << layer) != 0;
		}

		public static int RemoveLayer(int mask, int layer)
		{
			return mask & ~(1 << layer);
		}

		public static int AddLayer(int mask, int layer)
		{
			return mask | 1 << layer;
		}
	}
}
