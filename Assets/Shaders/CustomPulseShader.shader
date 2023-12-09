Shader "Custom/CustomPulseShader"
{   Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PulsesPerMinute("Pulses Per Minute", Range(25, 300)) = 60.0
        _Intensity("Intensity", Range(0,1)) = 1.0
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _PulseColor("Pulse Color", Color) = (1, 0, 0, 1)
        _PulsingEnabled("Pulsing Enabled", Int) = 1 // 1 for enabled, 0 for disabled
        _RandomPulsing("Random Pulsing", Int) = 0 // 1 for random, 0 for top-to-bottom
        _Frequency("Frequency", Range(0, 1)) = 0.2 // Adjust frequency of random spots
        _SpotSize("Spot Size", Range(0, 1)) = 0.1 // Adjust size of random spots
}
SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                ZTest Always Cull Off ZWrite Off
                Blend SrcAlpha OneMinusSrcAlpha
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _PulsesPerMinute;
                float _Intensity;
                fixed4 _BaseColor;
                fixed4 _PulseColor;
                int _PulsingEnabled;
                int _RandomPulsing;
                float _Frequency;
                float _SpotSize;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    if (_PulsingEnabled == 0) // Check if pulsing is disabled
                    {
                        // If pulsing is disabled, return the base color directly
                        return _BaseColor;
                    }

                    if (_RandomPulsing == 1) // Check if random pulsing is enabled
                    {
                        // Generate random spots for pulsing effect
                        float randomPulse = 0.0;
                        float randomValue = frac(sin(_Time.y * 10.0 + i.uv.x * 30.0) * 43758.5453);
                        if (randomValue < _Frequency)
                        {
                            // Adjust the range (_Frequency) to control the density of random spots
                            randomPulse = smoothstep(0, _Intensity, frac((_Time.y + i.uv.y * 0.5 + randomValue) * 10.0)) * _SpotSize;
                        }

                        fixed4 col = _BaseColor; // Set base color

                        fixed4 pulseColor = _PulseColor; // Set pulsing color

                        // Combine base color and pulsing color with pulse intensity
                        col.rgb *= (1 - randomPulse) + randomPulse * pulseColor.rgb;

                        return col;
                    }

                    // Default top-to-bottom pulsing
                    float secondsPerPulse = 60.0 / _PulsesPerMinute; // Calculate time between pulses based on BPM

                    float pulse = smoothstep(0, _Intensity, frac((_Time.y + i.uv.y * 0.5) / secondsPerPulse)); // Use smoothstep to create pulses

                    fixed4 defaultCol = _BaseColor; // Set base color

                    fixed4 defaultPulseColor = _PulseColor; // Set pulsing color

                    // Combine base color and pulsing color with pulse intensity
                    defaultCol.rgb *= (1 - pulse) + pulse * defaultPulseColor.rgb;

                    return defaultCol;
                }
                ENDCG
            }
        }
}
