// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:Standard,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33462,y:32751,varname:node_2865,prsc:2|diff-6343-OUT,spec-358-OUT,gloss-1813-OUT,normal-5964-RGB,emission-6303-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32738,y:32582,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:32506,y:32787,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:32485,y:32582,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-8211-OUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32648,y:32964,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9d67937a9d48dba4da1ce7730908aff1,ntxv:3,isnm:True|UVIN-8211-OUT;n:type:ShaderForge.SFN_Slider,id:358,x:32767,y:32757,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:_Metallic,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.5;n:type:ShaderForge.SFN_Slider,id:1813,x:32767,y:32852,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.06;n:type:ShaderForge.SFN_Tex2d,id:6759,x:32400,y:33047,ptovrint:False,ptlb:EmissionMap,ptin:_EmissionMap,varname:_EmissionMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4f15f9c6bc22fd543a3543620aaa8c71,ntxv:2,isnm:False|UVIN-8211-OUT;n:type:ShaderForge.SFN_Multiply,id:4108,x:32733,y:33234,varname:node_4108,prsc:2|A-6759-RGB,B-9258-RGB,C-4078-OUT;n:type:ShaderForge.SFN_Color,id:9258,x:32208,y:33251,ptovrint:False,ptlb:EmissionColor,ptin:_EmissionColor,varname:_EmissionColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.4352942,c3:0,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:4078,x:32416,y:33345,ptovrint:False,ptlb:EmissionStrength,ptin:_EmissionStrength,varname:_EmissionStrength,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_TexCoord,id:8360,x:31315,y:32264,varname:node_8360,prsc:2,uv:0,uaff:True;n:type:ShaderForge.SFN_Append,id:1752,x:31632,y:32724,varname:node_1752,prsc:2|A-4770-OUT,B-8252-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8983,x:30394,y:32906,ptovrint:False,ptlb:amplitude,ptin:_amplitude,varname:_amplitude,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_ValueProperty,id:8285,x:30394,y:32715,ptovrint:False,ptlb:frequency,ptin:_frequency,varname:_frequency,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.01;n:type:ShaderForge.SFN_Add,id:8211,x:31841,y:32613,varname:node_8211,prsc:2|A-8360-UVOUT,B-1752-OUT,C-5688-OUT;n:type:ShaderForge.SFN_Time,id:7955,x:30394,y:32557,varname:node_7955,prsc:2;n:type:ShaderForge.SFN_Sin,id:1624,x:31117,y:32768,varname:node_1624,prsc:2|IN-4867-OUT;n:type:ShaderForge.SFN_Multiply,id:8252,x:31363,y:32889,varname:node_8252,prsc:2|A-1624-OUT,B-8983-OUT;n:type:ShaderForge.SFN_Add,id:4867,x:30940,y:32768,varname:node_4867,prsc:2|A-8998-OUT,B-3801-OUT;n:type:ShaderForge.SFN_Multiply,id:8998,x:30662,y:32575,varname:node_8998,prsc:2|A-7955-T,B-8285-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3801,x:30394,y:32811,ptovrint:False,ptlb:offset,ptin:_offset,varname:_offset,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Sin,id:2844,x:31117,y:32577,varname:node_2844,prsc:2|IN-8998-OUT;n:type:ShaderForge.SFN_Multiply,id:4770,x:31359,y:32577,varname:node_4770,prsc:2|A-2844-OUT,B-8983-OUT;n:type:ShaderForge.SFN_Tex2d,id:741,x:32505,y:33458,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_741,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7f1cd443fabe3b34ab368f436b25ebf4,ntxv:0,isnm:False|UVIN-311-OUT;n:type:ShaderForge.SFN_Multiply,id:6303,x:33188,y:33240,varname:node_6303,prsc:2|A-4108-OUT,B-1042-OUT;n:type:ShaderForge.SFN_Slider,id:2217,x:32326,y:33710,ptovrint:False,ptlb:NoiseDarkening,ptin:_NoiseDarkening,varname:node_2217,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Multiply,id:9001,x:32711,y:33429,varname:node_9001,prsc:2|A-741-RGB,B-2217-OUT;n:type:ShaderForge.SFN_OneMinus,id:1042,x:32974,y:33387,varname:node_1042,prsc:2|IN-9001-OUT;n:type:ShaderForge.SFN_Time,id:3915,x:31734,y:33604,varname:node_3915,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:3549,x:31885,y:33437,varname:node_3549,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Append,id:3247,x:32131,y:33597,varname:node_3247,prsc:2|A-4473-OUT,B-6426-OUT;n:type:ShaderForge.SFN_Add,id:311,x:32326,y:33496,varname:node_311,prsc:2|A-3549-UVOUT,B-3247-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9507,x:31790,y:33773,ptovrint:False,ptlb:noiseXSpeed,ptin:_noiseXSpeed,varname:node_9507,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:4417,x:31790,y:33843,ptovrint:False,ptlb:noiseYSpeed,ptin:_noiseYSpeed,varname:node_4417,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:4473,x:31958,y:33621,varname:node_4473,prsc:2|A-3915-T,B-9507-OUT;n:type:ShaderForge.SFN_Multiply,id:6426,x:31969,y:33753,varname:node_6426,prsc:2|A-3915-T,B-4417-OUT;n:type:ShaderForge.SFN_Tex2d,id:5648,x:31874,y:32272,ptovrint:False,ptlb:Turbulance,ptin:_Turbulance,varname:node_5648,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-5004-OUT;n:type:ShaderForge.SFN_Multiply,id:5688,x:32121,y:32200,varname:node_5688,prsc:2|A-9303-B,B-5648-G;n:type:ShaderForge.SFN_Time,id:8561,x:30693,y:31910,varname:node_8561,prsc:2;n:type:ShaderForge.SFN_Append,id:4726,x:31170,y:32002,varname:node_4726,prsc:2|A-1348-OUT,B-8453-OUT;n:type:ShaderForge.SFN_Add,id:5004,x:31542,y:32107,varname:node_5004,prsc:2|A-4726-OUT,B-8360-UVOUT;n:type:ShaderForge.SFN_Vector4Property,id:4237,x:30382,y:32108,ptovrint:False,ptlb:TurbolanceSettings,ptin:_TurbolanceSettings,varname:node_4237,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Multiply,id:1348,x:30968,y:31930,varname:node_1348,prsc:2|A-9303-R,B-8561-T;n:type:ShaderForge.SFN_Multiply,id:8453,x:30968,y:32047,varname:node_8453,prsc:2|A-9303-G,B-8561-T;n:type:ShaderForge.SFN_Vector1,id:6202,x:30421,y:32284,varname:node_6202,prsc:2,v1:0.016;n:type:ShaderForge.SFN_Multiply,id:6706,x:30597,y:32123,varname:node_6706,prsc:2|A-4237-XYZ,B-6202-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9303,x:30772,y:32123,varname:node_9303,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-6706-OUT;proporder:7736-6665-358-1813-5964-6759-4078-9258-8983-8285-3801-741-2217-9507-4417-5648-4237;pass:END;sub:END;*/

