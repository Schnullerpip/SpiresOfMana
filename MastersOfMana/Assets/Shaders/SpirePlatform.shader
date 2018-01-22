// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:Standard,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33662,y:33163,varname:node_2865,prsc:2|diff-6343-OUT,spec-358-OUT,gloss-1813-OUT,normal-2915-OUT,emission-7470-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32387,y:32195,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:32194,y:32288,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:32194,y:32103,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c3cfe7d2684045147a1989894b810ce9,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32032,y:33007,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7eae89110ca57954d980b51193786cb1,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:358,x:32359,y:32391,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32317,y:32484,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Tex2d,id:1772,x:31712,y:33452,ptovrint:False,ptlb:SecondaryNormalMap,ptin:_SecondaryNormalMap,varname:node_1772,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:299df2f6a26d2e94ba683c706174da1b,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:8247,x:32210,y:33108,varname:node_8247,prsc:2|A-5964-RGB,B-6958-OUT;n:type:ShaderForge.SFN_Append,id:6958,x:32032,y:33192,varname:node_6958,prsc:2|A-5891-OUT,B-5891-OUT,C-3732-OUT;n:type:ShaderForge.SFN_Vector1,id:3732,x:31607,y:33284,varname:node_3732,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:5891,x:31742,y:33191,ptovrint:False,ptlb:NormalStrength,ptin:_NormalStrength,varname:node_5891,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:9165,x:31688,y:33363,ptovrint:False,ptlb:SecondaryNormalStrength,ptin:_SecondaryNormalStrength,varname:node_9165,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Append,id:739,x:31917,y:33335,varname:node_739,prsc:2|A-9165-OUT,B-9165-OUT,C-3732-OUT;n:type:ShaderForge.SFN_Multiply,id:6739,x:32107,y:33335,varname:node_6739,prsc:2|A-739-OUT,B-1772-RGB;n:type:ShaderForge.SFN_Add,id:5135,x:32386,y:33187,varname:node_5135,prsc:2|A-8247-OUT,B-6739-OUT;n:type:ShaderForge.SFN_Append,id:2915,x:32926,y:33210,varname:node_2915,prsc:2|A-338-R,B-338-G,C-4732-OUT;n:type:ShaderForge.SFN_ComponentMask,id:338,x:32571,y:33187,varname:node_338,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-5135-OUT;n:type:ShaderForge.SFN_Multiply,id:4732,x:32775,y:33312,varname:node_4732,prsc:2|A-338-B,B-3620-OUT;n:type:ShaderForge.SFN_Vector1,id:3620,x:32571,y:33384,varname:node_3620,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Tex2d,id:1304,x:31760,y:33758,ptovrint:False,ptlb:EmissionMask,ptin:_EmissionMask,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:11ebba2ab90456b4c93a366654837c7c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2531,x:31883,y:33961,ptovrint:False,ptlb:EmissionNoise,ptin:_EmissionNoise,varname:node_2531,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1579dbac07f3945dcb4c555af4956977,ntxv:0,isnm:False|UVIN-3060-UVOUT;n:type:ShaderForge.SFN_Blend,id:2048,x:32779,y:33885,varname:node_2048,prsc:2,blmd:1,clmp:True|SRC-1304-RGB,DST-1528-OUT;n:type:ShaderForge.SFN_Rotator,id:3060,x:31658,y:33992,varname:node_3060,prsc:2|UVIN-6851-UVOUT,SPD-8684-OUT;n:type:ShaderForge.SFN_TexCoord,id:6851,x:31442,y:33968,varname:node_6851,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:8684,x:31442,y:34156,ptovrint:False,ptlb:NoiseRotationSpeed,ptin:_NoiseRotationSpeed,varname:node_8684,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Color,id:2236,x:32142,y:33561,ptovrint:False,ptlb:node_2236,ptin:_node_2236,varname:node_2236,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:4237,x:31551,y:34404,ptovrint:False,ptlb:EmissionColor,ptin:_EmissionColor,varname:node_4237,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:0.3379309,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:1186,x:31326,y:34673,ptovrint:False,ptlb:EmissionStrength,ptin:_EmissionStrength,varname:node_1186,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:482,x:31788,y:34404,varname:node_482,prsc:2|A-4237-RGB,B-8394-OUT;n:type:ShaderForge.SFN_Multiply,id:7470,x:33047,y:34106,varname:node_7470,prsc:2|A-2048-OUT,B-482-OUT;n:type:ShaderForge.SFN_Rotator,id:7060,x:31693,y:34204,varname:node_7060,prsc:2|UVIN-6851-UVOUT,SPD-3429-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3429,x:31421,y:34250,ptovrint:False,ptlb:NoiseRotationSpeed2,ptin:_NoiseRotationSpeed2,varname:node_3429,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-1;n:type:ShaderForge.SFN_Tex2d,id:6568,x:31909,y:34176,ptovrint:False,ptlb:EmissionNoise2,ptin:_EmissionNoise2,varname:node_6568,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1579dbac07f3945dcb4c555af4956977,ntxv:1,isnm:False|UVIN-7060-UVOUT;n:type:ShaderForge.SFN_Blend,id:363,x:32104,y:33995,varname:node_363,prsc:2,blmd:10,clmp:True|SRC-2531-RGB,DST-6568-RGB;n:type:ShaderForge.SFN_Power,id:1528,x:32494,y:34044,varname:node_1528,prsc:2|VAL-363-OUT,EXP-6101-OUT;n:type:ShaderForge.SFN_Slider,id:6101,x:32093,y:34231,ptovrint:False,ptlb:NoiseSharpness,ptin:_NoiseSharpness,varname:node_6101,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:9552,x:31276,y:34560,ptovrint:False,ptlb:EmissionDamping,ptin:_EmissionDamping,varname:node_9552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:8394,x:31627,y:34601,varname:node_8394,prsc:2|A-1186-OUT,B-9552-OUT;proporder:7736-6665-5964-5891-358-1813-1772-9165-1304-4237-1186-2531-8684-6568-3429-6101-9552;pass:END;sub:END;*/

