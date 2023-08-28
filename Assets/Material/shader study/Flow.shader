Shader "Custom/Flow"
{
    Properties{
        _MainTex("MainTex",2D)="white"{}
        _MainColor("MainColor",Color)=(1,1,1,1)
        _FlowTex("FlowTex",2D)="white"{}
        _FlowColor("FlowColor",Color)=(1,1,1,1)
        _FlowSpeed("FlowSpeed",Range(0,2))=1.0
    }
    SubShader{
        Tags{"Queue"="Transparent" "IgnoreProjector"="True " "RenderType"="Transparent"}
        Pass{
            Tags{"LightMode"="ForwardBase"}
            ZWrite off
            Cull off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma enable_d3d11_debug_symbols
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            sampler2D _FlowTex;
            fixed4 _MainTex_ST;
            fixed4 _MainColor;
            fixed4 _FlowTex_ST;
            fixed _FlowSpeed;
            fixed4 _FlowColor;

            //这两个结构体代表的只是顶点的颜色，顶点最终只是用来对三角形内的像素进行插值赋值的，并无实际效果。
            //这个代表原始的，未经任何变化的顶点信息，相当于声明变量。
            struct raw_vert{
                float4 vertex:POSITION;
                float4 vertex_co:COLOR0;
                float2 texcoord_main:TEXCOORD0;
                float2 texcoord_flow:TEXCOORD1;
            };

            //这个代表最终用到的顶点信息。当然在对像素赋值时，直接用这个就行，自动插值过了。相当于对变量进行初始化。
            struct trans_vert{
                float4 pos:SV_POSITION;
                float4 co:COLOR0;
                float2 main_uv:TEXCOORD0;
                float2 flow_uv:TEXCOORD1;
            };
 
            //顶点信息初始化
            trans_vert vert(raw_vert v){
                trans_vert o;
                o.pos = normalize(UnityObjectToClipPos(v.vertex));
                o.main_uv.xy = v.texcoord_main * _MainTex_ST.xy + _MainTex_ST.zw; //注意这里和正常思维是相反的。
                o.flow_uv.xy = v.texcoord_flow * _FlowTex_ST.xy + _FlowTex_ST.zw;
                o.co = v.vertex_co;
                return o;
            }
            //使用插值后的顶点信息对像素进行赋值。
            fixed4 frag(trans_vert i):SV_Target{
                fixed4 texmain_co = tex2D(_MainTex, i.main_uv.xy) * _MainColor;   //根据纹理坐标获取到纹理上该像素的颜色。
                i.flow_uv.xy += _Time.y * _FlowSpeed;   //流光划过，注意因为这里是插值后的信息，对于原来的顶点的信息没有影响。
                fixed4 texflow_co = tex2D(_FlowTex, i.flow_uv.xy) * _FlowColor;
                fixed3 color = texmain_co + texflow_co + i.co;
                return fixed4(color, i.co.a);  //透明度在颜色之后施加，所以是两个纹理的颜色之和的透明度
            }
            ENDCG
 
        }
        
    }
}