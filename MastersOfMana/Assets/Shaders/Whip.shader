// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33068,y:32481,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32235,y:32601,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5531-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32823,y:32585,varname:node_2393,prsc:2|A-806-OUT,B-2053-RGB,C-797-RGB,D-9248-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32486,y:32779,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32486,y:32950,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32795,y:32736,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_TexCoord,id:9892,x:31789,y:32587,varname:node_9892,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:8248,x:32235,y:32790,ptovrint:False,ptlb:Fadeout,ptin:_Fadeout,varname:node_8248,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Blend,id:806,x:32486,y:32601,varname:node_806,prsc:2,blmd:1,clmp:True|SRC-6074-RGB,DST-8248-RGB;n:type:ShaderForge.SFN_ValueProperty,id:6330,x:31672,y:32962,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_6330,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Vector2,id:5220,x:31651,y:32761,varname:node_5220,prsc:2,v1:1,v2:0;n:type:ShaderForge.SFN_Multiply,id:3527,x:31897,y:32761,varname:node_3527,prsc:2|A-5220-OUT,B-6330-OUT,C-3841-T;n:type:ShaderForge.SFN_Add,id:5531,x:32024,y:32597,varname:node_5531,prsc:2|A-9892-UVOUT,B-3527-OUT;n:type:ShaderForge.SFN_Time,id:3841,x:31838,y:32909,varname:node_3841,prsc:2;proporder:6074-797-8248-6330;pass:END;sub:END;*/

Shader "Custom/Whip" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [HDR]_TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Fadeout ("Fadeout", 2D) = "white" {}
        _Speed ("Speed", Float ) = 1
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
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Fadeout; uniform float4 _Fadeout_ST;
            uniform float _Speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_3841 = _Time;
                float2 node_5531 = (i.uv0+(float2(1,0)*_Speed*node_3841.g));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5531, _MainTex));
                float4 _Fadeout_var = tex2D(_Fadeout,TRANSFORM_TEX(i.uv0, _Fadeout));
                float3 emissive = (saturate((_MainTex_var.rgb*_Fadeout_var.rgb))*i.vertexColor.rgb*_TintColor.rgb*2.0);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0.5,0.5,0.5,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
