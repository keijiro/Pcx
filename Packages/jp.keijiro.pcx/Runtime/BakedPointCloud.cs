// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using System.Collections.Generic;

namespace Pcx
{
    /// A container class for texture-baked point clouds.
    public sealed class BakedPointCloud : ScriptableObject
    {
        #region Public properties

        /// Number of points
        public int pointCount { get { return _pointCount; } }

        /// Position map texture
        public Texture2D positionMap { get { return _positionMap; } }

        /// Color map texture
        public Texture2D colorMap { get { return _colorMap; } }

        #endregion

        #region Serialized data members

        [SerializeField] int _pointCount;
        [SerializeField] Texture2D _positionMap;
        [SerializeField] Texture2D _colorMap;

        #endregion

        #region Editor functions

        #if UNITY_EDITOR

        public void Initialize(List<Vector3> positions, List<Color32> colors)
        {
            _pointCount = positions.Count;

            var width = Mathf.CeilToInt(Mathf.Sqrt(_pointCount));

            _positionMap = new Texture2D(width, width, TextureFormat.RGBAHalf, false);
            _positionMap.name = "Position Map";
            _positionMap.filterMode = FilterMode.Point;

            _colorMap = new Texture2D(width, width, TextureFormat.RGBA32, false);
            _colorMap.name = "Color Map";
            _colorMap.filterMode = FilterMode.Point;

            var i1 = 0;
            var i2 = 0U;

            for (var y = 0; y < width; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = i1 < _pointCount ? i1 : (int)(i2 % _pointCount);
                    var p = positions[i];

                    _positionMap.SetPixel(x, y, new Color(p.x, p.y, p.z));
                    _colorMap.SetPixel(x, y, colors[i]);

                    i1 ++;
                    i2 += 132049U; // prime
                }
            }

            _positionMap.Apply(false, true);
            _colorMap.Apply(false, true);
        }

        #endif

        #endregion
    }
}