Shader "Custom/Lava" {
    Properties {
        _MainTex ("Base Color", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0, 0.5)) = 0
        _Gloss ("Gloss", Range(0, 0.06)) = 0
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _EmissionMap ("EmissionMap", 2D) = "black" {}
        _EmissionStrength ("EmissionStrength", Float ) = 2
        _EmissionColor ("EmissionColor", Color) = (1,0.4352942,0,1)
        _amplitude ("amplitude", Float ) = 0.2
        _frequency ("frequency", Float ) = 0.01
        _offset ("offset", Float ) = 1
        _Noise ("Noise", 2D) = "white" {}
        _NoiseDarkening ("NoiseDarkening", Range(0, 1)) = 0.5
        _noiseXSpeed ("noiseXSpeed", Float ) = 0
        _noiseYSpeed ("noiseYSpeed", Float ) = 0
        _Turbulance ("Turbulance", 2D) = "white" {}
        _TurbolanceSettings ("TurbolanceSettings", Vector) = (0,0,0,0)
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
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissionStrength;
            uniform float _amplitude;
            uniform float _frequency;
            uniform float _offset;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _NoiseDarkening;
            uniform float _noiseXSpeed;
            uniform float _noiseYSpeed;
            uniform sampler2D _Turbulance; uniform float4 _Turbulance_ST;
            uniform float4 _TurbolanceSettings;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
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
                float4 node_7955 = _Time;
                float node_8998 = (node_7955.g*_frequency);
                float3 node_9303 = (_TurbolanceSettings.rgb*0.016).rgb;
                float4 node_8561 = _Time;
                float2 node_5004 = (float2((node_9303.r*node_8561.g),(node_9303.g*node_8561.g))+i.uv0);
                float4 _Turbulance_var = tex2D(_Turbulance,TRANSFORM_TEX(node_5004, _Turbulance));
                float2 node_8211 = (i.uv0+float2((sin(node_8998)*_amplitude),(sin((node_8998+_offset))*_amplitude))+(node_9303.b*_Turbulance_var.g));
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_8211, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
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
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8211, _MainTex));
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
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(node_8211, _EmissionMap));
                float4 node_3915 = _Time;
                float2 node_311 = (i.uv0+float2((node_3915.g*_noiseXSpeed),(node_3915.g*_noiseYSpeed)));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_311, _Noise));
                float3 emissive = ((_EmissionMap_var.rgb*_EmissionColor.rgb*_EmissionStrength)*(1.0 - (_Noise_var.rgb*_NoiseDarkening)));
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
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissionStrength;
            uniform float _amplitude;
            uniform float _frequency;
            uniform float _offset;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _NoiseDarkening;
            uniform float _noiseXSpeed;
            uniform float _noiseYSpeed;
            uniform sampler2D _Turbulance; uniform float4 _Turbulance_ST;
            uniform float4 _TurbolanceSettings;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
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
                float4 node_7955 = _Time;
                float node_8998 = (node_7955.g*_frequency);
                float3 node_9303 = (_TurbolanceSettings.rgb*0.016).rgb;
                float4 node_8561 = _Time;
                float2 node_5004 = (float2((node_9303.r*node_8561.g),(node_9303.g*node_8561.g))+i.uv0);
                float4 _Turbulance_var = tex2D(_Turbulance,TRANSFORM_TEX(node_5004, _Turbulance));
                float2 node_8211 = (i.uv0+float2((sin(node_8998)*_amplitude),(sin((node_8998+_offset))*_amplitude))+(node_9303.b*_Turbulance_var.g));
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_8211, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
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
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8211, _MainTex));
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
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissionStrength;
            uniform float _amplitude;
            uniform float _frequency;
            uniform float _offset;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _NoiseDarkening;
            uniform float _noiseXSpeed;
            uniform float _noiseYSpeed;
            uniform sampler2D _Turbulance; uniform float4 _Turbulance_ST;
            uniform float4 _TurbolanceSettings;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
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
                
                float4 node_7955 = _Time;
                float node_8998 = (node_7955.g*_frequency);
                float3 node_9303 = (_TurbolanceSettings.rgb*0.016).rgb;
                float4 node_8561 = _Time;
                float2 node_5004 = (float2((node_9303.r*node_8561.g),(node_9303.g*node_8561.g))+i.uv0);
                float4 _Turbulance_var = tex2D(_Turbulance,TRANSFORM_TEX(node_5004, _Turbulance));
                float2 node_8211 = (i.uv0+float2((sin(node_8998)*_amplitude),(sin((node_8998+_offset))*_amplitude))+(node_9303.b*_Turbulance_var.g));
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(node_8211, _EmissionMap));
                float4 node_3915 = _Time;
                float2 node_311 = (i.uv0+float2((node_3915.g*_noiseXSpeed),(node_3915.g*_noiseYSpeed)));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_311, _Noise));
                o.Emission = ((_EmissionMap_var.rgb*_EmissionColor.rgb*_EmissionStrength)*(1.0 - (_Noise_var.rgb*_NoiseDarkening)));
                
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8211, _MainTex));
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
