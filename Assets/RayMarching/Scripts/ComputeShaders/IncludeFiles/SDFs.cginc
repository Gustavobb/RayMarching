float SphereDistance(float3 p) 
{
    return length(p) - 1.0;
}

float CubeDistance(float3 p, float3 size) 
{
  float3 q = abs(p) - size;
  return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Following distance functions from http://iquilezles.org/www/articles/distfunctions/distfunctions.htm
float TorusDistance(float3 p, float r)
{   
    float2 q = float2(length(p.xz) - 1.0, p.y);
    return length(q) - r;
}

float PyramidDistance(float3 p)
{
    float h = 1.0;
    float m2 = h * h + 0.25;

    p.xz = abs(p.xz);
    p.xz = (p.z > p.x) ? p.zx : p.xz;
    p.xz -= 0.5;

    float3 q = float3(p.z, h * p.y - 0.5 * p.x, h * p.x + 0.5 * p.y);

    float s = max(-q.x, 0.0);
    float t = clamp((q.y - 0.5 * p.z) / (m2 + 0.25), 0.0, 1.0);

    float a = m2 * (q.x + s) * (q.x + s) + q.y * q.y;
    float b = m2 * (q.x + 0.5 * t) * (q.x + 0.5 * t) + (q.y - m2 * t) * (q.y - m2 * t);

    float d2 = min(q.y, -q.x * m2 - q.y * 0.5) > 0.0 ? 0.0 : min(a, b);

    return sqrt((d2 + q.z * q.z) / m2) * sign(max(q.z, -p.y));
}

float CappedCylinderDistance(float3 p)
{
    float3 a = float3(0.0, 1.0, 0.0) * 0.5;
    float3 b = -a;
    float3 ba = b - a;
    float3 pa = p - a;
    float baba = dot(ba, ba);
    float paba = dot(pa, ba);
    float x = length(pa * baba - ba * paba) - 1.0 * baba;
    float y = abs(paba - baba * 0.5) - baba * 0.5;
    float x2 = x * x;
    float y2 = y * y * baba;
    float d = (max(x, y) < 0.0) ? -min(x2, y2) : (((x > 0.0) ? x2 : 0.0) + ((y > 0.0) ? y2 : 0.0));
    return sign(d) * sqrt(abs(d)) / baba;
}

float ConeDistance(float3 p)
{
    float h = 1.0;
    float2 c = float2(.5, 1.0);
    float2 q = h * float2(c.x / c.y, 1.0);
    float2 w = float2(length(p.xz), p.y);
    float2 a = w - q * clamp(dot(w, q) / dot(q, q), 0.0, 1.0);
    float2 b = w - q * float2(clamp(w.x / q.x, 0.0, 1.0 ), 1.0);
    float k = sign(q.y);
    float d = min(dot(a, a), dot(b, b));
    float s = max(k * (w.x * q.y - w.y * q.x), k * (w.y - q.y));
    return sqrt(d) * sign(s);
}

float VerticalCapsuleDistance(float3 p)
{
  p.y -= clamp(p.y, -.5, .5);
  return length(p) - .5;
}

float Mandelbulb(float3 p)
{
	float3 w = p;
    float m = dot(w, w);

	float dz = 1.0;
        
	for(int i = 0; i < 15; i++)
    {
        dz = 8 * pow(sqrt(m), 7.0) * dz + 1.0;
        float r = length(w);
        float b = 8 * acos(w.y / r);
        float a = 8 * atan2(w.x, w.z);
        w = p + pow(r, 8) * float3(sin(b) * sin(a), cos(b), sin(b) * cos(a));

        m = dot(w, w);
		if(m > 256.0)
            break;
    }
    return 0.25 * log(m) * sqrt(m) / dz;
}