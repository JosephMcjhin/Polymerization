Shader "Custom/Cellshader"
{
    Properties{
        _tag1("tag1",Color) = (0,0,0,1)
        _tag2("tag2",Color) = (0,0,0,1)
        _tag3("tag3",Color) = (0,0,0,1)
        _tag4("tag4",Color) = (0,0,0,1)
        _sp("speed",Float) = 0
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

            fixed4 _tag1;
            fixed4 _tag2;
            fixed4 _tag3;
            fixed4 _tag4;
            float _sp;


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
                o.main_uv.xy = v.texcoord_main; //注意这里和正常思维是相反的。
                return o;
            }

            fixed3 pal(float t, fixed3 a, fixed3 b, fixed3 c, fixed3 d ){
                return a + b*(cos( 6.28318*(c*t+d) ));
            }

            //使用插值后的顶点信息对像素进行赋值。
            fixed4 frag(trans_vert i):SV_Target{
                fixed2 p = i.main_uv.xy;
                    
                // animate
                p.y += _sp*_Time.y;
                    
                // compute colors
                fixed3 col = pal( p.y - floor(p.y), _tag1.rgb, _tag2.rgb, _tag3.rgb, _tag4.rgb);
                // if( p.x>(1.0/7.0) ) col = pal( p.y, fixed3(0.5,0.5,0.5),fixed3(0.5,0.5,0.5),fixed3(1.0,1.0,1.0),fixed3(0.0,0.10,0.20) );
                // if( p.x>(2.0/7.0) ) col = pal( p.y, fixed3(0.5,0.5,0.5),fixed3(0.5,0.5,0.5),fixed3(1.0,1.0,1.0),fixed3(0.3,0.20,0.20) );
                // if( p.x>(3.0/7.0) ) col = pal( p.y, fixed3(0.5,0.5,0.5),fixed3(0.5,0.5,0.5),fixed3(1.0,1.0,0.5),fixed3(0.8,0.90,0.30) );
                // if( p.x>(4.0/7.0) ) col = pal( p.y, fixed3(0.5,0.5,0.5),fixed3(0.5,0.5,0.5),fixed3(1.0,0.7,0.4),fixed3(0.0,0.15,0.20) );
                // if( p.x>(5.0/7.0) ) col = pal( p.y, fixed3(0.5,0.5,0.5),fixed3(0.5,0.5,0.5),fixed3(2.0,1.0,0.0),fixed3(0.5,0.20,0.25) );
                // if( p.x>(6.0/7.0) ) col = pal( p.y, fixed3(0.8,0.5,0.4),fixed3(0.2,0.4,0.2),fixed3(2.0,1.0,1.0),fixed3(0.0,0.25,0.25) );
                    

                // band
                float f = p.x;
                // borders
                //col *= smoothstep( 0.49, 0.47, abs(f-0.5) );
                // shadowing
                col *= 0.5 + 0.5*sqrt(4.0*f*(1.0-f));

                return fixed4(col, 1.0);
            }
            ENDCG
 
        }
        
    }
}