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

        [SerializeField, HideInInspector] Shader _shader;
        [SerializeField, HideInInspector] ComputeShader _sorter;

        #endregion

        #region Private variables

        ComputeBuffer _pointBuffer;

        Material _material1;
        Material _material2;
        Material _material3;

        ComputeBuffer _drawBuffer1;
        ComputeBuffer _drawBuffer2;
        ComputeBuffer _drawBuffer3;

        ComputeBuffer _argsBuffer1;
        ComputeBuffer _argsBuffer2;
        ComputeBuffer _argsBuffer3;

        #endregion

        #region MonoBehaviour methods

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

                _drawBuffer1.Release();
                _drawBuffer2.Release();
                _drawBuffer3.Release();

                _argsBuffer1.Release();
                _argsBuffer2.Release();
                _argsBuffer3.Release();

                _pointBuffer = null;

                _drawBuffer1 = null;
                _drawBuffer2 = null;
                _drawBuffer3 = null;

                _argsBuffer1 = null;
                _argsBuffer2 = null;
                _argsBuffer3 = null;
            }
        }

        void OnDestroy()
        {
            if (_material1 != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_material1);
                    Destroy(_material2);
                    Destroy(_material3);
                }
                else
                {
                    DestroyImmediate(_material1);
                    DestroyImmediate(_material2);
                    DestroyImmediate(_material3);
                }
            }
        }

        void Update()
        {
            // Lazy initialization
            if (_material1 == null)
            {
                _material1 = new Material(_shader);
                _material2 = new Material(_shader);
                _material3 = new Material(_shader);

                _material1.hideFlags = HideFlags.DontSave;
                _material2.hideFlags = HideFlags.DontSave;
                _material3.hideFlags = HideFlags.DontSave;
            }

            if (_pointBuffer == null && _source != null)
            {
                _pointBuffer = _source.CreateComputeBuffer();

                _drawBuffer1 = new ComputeBuffer(_source.pointCount, 4 * 4, ComputeBufferType.Append);
                _drawBuffer2 = new ComputeBuffer(_source.pointCount, 4 * 4, ComputeBufferType.Append);
                _drawBuffer3 = new ComputeBuffer(_source.pointCount, 4 * 4, ComputeBufferType.Append);

                _argsBuffer1 = new ComputeBuffer(1, 4 * 4, ComputeBufferType.IndirectArguments);
                _argsBuffer2 = new ComputeBuffer(1, 4 * 4, ComputeBufferType.IndirectArguments);
                _argsBuffer3 = new ComputeBuffer(1, 4 * 4, ComputeBufferType.IndirectArguments);

                _argsBuffer1.SetData(new uint[] { 0, 1, 0, 0 });
                _argsBuffer2.SetData(new uint[] { 3 * 4, 0, 0, 0 });
                _argsBuffer3.SetData(new uint[] { 3 * 12, 0, 0, 0 });
            }
        }

        void OnRenderObject()
        {
            if (_material1 == null || _pointBuffer == null) return;

            var proj = GL.GetGPUProjectionMatrix(Camera.current.projectionMatrix, true);
            var view = Camera.current.worldToCameraMatrix;

            _sorter.SetFloat("ScreenHeight", Camera.current.pixelHeight);
            _sorter.SetVector("Extent", new Vector2(Mathf.Abs(proj[0, 0]), Mathf.Abs(proj[1, 1])) * _pointSize);
            _sorter.SetMatrix("Transform", proj * view * transform.localToWorldMatrix);

            var kernel = _sorter.FindKernel("Main");
            _sorter.SetBuffer(kernel, "PointBuffer", _pointBuffer);
            _sorter.SetBuffer(kernel, "DrawBuffer1", _drawBuffer1);
            _sorter.SetBuffer(kernel, "DrawBuffer2", _drawBuffer2);
            _sorter.SetBuffer(kernel, "DrawBuffer3", _drawBuffer3);

            _drawBuffer1.SetCounterValue(0);
            _drawBuffer2.SetCounterValue(0);
            _drawBuffer3.SetCounterValue(0);

            _sorter.Dispatch(kernel, _source.pointCount / 128, 1, 1);

            ComputeBuffer.CopyCount(_drawBuffer1, _argsBuffer1, 0);
            ComputeBuffer.CopyCount(_drawBuffer2, _argsBuffer2, 4);
            ComputeBuffer.CopyCount(_drawBuffer3, _argsBuffer3, 4);

            _material1.SetPass(0);
            _material1.SetColor("_Tint", _pointColor);
            _material1.SetMatrix("_Transform", transform.localToWorldMatrix);
            _material1.SetBuffer("_PointBuffer", _drawBuffer1);
            Graphics.DrawProceduralIndirect(MeshTopology.Points, _argsBuffer1, 0);

            _material2.SetPass(1);
            _material2.SetColor("_Tint", _pointColor);
            _material2.SetFloat("_PointSize", _pointSize);
            _material2.SetMatrix("_Transform", transform.localToWorldMatrix);
            _material2.SetBuffer("_PointBuffer", _drawBuffer2);
            Graphics.DrawProceduralIndirect(MeshTopology.Triangles, _argsBuffer2, 0);

            _material3.SetPass(2);
            _material3.SetColor("_Tint", _pointColor);
            _material3.SetFloat("_PointSize", _pointSize);
            _material3.SetMatrix("_Transform", transform.localToWorldMatrix);
            _material3.SetBuffer("_PointBuffer", _drawBuffer3);
            Graphics.DrawProceduralIndirect(MeshTopology.Triangles, _argsBuffer3, 0);
        }

        #endregion
    }
}
