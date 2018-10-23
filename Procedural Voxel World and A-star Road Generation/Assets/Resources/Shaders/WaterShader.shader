// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.9803922,fgcg:0.3215686,fgcb:0,fgca:1,fgde:0.0015,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9981,x:32719,y:32712,varname:node_9981,prsc:2|diff-8987-OUT,alpha-1465-OUT,refract-8871-OUT;n:type:ShaderForge.SFN_Tex2d,id:7570,x:32064,y:33098,ptovrint:False,ptlb:refraction,ptin:_refraction,varname:node_7570,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-4303-OUT;n:type:ShaderForge.SFN_Slider,id:2976,x:32085,y:33344,ptovrint:False,ptlb:distortion,ptin:_distortion,varname:node_2976,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.02,max:1;n:type:ShaderForge.SFN_Multiply,id:3947,x:32459,y:33265,varname:node_3947,prsc:2|A-2976-OUT,B-4589-OUT;n:type:ShaderForge.SFN_Panner,id:8751,x:31573,y:33099,varname:node_8751,prsc:2,spu:0.005,spv:0.005|UVIN-37-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:37,x:31390,y:33099,varname:node_37,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:8137,x:31235,y:33577,ptovrint:False,ptlb:timeMult,ptin:_timeMult,varname:node_8137,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:4303,x:31596,y:33418,varname:node_4303,prsc:2|A-8751-UVOUT,B-8137-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7364,x:32322,y:32985,varname:node_7364,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7570-RGB;n:type:ShaderForge.SFN_DepthBlend,id:4589,x:32222,y:33428,varname:node_4589,prsc:2|DIST-2090-OUT;n:type:ShaderForge.SFN_Slider,id:2090,x:31896,y:33455,ptovrint:False,ptlb:distance,ptin:_distance,varname:node_2090,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Multiply,id:8871,x:32502,y:33095,varname:node_8871,prsc:2|A-7364-OUT,B-3947-OUT;n:type:ShaderForge.SFN_Color,id:4680,x:31744,y:32734,ptovrint:False,ptlb:color,ptin:_color,varname:node_4680,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.388,c2:0.772,c3:0.812,c4:1;n:type:ShaderForge.SFN_DepthBlend,id:1826,x:31744,y:32889,varname:node_1826,prsc:2|DIST-5251-OUT;n:type:ShaderForge.SFN_Color,id:391,x:31744,y:32573,ptovrint:False,ptlb:foamColor,ptin:_foamColor,varname:node_391,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:8987,x:32034,y:32715,varname:node_8987,prsc:2|A-391-RGB,B-4680-RGB,T-1826-OUT;n:type:ShaderForge.SFN_Slider,id:5251,x:31396,y:32888,ptovrint:False,ptlb:foamDepth,ptin:_foamDepth,varname:node_5251,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:1465,x:32361,y:32904,ptovrint:False,ptlb:opacity,ptin:_opacity,varname:node_1465,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:7570-4680-1465-2976-8137-2090-391-5251;pass:END;sub:END;*/

Shader "Custom/WaterShader" {
    Properties {
        _refraction ("refraction", 2D) = "bump" {}
        _color ("color", Color) = (0.388,0.772,0.812,1)
        _opacity ("opacity", Range(0, 1)) = 0
        _distortion ("distortion", Range(0, 1)) = 0.02
        _timeMult ("timeMult", Range(0, 1)) = 0
        _distance ("distance", Range(0, 10)) = 0
        _foamColor ("foamColor", Color) = (1,1,1,1)
        _foamDepth ("foamDepth", Range(0, 1)) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _refraction; uniform float4 _refraction_ST;
            uniform float _distortion;
            uniform float _timeMult;
            uniform float _distance;
            uniform float4 _color;
            uniform float4 _foamColor;
            uniform float _foamDepth;
            uniform float _opacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_5885 = _Time;
                float2 node_4303 = ((i.uv0+node_5885.g*float2(0.005,0.005))*_timeMult);
                float3 _refraction_var = UnpackNormal(tex2D(_refraction,TRANSFORM_TEX(node_4303, _refraction)));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (_refraction_var.rgb.rg*(_distortion*saturate((sceneZ-partZ)/_distance)));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = lerp(_foamColor.rgb,_color.rgb,saturate((sceneZ-partZ)/_foamDepth));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,_opacity),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _refraction; uniform float4 _refraction_ST;
            uniform float _distortion;
            uniform float _timeMult;
            uniform float _distance;
            uniform float4 _color;
            uniform float4 _foamColor;
            uniform float _foamDepth;
            uniform float _opacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
                LIGHTING_COORDS(4,5)
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_3000 = _Time;
                float2 node_4303 = ((i.uv0+node_3000.g*float2(0.005,0.005))*_timeMult);
                float3 _refraction_var = UnpackNormal(tex2D(_refraction,TRANSFORM_TEX(node_4303, _refraction)));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (_refraction_var.rgb.rg*(_distortion*saturate((sceneZ-partZ)/_distance)));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuseColor = lerp(_foamColor.rgb,_color.rgb,saturate((sceneZ-partZ)/_foamDepth));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * _opacity,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
