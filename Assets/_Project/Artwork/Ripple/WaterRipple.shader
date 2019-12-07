Shader "Custom/WaterRipple"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Scale("Scale", float) = 1.0
		_Speed("Speed", float) = 1.0
		_Frequency("Frequency", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
		Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows 
		#pragma vertex vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		float _Scale, _Speed, _Frequency;
		
		float4 _Ripples[128]; // x = Offset X, y = Offset Z, z = Amplitude, w = Distance

        struct Input
        {
            float2 uv_MainTex;
        };

		void vert(inout appdata_full v) 
		{
			half3 worldpos = mul(unity_ObjectToWorld, v.vertex);

			for( int i = 0; i < 128; i++ )
			{
				float4 ripple = _Ripples[i];
				if( ripple.z <= 0.0
					|| sqrt(pow(worldpos.x - ripple.x, 2) + pow(worldpos.z - ripple.y, 2) >= ripple.w) )
					continue;

				half3 worldoffset = worldpos;
				worldoffset.x -= ripple.x;
				worldoffset.z -= ripple.y;

				half offsetvert = ( ( worldoffset.x * worldoffset.x ) + ( worldoffset.z * worldoffset.z ) );

				half value = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert) * ripple.z;
				v.vertex.y += value;

				float3 worldNormal = mul(unity_ObjectToWorld, v.normal);
				worldNormal.x -= ripple.x;
				worldNormal.z -= ripple.z;
				v.normal = mul(unity_WorldToObject, normalize(worldNormal));
			}
		}

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
