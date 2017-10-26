using UnityEngine;

namespace Pcx
{
    [ExecuteInEditMode]
    public class PointCloudRenderer : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] PointCloudBuffer _source;
        [SerializeField] Color _pointColor = Color.white;
        [SerializeField] float _pointSize = 0.05f;

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _pointShader;
        [SerializeField, HideInInspector] Shader _discShader;
        [SerializeField, HideInInspector] ComputeShader _converter;

        #endregion

        #region Private variables

        Material _pointMaterial;
        Material _discMaterial;

        ComputeBuffer _positionBuffer;
        ComputeBuffer _colorBuffer;
        ComputeBuffer _triangleBuffer;

        #endregion

        #region MonoBehaviour methods

        void OnValidate()
        {
            _pointSize = Mathf.Max(0, _pointSize);
        }

        void OnDisable()
        {
            // Note: This should be done in OnDisable, not in OnDestroy.
            if (_positionBuffer != null)
            {
                _positionBuffer.Release();
                _colorBuffer.Release();
                _triangleBuffer.Release();

                _positionBuffer = null;
                _colorBuffer = null;
                _triangleBuffer = null;
            }
        }

        void OnDestroy()
        {
            if (_pointMaterial != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_pointMaterial);
                    Destroy(_discMaterial);
                }
                else
                {
                    DestroyImmediate(_pointMaterial);
                    DestroyImmediate(_discMaterial);
                }
            }
        }

        void Update()
        {
            // Lazy initialization
            if (_pointMaterial == null)
            {
                _pointMaterial = new Material(_pointShader);
                _pointMaterial.hideFlags = HideFlags.DontSave;
                _pointMaterial.EnableKeyword("_COMPUTE_BUFFER");

                _discMaterial = new Material(_discShader);
                _discMaterial.hideFlags = HideFlags.DontSave;
            }

            if (_positionBuffer == null && _source != null)
            {
                _positionBuffer = _source.CreatePositionBuffer();
                _colorBuffer = _source.CreateColorBuffer();

                _triangleBuffer = new ComputeBuffer(
                    _source.pointCount, sizeof(float) * 4 * 4,
                    ComputeBufferType.Append
                );
            }
        }

        void OnRenderObject()
        {
            if (_pointMaterial == null || _positionBuffer == null) return;

            if (_pointSize == 0)
            {
                _pointMaterial.SetPass(0);
                _pointMaterial.SetColor("_Color", _pointColor);
                _pointMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);
                _pointMaterial.SetBuffer("_PositionBuffer", _positionBuffer);
                _pointMaterial.SetBuffer("_ColorBuffer", _colorBuffer);
                Graphics.DrawProcedural(MeshTopology.Points, _source.pointCount, 1);
            }
            else
            {
                var pm = GL.GetGPUProjectionMatrix(Camera.current.projectionMatrix, false);
                var vm = Camera.current.worldToCameraMatrix;
                var kernel = _converter.FindKernel("Main");

                _converter.SetVector("Tint", _pointColor);
                _converter.SetMatrix("Transform", pm * vm * transform.localToWorldMatrix);
                _converter.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
                _converter.SetBuffer(kernel, "ColorBuffer", _colorBuffer);
                _converter.SetBuffer(kernel, "TriangleBuffer", _triangleBuffer);
                _triangleBuffer.SetCounterValue(0);
                _converter.Dispatch(kernel, _source.pointCount / 128, 1, 1);

                _discMaterial.SetPass(0);
                _discMaterial.SetBuffer("_TriangleBuffer", _triangleBuffer);
                Graphics.DrawProcedural(MeshTopology.Triangles, 3 * _source.pointCount);
            }
        }

        #endregion
    }
}