Shader "Custom/SpirePlatform" {
    Properties {
        _MainTex ("Base Color", 2D) = "black" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("NormalStrength", Float ) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.1
        _SecondaryNormalMap ("SecondaryNormalMap", 2D) = "bump" {}
        _SecondaryNormalStrength ("SecondaryNormalStrength", Float ) = 1
        _EmissionMask ("EmissionMask", 2D) = "white" {}
        _EmissionColor ("EmissionColor", Color) = (0,1,0.3379309,1)
        _EmissionStrength ("EmissionStrength", Float ) = 1
        _EmissionNoise ("EmissionNoise", 2D) = "white" {}
        _NoiseRotationSpeed ("NoiseRotationSpeed", Float ) = 2
        _EmissionNoise2 ("EmissionNoise2", 2D) = "gray" {}
        _NoiseRotationSpeed2 ("NoiseRotationSpeed2", Float ) = -1
        _NoiseSharpness ("NoiseSharpness", Range(1, 10)) = 1
        _EmissionDamping ("EmissionDamping", Range(0, 1)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles metal 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _SecondaryNormalMap; uniform float4 _SecondaryNormalMap_ST;
            uniform float _NormalStrength;
            uniform float _SecondaryNormalStrength;
            uniform sampler2D _EmissionMask; uniform float4 _EmissionMask_ST;
            uniform sampler2D _EmissionNoise; uniform float4 _EmissionNoise_ST;
            uniform float _NoiseRotationSpeed;
            uniform float4 _EmissionColor;
            uniform float _EmissionStrength;
            uniform float _NoiseRotationSpeed2;
            uniform sampler2D _EmissionNoise2; uniform float4 _EmissionNoise2_ST;
            uniform float _NoiseSharpness;
            uniform float _EmissionDamping;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float node_3732 = 1.0;
                float3 _SecondaryNormalMap_var = UnpackNormal(tex2D(_SecondaryNormalMap,TRANSFORM_TEX(i.uv0, _SecondaryNormalMap)));
                float3 node_338 = ((_BumpMap_var.rgb*float3(_NormalStrength,_NormalStrength,node_3732))+(float3(_SecondaryNormalStrength,_SecondaryNormalStrength,node_3732)*_SecondaryNormalMap_var.rgb)).rgb;
                float3 normalLocal = float3(node_338.r,node_338.g,(node_338.b*0.5));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (_MainTex_var.rgb*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _EmissionMask_var = tex2D(_EmissionMask,TRANSFORM_TEX(i.uv0, _EmissionMask));
                float4 node_5890 = _Time;
                float node_3060_ang = node_5890.g;
                float node_3060_spd = _NoiseRotationSpeed;
                float node_3060_cos = cos(node_3060_spd*node_3060_ang);
                float node_3060_sin = sin(node_3060_spd*node_3060_ang);
                float2 node_3060_piv = float2(0.5,0.5);
                float2 node_3060 = (mul(i.uv0-node_3060_piv,float2x2( node_3060_cos, -node_3060_sin, node_3060_sin, node_3060_cos))+node_3060_piv);
                float4 _EmissionNoise_var = tex2D(_EmissionNoise,TRANSFORM_TEX(node_3060, _EmissionNoise));
                float node_7060_ang = node_5890.g;
                float node_7060_spd = _NoiseRotationSpeed2;
                float node_7060_cos = cos(node_7060_spd*node_7060_ang);
                float node_7060_sin = sin(node_7060_spd*node_7060_ang);
                float2 node_7060_piv = float2(0.5,0.5);
                float2 node_7060 = (mul(i.uv0-node_7060_piv,float2x2( node_7060_cos, -node_7060_sin, node_7060_sin, node_7060_cos))+node_7060_piv);
                float4 _EmissionNoise2_var = tex2D(_EmissionNoise2,TRANSFORM_TEX(node_7060, _EmissionNoise2));
                float3 emissive = (saturate((_EmissionMask_var.rgb*pow(saturate(( _EmissionNoise2_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_EmissionNoise2_var.rgb-0.5))*(1.0-_EmissionNoise_var.rgb)) : (2.0*_EmissionNoise2_var.rgb*_EmissionNoise_var.rgb) )),_NoiseSharpness)))*(_EmissionColor.rgb*(_EmissionStrength*_EmissionDamping)));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles metal 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _SecondaryNormalMap; uniform float4 _SecondaryNormalMap_ST;
            uniform float _NormalStrength;
            uniform float _SecondaryNormalStrength;
            uniform sampler2D _EmissionMask; uniform float4 _EmissionMask_ST;
            uniform sampler2D _EmissionNoise; uniform float4 _EmissionNoise_ST;
            uniform float _NoiseRotationSpeed;
            uniform float4 _EmissionColor;
            uniform float _EmissionStrength;
            uniform float _NoiseRotationSpeed2;
            uniform sampler2D _EmissionNoise2; uniform float4 _EmissionNoise2_ST;
            uniform float _NoiseSharpness;
            uniform float _EmissionDamping;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float node_3732 = 1.0;
                float3 _SecondaryNormalMap_var = UnpackNormal(tex2D(_SecondaryNormalMap,TRANSFORM_TEX(i.uv0, _SecondaryNormalMap)));
                float3 node_338 = ((_BumpMap_var.rgb*float3(_NormalStrength,_NormalStrength,node_3732))+(float3(_SecondaryNormalStrength,_SecondaryNormalStrength,node_3732)*_SecondaryNormalMap_var.rgb)).rgb;
                float3 normalLocal = float3(node_338.r,node_338.g,(node_338.b*0.5));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (_MainTex_var.rgb*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles metal 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _EmissionMask; uniform float4 _EmissionMask_ST;
            uniform sampler2D _EmissionNoise; uniform float4 _EmissionNoise_ST;
            uniform float _NoiseRotationSpeed;
            uniform float4 _EmissionColor;
            uniform float _EmissionStrength;
            uniform float _NoiseRotationSpeed2;
            uniform sampler2D _EmissionNoise2; uniform float4 _EmissionNoise2_ST;
            uniform float _NoiseSharpness;
            uniform float _EmissionDamping;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 _EmissionMask_var = tex2D(_EmissionMask,TRANSFORM_TEX(i.uv0, _EmissionMask));
                float4 node_6660 = _Time;
                float node_3060_ang = node_6660.g;
                float node_3060_spd = _NoiseRotationSpeed;
                float node_3060_cos = cos(node_3060_spd*node_3060_ang);
                float node_3060_sin = sin(node_3060_spd*node_3060_ang);
                float2 node_3060_piv = float2(0.5,0.5);
                float2 node_3060 = (mul(i.uv0-node_3060_piv,float2x2( node_3060_cos, -node_3060_sin, node_3060_sin, node_3060_cos))+node_3060_piv);
                float4 _EmissionNoise_var = tex2D(_EmissionNoise,TRANSFORM_TEX(node_3060, _EmissionNoise));
                float node_7060_ang = node_6660.g;
                float node_7060_spd = _NoiseRotationSpeed2;
                float node_7060_cos = cos(node_7060_spd*node_7060_ang);
                float node_7060_sin = sin(node_7060_spd*node_7060_ang);
                float2 node_7060_piv = float2(0.5,0.5);
                float2 node_7060 = (mul(i.uv0-node_7060_piv,float2x2( node_7060_cos, -node_7060_sin, node_7060_sin, node_7060_cos))+node_7060_piv);
                float4 _EmissionNoise2_var = tex2D(_EmissionNoise2,TRANSFORM_TEX(node_7060, _EmissionNoise2));
                o.Emission = (saturate((_EmissionMask_var.rgb*pow(saturate(( _EmissionNoise2_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_EmissionNoise2_var.rgb-0.5))*(1.0-_EmissionNoise_var.rgb)) : (2.0*_EmissionNoise2_var.rgb*_EmissionNoise_var.rgb) )),_NoiseSharpness)))*(_EmissionColor.rgb*(_EmissionStrength*_EmissionDamping)));
                
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffColor = (_MainTex_var.rgb*_Color.rgb);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Standard"
    CustomEditor "ShaderForgeMaterialInspector"
}
