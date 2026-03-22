#define TWO_PI 6.28318530718

float GetStep(float length, float initialRadius, float additionalRadius)
{
    float c = TWO_PI * (initialRadius + additionalRadius * 0.5f);
    if (length < c) return length / c;
    
    float stepCount = 1;
    length -= c;
    
    while (stepCount < 10)
    {
        c = c + TWO_PI * additionalRadius;
        if (length < c) return length / c + stepCount;
        stepCount++;
        length -= c;
    }
    return 0;
}

void Scroll_float(in float3 position, in float scrolledLength, in float initialRadius, 
    in float additionalRadius, in float sideRadiusMul, out float3 result)
{
    if (position.z < scrolledLength)
    {
        float multiplier = 1.0 + position.x * position.x * sideRadiusMul;
        
        float scrolledStep = GetStep(scrolledLength, initialRadius, additionalRadius);
        float vertexStep = GetStep(position.z, scrolledLength, additionalRadius);
        
        float scrolledAngle = scrolledStep * TWO_PI;
        float vertexAngle = vertexStep * TWO_PI;
        
        float scrolledRadius = initialRadius + additionalRadius * scrolledStep * multiplier;
        float vertexRadius = initialRadius + additionalRadius * vertexStep * multiplier;
        
        vertexAngle = vertexAngle - 1.57 - scrolledAngle;
        
        float x = vertexRadius * cos(vertexAngle);
        float y = vertexRadius * sin(vertexAngle);
        
        position.y = scrolledRadius + y;
        position.z = scrolledLength + x;
    }
    result = position;
}