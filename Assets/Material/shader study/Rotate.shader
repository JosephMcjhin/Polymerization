Shader "Custom/Rotate"
{
    Properties{
        _MainTex("MainTex",2D)="white"{}
        _Rotate("Rotate",Float)=0
    }
    SubShader{
        Tags{"Queue"="Transparent" "IgnoreProjector"="True " "RenderType"="Transparent"}
        Pass{
            Tags{"LightMode"="ForwardBase"}
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            fixed _Rotate;

            //这两个结构体代表的只是顶点的颜色，顶点最终只是用来对三角形内的像素进行插值赋值的，并无实际效果。
            //这个代表原始的，未经任何变化的顶点信息，相当于声明变量。
            struct raw_vert{
                float4 vertex:POSITION;
                float2 texcoord_main:TEXCOORD0;
                float2 texcoord_flow:TEXCOORD1;
            };

            //这个代表最终用到的顶点信息。当然在对像素赋值时，直接用这个就行，自动插值过了。相当于对变量进行初始化。
            struct trans_vert{
                float4 pos:SV_POSITION;
                float2 main_uv:TEXCOORD0;
                float2 flow_uv:TEXCOORD1;
            };
 
            //顶点信息初始化
            trans_vert vert(raw_vert v){
                trans_vert o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.main_uv.xy = v.texcoord_main * _MainTex_ST.xy + _MainTex_ST.zw; //注意这里和正常思维是相反的。
                return o;
            }
            //使用插值后的顶点信息对像素进行赋值。
            fixed4 frag(trans_vert i):SV_Target{
                fixed2 dis = i.main_uv.xy - fixed2(0.5,0.5);
                fixed length = sqrt(dot(dis,dis));
                float rotate_angle = length * _Rotate;
                fixed2x2 rotate_matrix = {
                    cos(rotate_angle), sin(rotate_angle),
                    -sin(rotate_angle), cos(rotate_angle)
                };
                fixed4 tex_color = tex2D(_MainTex, fixed2(0.5,0.5) + mul(dis, rotate_matrix));
                return tex_color;
            }
            ENDCG
 
        }
        
    }
}