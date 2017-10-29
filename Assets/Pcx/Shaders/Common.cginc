// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

#define PCX_MAX_BRIGHTNESS 16

uint PcxEncodeColor(half3 rgb)
{
    half y = max(max(rgb.r, rgb.g), rgb.b);
    y = clamp(ceil(y * 255 / PCX_MAX_BRIGHTNESS), 1, 255);
    rgb *= 255 * 255 / (y * PCX_MAX_BRIGHTNESS);
    uint4 i = half4(rgb, y);
    return i.x | (i.y << 8) | (i.z << 16) | (i.w << 24);
}

half3 PcxDecodeColor(uint data)
{
    half r = (data      ) & 0xff;
    half g = (data >>  8) & 0xff;
    half b = (data >> 16) & 0xff;
    half a = (data >> 24) & 0xff;
    return half3(r, g, b) * a * PCX_MAX_BRIGHTNESS / (255 * 255);
}
