// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Fancyoutline"
{	 
	Properties{
		_Highlighted("Hightlight",float) = 0
	}
	SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float _Highlighted;

            struct a2v{
                float4 position : POSITION;
                float2 uv: TEXCOORD0;
            };
			struct v2f {
				float4 position : SV_POSITION;
                float2 uv: TEXCOORD0;
			};

			v2f vert(a2v v){
				v2f o;
				o.position = UnityObjectToClipPos(v.position);
                o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				if(_Highlighted == 0){
					return fixed4(0,0,0,0);
				}
				float2 p = (i.uv*2 - fixed2(1,1)) / fixed2(0.5,0.5);
                //float2 p = i.position.xy;
				float tau = 3.1415926535*2.0; 
				float a = atan2(p.x, p.y);
				float r = length(p)*0.75;
				float2 uv = float2(a / tau, r);

				//get the color
				float xCol = (uv.x + (_Time.y / 3.0)) * 3.0;
				xCol = fmod(xCol, 3.0);
				float3 horColour = float3(0.25, 0.25, 0.25);

				if (xCol < 1.0) {

					horColour.r += 1.0 - xCol;
					horColour.g += xCol;
				}
				else if (xCol < 2.0) {

					xCol -= 1.0;
					horColour.g += 1.0 - xCol;
					horColour.b += xCol;
				}
				else {

					xCol -= 2.0;
					horColour.b += 1.0 - xCol;
					horColour.r += xCol;
				}

				// draw color beam
				uv = (2.0 * uv) - 1.0;
				float beamWidth = (0.7 + 0.5*cos(uv.x*10.0*tau*0.15*clamp(floor(5.0 + 10.0*cos(_Time.y)), 0.0, 0.0))) * clamp(abs(1.0 / (30.0 *(1 - uv.y))), 2, 10);
				float3 horBeam = float3(beamWidth, beamWidth, beamWidth);
				return float4(((horBeam)* horColour), 1.0);
				////////////////////				
			}
			ENDCG
		}
	}
}