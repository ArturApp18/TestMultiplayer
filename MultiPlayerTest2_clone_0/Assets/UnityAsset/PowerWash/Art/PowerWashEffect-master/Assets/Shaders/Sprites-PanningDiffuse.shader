Shader "Sprites/PanningDiffuse"
{
  Properties
    {
        _MainTex ("Water Texture", 2D) = "white" {}     
        _Color ("Tint", Color) = (0.2, 0.6, 1, 1)      
        _WaveSpeed ("Wave Speed", float) = 2.0          
        _WaveAmplitude ("Wave Amplitude", float) = 0.05
        _WaveFrequency ("Wave Frequency", float) = 3.0  
        _WaveDelay ("Wave Delay", float) = 2.0         
        _TexSpeed ("Texture Scroll Speed", float) = 0.5  
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0; 
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST; 
            float _WaveSpeed;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveDelay;
            fixed4 _Color;
            float _TexSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                
                float wavePhase = (_Time.y * _WaveSpeed) - (v.uv.x * _WaveDelay);
                float waveOffset = sin(wavePhase * _WaveFrequency) * _WaveAmplitude;
                waveOffset *= v.uv.x; 
                
                float4 vertex = v.vertex;
                vertex.y += waveOffset;

                o.vertex = UnityObjectToClipPos(vertex);
                o.color = v.color * _Color;

                o.uv = v.uv;
                o.uv.x += _Time.y * _TexSpeed;
                o.uv = TRANSFORM_TEX(o.uv, _MainTex); 

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                return texColor * i.color;
            }
            ENDCG
        }
    }
    Fallback "Transparent/VertexLit"
}