Shader "Highlight"{
    Properties{
        _NColor ("Normal Color", Color) = (0.5, 1, 0.5, 1)
        _HColor ("Highlight Color", Color) = (1, 1, 1, 1)
        [Toggle] _Highlighted ("Highlighted", Float) = 0
    }
    SubShader{
        Pass{

            //Tags {"RenderType" = "Transparent"}
            // 关闭深度写入
            ZWrite Off
             // 开启混合模式，并设置混合因子为SrcAlpha和OneMinusSrcAlpha
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            float4 _NColor;
            float4 _HColor;
            float _Highlighted;


            float4 vert(float4 v : POSITION) : SV_POSITION{
                return UnityObjectToClipPos(v);
            }

            float4 frag(float4 v : SV_POSITION, uint triangleID : SV_PrimitiveID) : SV_Target{
                if(triangleID < 12){
                    if(_Highlighted == 1){
                        return _HColor;
                    }
                    else{
                        return _NColor;
                    }
                }
                return float4(1,1,1,0);
            }

            ENDCG

            Cull Off
        }
    }
}