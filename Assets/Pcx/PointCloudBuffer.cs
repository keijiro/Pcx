using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Pcx
{
    public sealed class PointCloudBuffer : ScriptableObject
    {
        #region Public properties and methods

        /// Number of points.
        public int pointCount {
            get { return _positionData.Length; }
        }

        /// Create a compute buffer for the position data.
        /// The returned buffer must be released by the caller.
        public ComputeBuffer CreatePositionBuffer()
        {
            var buffer = new ComputeBuffer(pointCount, sizeof(float) * 4);
            buffer.SetData(_positionData);
            return buffer;
        }

        /// Create a compute buffer for the color data.
        /// The returned buffer must be released by the caller.
        public ComputeBuffer CreateColorBuffer()
        {
            var buffer = new ComputeBuffer(pointCount, sizeof(System.Byte) * 4);
            buffer.SetData(_colorData);
            return buffer;
        }

        #endregion

        #region Serialized data members

        [SerializeField] Vector4[] _positionData;
        [SerializeField] Color32[] _colorData;

        #endregion

        #region Editor functions

        #if UNITY_EDITOR

        public void Initialize(List<Vector3> positions, List<Color32> colors)
        {
            _positionData = positions.Select(x => (Vector4)x).ToArray();
            _colorData = colors.ToArray();
        }

        #endif

        #endregion
    }
}
