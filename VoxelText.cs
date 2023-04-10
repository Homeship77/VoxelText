using VoxelText.Baking
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelText
{
	public class VoxelText : MonoBehaviour
	{
		public float SpawnDelay = 5f;
		
		public Texture2D InputTexture;

		public float AlphaValue = 0.55f;
		[SerializeField]
		Material _meshMat;

		[SerializeField]
		float TTL = 5f;

		[SerializeField]
		float _maxScale = 2f;

		Mesh _newMesh;

		public void SetMesh(Mesh msh)
		{
			if (_meshFilter != null)
			{
				if (_meshFilter.mesh != msh)
					_meshFilter.mesh = msh;
			}
			else
			{
				_newMesh = msh;
			}
		}

		void Awake()
		{
			_timer = Time.timeSinceLevelLoad;

			if (_tempObject == null)
			{ 
				_tempObject = new GameObject("voxelTextMesh");

				_tempObject.transform.SetParent(transform);
				_tempObject.transform.localPosition = Vector3.zero;
				_tempObject.transform.localScale = Vector3.zero;
			}

			if (_meshFilter == null)
			{
				_meshFilter = _tempObject.AddComponent<MeshFilter>();
			}

			if (_renderer == null)
			{
				_renderer = _tempObject.AddComponent<MeshRenderer>();

				_renderer.material = _meshMat;
				_renderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
				_renderer.material.SetTextureOffset("_NoiseTex", new Vector2(Random.value, Random.value));
				_renderer.enabled = false;
			}
		   
		}

		float _timer;
		[SerializeField]
		GameObject _tempObject;
		[SerializeField]
		MeshFilter _meshFilter;
		[SerializeField]
		MeshRenderer _renderer;

		float _startTime = 0f;

		public void ShowMesh()
		{
			if (_meshFilter != null)
			{
				if ((_newMesh != null && _meshFilter.mesh != _newMesh) || (_meshFilter.mesh == null))
					_meshFilter.mesh = _newMesh;
			}
			_renderer.material = _meshMat;
			_startTime = Time.timeSinceLevelLoad;
			_renderer.material.SetFloat("_ElapsedTime", 0f);
			_renderer.material.SetTextureOffset("_NoiseTex", new Vector2(Random.value, Random.value));
			_renderer.enabled = true;
			_tempObject.transform.localScale = Vector3.zero;
		}

		public void CreateVoxelText()
		{
			_tempObject = new GameObject("voxelTextMesh");
			_timer = Time.timeSinceLevelLoad;
			_tempObject.transform.SetParent(transform);
			_tempObject.transform.localPosition = Vector3.zero;
			_meshFilter = _tempObject.AddComponent<MeshFilter>();
			_renderer = _tempObject.AddComponent<MeshRenderer>();

			_renderer.material = _meshMat;
			if (_newMesh == null)
			{
				_meshFilter.sharedMesh = VoxelGenerator.GetMeshFromTexture(InputTexture, AlphaValue);
			}
			else
			{
				_meshFilter.mesh = _newMesh;
			}
			_startTime = Time.timeSinceLevelLoad;
			_renderer.material.SetFloat("_ElapsedTime", 0f);
			_renderer.material.SetTextureOffset("_NoiseTex", new Vector2(Random.value, Random.value));
			_tempObject.transform.localScale = Vector3.zero;
		}

		private void Update()
		{
			if (_tempObject != null && Time.timeSinceLevelLoad > _timer + TTL + SpawnDelay)
			{
				Destroy(_meshFilter.mesh);
				gameObject.SetActive(false);
			}

			if (Time.timeSinceLevelLoad < _timer + SpawnDelay)
			{
				return;
			}

			if (_renderer.enabled)
			{
				_tempObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * _maxScale, (Time.timeSinceLevelLoad - _timer) / (TTL * 0.25f));
				
				_renderer.material.SetFloat("_ElapsedTime", Time.timeSinceLevelLoad - _startTime);
				return;
			}

			ShowMesh();
		}

		private void OnDestroy()
		{
			Destroy(_meshFilter.mesh);
		}
	}
}