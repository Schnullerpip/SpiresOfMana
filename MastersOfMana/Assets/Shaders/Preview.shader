// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.03,fgrn:5,fgrf:30,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33218,y:32674,varname:node_3138,prsc:2|emission-2138-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32621,y:32381,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:683,x:32355,y:33066,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_683,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:82e77c5667c7c4d6aa93d305e368308b,ntxv:0,isnm:False|UVIN-6027-OUT;n:type:ShaderForge.SFN_Code,id:9979,x:31997,y:32647,varname:node_9979,prsc:2,code:cgBlAHQAdQByAG4AIABmAGwAbwBhAHQAMgAoAF8AVABpAG0AZQAuAHgAKgB4ACwAXwBUAGkAbQBlAC4AeAAqAHkAKQA7AA==,output:1,fname:TimeGenerator,width:278,height:112,input:0,input:0,input_1_label:x,input_2_label:y|A-5616-OUT,B-6491-OUT;n:type:ShaderForge.SFN_TexCoord,id:8036,x:31955,y:33049,varname:node_8036,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:6027,x:32163,y:32945,varname:node_6027,prsc:2|A-9979-OUT,B-8036-UVOUT;n:type:ShaderForge.SFN_Slider,id:5588,x:32223,y:33321,ptovrint:False,ptlb:NoiseStrength,ptin:_NoiseStrength,varname:node_5588,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:2138,x:32977,y:32800,varname:node_2138,prsc:2|A-7241-RGB,B-7938-OUT,C-1067-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5616,x:31793,y:32639,ptovrint:False,ptlb:MovementX,ptin:_MovementX,varname:node_5616,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:6491,x:31793,y:32717,ptovrint:False,ptlb:MovementY,ptin:_MovementY,varname:node_6491,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5742,x:32596,y:33083,varname:node_5742,prsc:2|A-683-RGB,B-5588-OUT;n:type:ShaderForge.SFN_OneMinus,id:7938,x:32775,y:32954,varname:node_7938,prsc:2|IN-5742-OUT;n:type:ShaderForge.SFN_Code,id:4938,x:31793,y:32452,varname:node_4938,prsc:2,code:cgBlAHQAdQByAG4AIABzAGkAbgAoAF8AVABpAG0AZQAuAHgAIAAqACAAZgByAGUAcQApACAAKgAgAGEAbQBwACAAKgAgADAALgA1ACAAKwAgADAALgA1ACAAKwAgAG8AZgBmAHMAOwA=,output:0,fname:SinTime,width:592,height:131,input:0,input:0,input:0,input_1_label:amp,input_2_label:freq,input_3_label:offs|A-5056-OUT,B-4769-OUT,C-2352-OUT;n:type:ShaderForge.SFN_Multiply,id:1067,x:32647,y:32706,varname:node_1067,prsc:2|A-7241-A,B-4938-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5056,x:31564,y:32447,ptovrint:False,ptlb:Amplitude,ptin:_Amplitude,varname:node_5056,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:4769,x:31590,y:32507,ptovrint:False,ptlb:Frequency,ptin:_Frequency,varname:node_4769,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2352,x:31570,y:32604,ptovrint:False,ptlb:Offset,ptin:_Offset,varname:node_2352,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;proporder:7241-683-5588-5616-6491-5056-4769-2352;pass:END;sub:END;*/

Shader "Custom/Preview" {
    Properties {
        [HDR]_Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Noise ("Noise", 2D) = "white" {}
        _NoiseStrength ("NoiseStrength", Range(0, 1)) = 1
        _MovementX ("MovementX", Float ) = 0
        _MovementY ("MovementY", Float ) = 0
        _Amplitude ("Amplitude", Float ) = 0.5
        _Frequency ("Frequency", Float ) = 1
        _Offset ("Offset", Float ) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcColor
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles metal 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            float2 TimeGenerator( float x , float y ){
            return float2(_Time.x*x,_Time.x*y);
            }
            
            uniform float _NoiseStrength;
            uniform float _MovementX;
            uniform float _MovementY;
            float SinTime( float amp , float freq , float offs ){
            return sin(_Time.x * freq) * amp * 0.5 + 0.5 + offs;
            }
            
            uniform float _Amplitude;
            uniform float _Frequency;
            uniform float _Offset;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_6027 = (TimeGenerator( _MovementX , _MovementY )+i.uv0);
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_6027, _Noise));
                float3 emissive = (_Color.rgb*(1.0 - (_Noise_var.rgb*_NoiseStrength))*(_Color.a*SinTime( _Amplitude , _Frequency , _Offset )));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
