// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;

namespace Pcx
{
    /// A renderer class that renders a point cloud contained by PointCloudData.
    [ExecuteInEditMode]
    public sealed class PointCloudRenderer : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] PointCloudData _source;

        public PointCloudData source {
            get { return _source; }
            set { _source = value; }
        }

        [SerializeField] Color _pointTint = new Color(0.5f, 0.5f, 0.5f, 1);

        public Color pointTint {
            get { return _pointTint; }
            set { _pointTint = value; }
        }

        [SerializeField] float _pointSize = 0.05f;

        public float pointSize {
            get { return _pointSize; }
            set { _pointSize = value; }
        }

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _pointShader;
        [SerializeField, HideInInspector] Shader _diskShader;

        #endregion

        #region Private objects

        ComputeBuffer _pointBuffer;
        Material _pointMaterial;
        Material _diskMaterial;

        #endregion

        #region MonoBehaviour implementation

        void OnValidate()
        {
            _pointSize = Mathf.Max(0, _pointSize);
        }

        void OnDisable()
        {
            // Note: This should be done in OnDisable, not in OnDestroy.
            if (_pointBuffer != null)
            {
                _pointBuffer.Release();
                _pointBuffer = null;
            }
        }

        void OnDestroy()
        {
            if (_pointMaterial != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_pointMaterial);
                    Destroy(_diskMaterial);
                }
                else
                {
                    DestroyImmediate(_pointMaterial);
                    DestroyImmediate(_diskMaterial);
                }
            }
        }

        void OnRenderObject()
        {
            if (_source == null) return;

            // TODO: Do view frustum culling here.

            // Lazy initialization
            if (_pointBuffer == null)
                _pointBuffer = _source.CreateComputeBuffer();

            if (_pointMaterial == null)
            {
                _pointMaterial = new Material(_pointShader);
                _pointMaterial.hideFlags = HideFlags.DontSave;
                _pointMaterial.EnableKeyword("_COMPUTE_BUFFER");

                _diskMaterial = new Material(_diskShader);
                _diskMaterial.hideFlags = HideFlags.DontSave;
                _diskMaterial.EnableKeyword("_COMPUTE_BUFFER");
            }

            if (_pointSize == 0)
            {
                _pointMaterial.SetPass(0);
                _pointMaterial.SetColor("_Color", _pointTint);
                _pointMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);
                _pointMaterial.SetBuffer("_PointBuffer", _pointBuffer);
                Graphics.DrawProcedural(MeshTopology.Points, _source.pointCount, 1);
            }
            else
            {
                _diskMaterial.SetPass(0);
                _diskMaterial.SetColor("_Color", _pointTint);
                _diskMaterial.SetMatrix("_Transform", transform.localToWorldMatrix);
                _diskMaterial.SetBuffer("_PointBuffer", _pointBuffer);
                _diskMaterial.SetFloat("_PointSize", _pointSize);
                Graphics.DrawProcedural(MeshTopology.Points, _source.pointCount, 1);
            }
        }

        #endregion
    }
}
