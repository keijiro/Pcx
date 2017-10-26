using UnityEngine;

namespace Pcx
{
    [ExecuteInEditMode]
    public class PointCloudRenderer : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] PointCloudBuffer _source;

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _shader;

        #endregion

        #region Private variables

        Material _material;

        ComputeBuffer _positionBuffer;
        ComputeBuffer _colorBuffer;

        #endregion

        #region MonoBehaviour methods

        void OnDisable()
        {
            // Note: This should be done in OnDisable, not in OnDestroy.
            if (_positionBuffer != null)
            {
                _positionBuffer.Release();
                _colorBuffer.Release();
                _positionBuffer = _colorBuffer = null;
            }
        }

        void OnDestroy()
        {
            if (_material != null)
            {
                if (Application.isPlaying)
                    Destroy(_material);
                else
                    DestroyImmediate(_material);
            }
        }

        void Update()
        {
            // Lazy initialization
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            if (_positionBuffer == null && _source != null)
            {
                _positionBuffer = _source.CreatePositionBuffer();
                _colorBuffer = _source.CreateColorBuffer();
            }
        }

        void OnRenderObject()
        {
            if (_material == null || _positionBuffer == null) return;

            _material.SetPass(0);
            _material.EnableKeyword("_COMPUTE_BUFFER");
            _material.SetBuffer("_PositionBuffer", _positionBuffer);
            _material.SetBuffer("_ColorBuffer", _colorBuffer);

            Graphics.DrawProcedural(MeshTopology.Points, _source.pointCount, 1);
        }

        #endregion
    }
}
