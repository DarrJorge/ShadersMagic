void SampleAnimation_float(in UnityTexture2D animTexture, in float vertexId, in float time, in float frameRate,
    out float3 position, out float3 normal)
{
    float frameCount = animTexture.texelSize.z;
    float duration = frameCount / frameRate;
    float normalizedTime = (time / duration) % 1;
    
    float positionY = (vertexId * 2 + 0.5) * animTexture.texelSize.y;
    float2 positionUV = float2(normalizedTime, positionY);
    position = animTexture.SampleLevel(animTexture.samplerstate, positionUV, 0).xyz;
    
    float normalY = (vertexId * 2 + 1.5) * animTexture.texelSize.y;
    float2 normalUV = float2(normalizedTime, normalY);
    normal = animTexture.SampleLevel(animTexture.samplerstate, normalUV, 0).xyz;
}