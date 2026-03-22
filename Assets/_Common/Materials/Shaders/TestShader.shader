Shader "Geometry"
{
    Properties
    {
        _Color("Color", color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
        }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vertexProgram
            #pragma fragment fragmentProgram
            #pragma geometry geometryProgram
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct vertexInput
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct geometryInput
            {
                float4 positionOS : POSITION;
            };
            
            struct fragmentInput
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float4 shadowCoords: TEXCOORD1;
            };
            
            struct fragmentOutput
            {
                float4 color : SV_Target;
            };
            
            float4 _Color;

            float3 GetObjectCenterWS()
            {
                return float3(
                    unity_ObjectToWorld._m03,
                    unity_ObjectToWorld._m13,
                    unity_ObjectToWorld._m23
                );
            }
            
            geometryInput vertexProgram(vertexInput v)
            {
                geometryInput o;
                o.positionOS = v.positionOS;
                return o;
            }
            
            [maxvertexcount(12)]
            void geometryProgram(triangle  geometryInput input[3], inout TriangleStream<fragmentInput> stream)
            {
                for (int i = 0; i < 3; ++i)
                {
                    float3 positionWS = TransformObjectToWorld(input[i].positionOS);
                    float3 positionVS = TransformWorldToView(positionWS);
                    
                    fragmentInput output;
                    output.normalWS = GetWorldSpaceNormalizeViewDir(positionWS);
                    output.shadowCoords = TransformWorldToShadowCoord(positionWS);
                    
                    output.positionCS = TransformWViewToHClip(positionVS + float3(-0.1, -0.1, 0));
                    stream.Append(output);
                    output.positionCS = TransformWViewToHClip(positionVS + float3(-0.1, 0.1, 0));
                    stream.Append(output);            
                    output.positionCS = TransformWViewToHClip(positionVS + float3(0.1, -0.1, 0));
                    stream.Append(output);
                    output.positionCS = TransformWViewToHClip(positionVS + float3(0.1, 0.1, 0));
                    stream.Append(output);
                    
                    stream.RestartStrip();
                }
            }
            
            float3 getAttenuatedLight(Light light)
            {
                float3 result = light.color * (light.distanceAttenuation * light.shadowAttenuation); 
                return result;
            }
            
            fragmentOutput fragmentProgram(fragmentInput i)
            {
                fragmentOutput o;
                o.color = _Color;
                
                Light mainLight = GetMainLight(i.shadowCoords);
                float3 attenuatedLightColor = getAttenuatedLight(mainLight);
                float3 lightColor = LightingLambert(attenuatedLightColor, mainLight.direction, i.normalWS);
                lightColor = saturate(lightColor + float3(0.5, 0.5, 0.5));
                
                o.color.rgb *= lightColor;
                
                return o;
            }
            ENDHLSL
        }
    }
}