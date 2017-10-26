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

        #endregion

        #region Private variables

        Material _pointMaterial;
        Material _discMaterial;

        ComputeBuffer _positionBuffer;
        ComputeBuffer _colorBuffer;

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
                _positionBuffer = _colorBuffer = null;
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
                _discMaterial.EnableKeyword("_COMPUTE_BUFFER");
            }

            if (_positionBuffer == null && _source != null)
            {
                _positionBuffer = _source.CreatePositionBuffer();
                _colorBuffer = _source.CreateColorBuffer();
            }

            _pointMaterial.SetColor("_Color", _pointColor);
            _discMaterial.SetColor("_Color", _pointColor);

            _pointMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);
            _discMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);

            _discMaterial.SetFloat("_PointSize", _pointSize);

            _pointMaterial.SetBuffer("_PositionBuffer", _positionBuffer);
            _discMaterial.SetBuffer("_PositionBuffer", _positionBuffer);

            _pointMaterial.SetBuffer("_ColorBuffer", _colorBuffer);
            _discMaterial.SetBuffer("_ColorBuffer", _colorBuffer);
        }

        void OnRenderObject()
        {
            if (_pointMaterial == null || _positionBuffer == null) return;

            if (_pointSize == 0)
                _pointMaterial.SetPass(0);
            else
                _discMaterial.SetPass(0);

            Graphics.DrawProcedural(MeshTopology.Points, _source.pointCount, 1);

        }

        #endregion
    }
}
