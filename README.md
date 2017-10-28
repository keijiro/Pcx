Pcx - Point Cloud Importer/Renderer for Unity
=============================================

![GIF](https://i.imgur.com/zc6P96x.gif)
![GIF](https://i.imgur.com/lpWIiXu.gif)

**Pcx** is a custom importer and renderer that allows handling point cloud data
in Unity.

System Requirements
-------------------

- Unity 2017.3

Pcx uses the 32-bit vertex index format that is newly introduced in Unity
2017.3, so it can't be used with the previous versions.

At the time this document is written, Unity 2017.3 is still in the beta testing
phase. Note that it possibly introduces some issues with the final released
version.

Supported Formats
-----------------

At the moment, Pcx only supports PLY binary little-endian format.

Container Types
---------------

![inspector](https://i.imgur.com/Da0p6uV.png)

There are two types of container types for point clouds.

### Mesh

Points are to be contained in the standard Mesh object. They can be rendered
with the MeshRenderer component. It's recommended to use the custom shaders
included in Pcx ("Point Cloud/Point" and "Point Cloud/Disk").

### ComputeBuffer

Points are to be contained in the PointCloudData object, which is optimized
for ComputeBuffer/ComputeShader use. They can be rendered with using the
PointCloudRenderer component.

There are significant performance difference between these two types.
The ComputeBuffer type is just convenient to animating points with using
compute shaders.

Rendering Methods
-----------------

There are two types of rendering methods in Pcx.

- Point (point primitives)

Points are rendered as point primitives when using the "Point Cloud/Point"
shader.

The size of points can be adjusted by the material properties. These size
properties are only supported on some platforms (i.e. DX11/12 doesn't support
them).

This methods is also used when the point size is set to zero in
PointCloudRenderer.

- Disk (geometry shader)

Points are rendered as small disks when using the "Point Cloud/Disk" shader or
PointCloudRenderer.

This method requires geometry shader support.
