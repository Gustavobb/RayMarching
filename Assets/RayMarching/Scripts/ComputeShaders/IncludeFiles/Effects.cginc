#include "IncludeFiles/Utils.cginc"

// polynomial smooth min (k = 0.1);
// from https://www.iquilezles.org/www/articles/smin/smin.htm
float4 SmoothUnion(float d1, float d2, float3 colA, float3 colB, float k)
{
    float h = clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
    float blendDst = lerp(d2, d1, h) - k * h * (1.0 - h);
    float3 blendCol = lerp(colB, colA, h);
    return float4(blendCol, blendDst);
}

float4 SmoothSubtraction(float d1, float d2, float3 colA, float3 colB, float k) 
{
    float h = clamp(0.5 - 0.5 * (d2 + d1) / k, 0.0, 1.0);
    float blendDst = lerp(d2, -d1, h) + k * h * (1.0 - h);
    float3 blendCol = lerp(colB, colA, h);
    return float4(blendCol, blendDst);
}

float4 SmoothIntersection(float d1, float d2, float3 colA, float3 colB, float k) 
{
    float h = clamp(0.5 - 0.5 * (d2 - d1) / k, 0.0, 1.0);
    float blendDst = lerp(d2, d1, h) + k * h * (1.0 - h);
    float3 blendCol = lerp(colB, colA, h);
    return float4(blendCol, blendDst);
}

float3 InfiniteRep(float3 p, float3 size, float3 offset)
{
    float3 s = float3(offset.x + size.x, offset.y + size.y, offset.z + size.z);
    return fmod(p + s * 0.5, s) - s * 0.5;
}

float3 CheapBend(float3 p, float k)
{
    float c = cos(k * p.x);
    float s = sin(k * p.x);
    float2x2 m = float2x2(c, -s, s, c);
    return float3(mul(m, p.xy), p.z);
}

float3 Twist(float3 p, float k)
{
    p = mul(float4(p, 1), QuaternionMatrix(-0.7, 0.0, 0.0, 0.7)).xyz;
    float c = cos(k * p.y);
    float s = sin(k * p.y);
    float2x2 m = float2x2(c, -s, s, c);
    return float3(mul(m, p.xz), p.y);
}

float Displace(float d1, float3 p, float k)
{
    float d2 = sin(k * p.x) * sin(k * p.y) * sin(k * p.z);
    return d1 + d2;
}
