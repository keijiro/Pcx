using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pcx
{
    [ScriptedImporter(1, "ply")]
    class PlyImporter : ScriptedImporter
    {
        #region ScriptedImporter implementation

        public override void OnImportAsset(AssetImportContext context)
        {
            var gameObject = new GameObject();
            var mesh = ImportPlyPointCloud(context.assetPath);

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = GetDefaultMaterial();

            context.AddObjectToAsset("prefab", gameObject);
            if (mesh != null) context.AddObjectToAsset("mesh", mesh);

            context.SetMainObject(gameObject);
        }

        #endregion

        #region Internal utilities

        static Material GetDefaultMaterial()
        {
            return AssetDatabase.GetBuiltinExtraResource<Material>(
                "Default-Material.mat"
            );
        }

        #endregion

        #region Ply file header definition

        class Header
        {
            public enum Property {
                Invalid, Data8, Data16, Data32, X, Y, Z, R, G, B
            }
            public List<Property> properties = new List<Property>();
            public int vertexCount = -1;
        }

        #endregion

        #region Reader implementation

        Mesh ImportPlyPointCloud(string path)
        {
            try
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var header = ReadHeader(new StreamReader(stream));
                return ReadPointCloud(header, name, new BinaryReader(stream));
            }
            catch (Exception e)
            {
                Debug.LogError("Failed importing " + path + ". " + e.Message);
                return null;
            }
        }

        Header ReadHeader(StreamReader reader)
        {
            var header = new Header();
            var read = 0;

            // Magic number line ("ply")
            var line = reader.ReadLine();
            read += line.Length + 1;
            if (line != "ply")
                throw new ArgumentException("Invalid magic number.");

            // Data format: binary/little endian
            line = reader.ReadLine();
            read += line.Length + 1;
            if (line != "format binary_little_endian 1.0")
                throw new ArgumentException("Invalid data format. Should be binary/little endian.");

            // Read header members.
            for (var skip = false;;)
            {
                // Read a line and split it with white space.
                line = reader.ReadLine();
                read += line.Length + 1;
                if (line == "end_header") break;
                var col = line.Split();

                // Element declaration line
                if (col[0] == "element")
                {
                    if (col[1] == "vertex")
                    {
                        header.vertexCount = Convert.ToInt32(col[2]);
                        skip = false;
                    }
                    else
                    {
                        skip = true;
                    }
                }

                if (skip) continue;

                // Property declaration line
                if (col[0] == "property")
                {
                    var prop = Header.Property.Invalid;

                    switch (col[2])
                    {
                        case "x"    : prop = Header.Property.X; break;
                        case "y"    : prop = Header.Property.Y; break;
                        case "z"    : prop = Header.Property.Z; break;
                        case "red"  : prop = Header.Property.R; break;
                        case "green": prop = Header.Property.G; break;
                        case "blue" : prop = Header.Property.B; break;
                    }

                    if (col[1] == "uchar")
                    {
                        if (prop == Header.Property.Invalid)
                        {
                            prop = Header.Property.Data8;
                        }
                        else if (prop != Header.Property.R &&
                                 prop != Header.Property.G &&
                                 prop != Header.Property.B)
                        {
                            throw new ArgumentException("Invalid property type.");
                        }
                    }
                    else if (col[1] == "short" || col[1] == "ushort")
                    {
                        if (prop == Header.Property.Invalid)
                        {
                            prop = Header.Property.Data16;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid property type.");
                        }
                    }
                    else if (col[1] == "int" || col[1] == "uint")
                    {
                        if (prop == Header.Property.Invalid)
                        {
                            prop = Header.Property.Data32;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid property type.");
                        }
                    }
                    else if (col[1] == "float")
                    {
                        if (prop == Header.Property.Invalid)
                        {
                            prop = Header.Property.Data32;
                        }
                        else if (prop != Header.Property.X &&
                                 prop != Header.Property.Y &&
                                 prop != Header.Property.Z)
                        {
                            throw new ArgumentException("Invalid property type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported property type.");
                    }

                    header.properties.Add(prop);
                }
            }

            // Rewind the stream back to the exact position of the reader.
            reader.BaseStream.Position = read;

            return header;
        }

        Mesh ReadPointCloud(Header header, string name, BinaryReader reader)
        {
            var vertices = new List<Vector3>(header.vertexCount);
            var colors = new List<Color32>(header.vertexCount);

            float x = 0, y = 0, z = 0;
            Byte r = 0, g = 0, b = 0;

            for (var i = 0; i < header.vertexCount; i++)
            {
                foreach (var prop in header.properties)
                {
                    switch (prop)
                    {
                        case Header.Property.X: x = reader.ReadSingle(); break;
                        case Header.Property.Y: y = reader.ReadSingle(); break;
                        case Header.Property.Z: z = reader.ReadSingle(); break;

                        case Header.Property.R: r = reader.ReadByte(); break;
                        case Header.Property.G: g = reader.ReadByte(); break;
                        case Header.Property.B: b = reader.ReadByte(); break;

                        case Header.Property.Data8:
                            reader.ReadByte(); break;

                        case Header.Property.Data16:
                            reader.BaseStream.Position += 2; break;

                        case Header.Property.Data32:
                            reader.BaseStream.Position += 4; break;
                    }
                }

                vertices.Add(new Vector3(x, y, z));
                colors.Add(new Color32(r, g, b, 255));
            }

            var mesh = new Mesh();
            mesh.name = name;
            mesh.indexFormat = IndexFormat.UInt32;

            mesh.SetVertices(vertices);
            mesh.SetColors(colors);

            mesh.SetIndices(
                Enumerable.Range(0, header.vertexCount).ToArray(),
                MeshTopology.Points, 0
            );

            mesh.UploadMeshData(true);
            return mesh;
        }
    }

    #endregion
}
