Shader "SpriteDlight/Sprite (Simple)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Diffuse Texture", 2D) = "white" {}		//Alpha channel is plain old transparency
        _NormalDepth ("Normal Depth", 2D) = "bump" {} 		//Normal information in the colour channels, depth in the alpha channel.
        _AmplifyDepth ("Amplify Depth", Range (0,1.0)) = 0.0	//Affects the 'severity' of the depth map - affects shadows (and shading to a lesser extent).
        _AboveAmbientColour("Upper Ambient Colour", Color) = (0.3, 0.3, 0.3, 0.3)	//Ambient light coming from above.
        _BelowAmbientColour("Lower Ambient Colour", Color) = (0.1, 0.1, 0.1, 0.1)	//Ambient light coming from below.
        _LightWrap("Wraparound lighting", Range (0,1.0)) = 0.0	//Higher values of this will cause diffuse light to 'wrap around' and light the away-facing pixels a bit.
        _AttenuationExponent("Attenuation exponent", Range(0.0, 4.0)) = 2.0	//Higher number makes attenuation dropoff faster at first.
    }

    SubShader
    {
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
        Pass
        {    
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 
            #pragma target 3.0
			
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _NormalDepth;
            uniform float4 _AboveAmbientColour;
            uniform float4 _BelowAmbientColour;
            uniform float4 _LightColor0;
            uniform float _AmplifyDepth;
            uniform float _LightWrap;
            uniform float4x4 _LightMatrix0; // transformation
         	
           
            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 posLight : TEXCOORD2;
            };

            VertexOutput vert(VertexInput input) 
            {                
                VertexOutput output;

                output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
                output.posWorld = mul(_Object2World, input.vertex);

                output.uv = input.uv.xy;
                output.color = input.color;
				output.posLight = mul(_LightMatrix0, output.posWorld);
                return output;
            }

            float4 frag(VertexOutput input) : COLOR
            {
                float4 diffuseColour = tex2D(_MainTex, input.uv);
                float4 normalDepth = tex2D(_NormalDepth, input.uv);
                
                float4 ambientResult;
                
                float3 worldNormalDirection = (normalDepth.xyz - 0.5) * 2.0;
                
                worldNormalDirection = float3(mul(float4(worldNormalDirection, 1.0), _World2Object).xyz);
                
                float upness = worldNormalDirection.y * 0.5 + 0.5; //'upness' - 1.0 means the normal is facing straight up, 0.5 means horizontal, 0.0 straight down, etc.
                
                float4 ambientColour = ((_BelowAmbientColour * (1.0 - upness) + _AboveAmbientColour * upness) + float4(UNITY_LIGHTMODEL_AMBIENT.rgb, 0)) / 2;
                
                
                ambientResult = float4(ambientColour * diffuseColour);
                
                //We have to calculate illumination here too, because the first light that gets rendered
                //gets folded into the ambient pass apparently.
                //Get the real vector for the normal, 
		        float3 normalDirection = (normalDepth.xyz - 0.5) * 2.0;
                normalDirection.z *= -1.0;
                normalDirection = normalize(normalDirection);
				
               float depthColour = normalDepth.a;
                
                float2 roundedUVs = input.uv;

                float3 vertexToLightSource;
                float3 lightDirection;
                
                if (0.0 == _WorldSpaceLightPos0.w) // directional light?
	            {
	            	//This handles directional lights
	              	lightDirection = normalize(float3(_WorldSpaceLightPos0.xyz));
	            }
                
                //We calculate shadows here. Magic numbers incoming (FIXME).
                float shadowMult = 1.0;
                float3 moveVec = lightDirection.xyz * 0.006 * float3(1.0, 1.0, -1.0);
                float thisHeight = depthColour * _AmplifyDepth;
               
                float3 tapPos = float3(roundedUVs, thisHeight + 0.1);
                //This loop traces along the light ray and checks if that ray is inside the depth map at each point.
                //If it is, darken that pixel a bit.
                for (int i = 0; i < 8; i++)
				{
					tapPos += moveVec;
					float tapDepth = tex2D(_NormalDepth, tapPos.xy).a * _AmplifyDepth;
					if (tapDepth > tapPos.z)
					{
						shadowMult -= 0.125;
					}
				}
                shadowMult = clamp(shadowMult, 0.0, 1.0);
                
                
                

                // Compute diffuse part of lighting
                float normalDotLight = dot(normalDirection, lightDirection);
                
                //Slightly awkward maths for light wrap.
                float diffuseLevel = clamp(normalDotLight + _LightWrap, 0.0, _LightWrap + 1.0) / (_LightWrap + 1.0) * shadowMult;
                
                //The easy bits - assemble the final values based on light and map colours and combine.
                float3 diffuseReflection = diffuseColour.xyz * input.color.xyz * _LightColor0.xyz * diffuseLevel;
                
                float4 finalColour = float4(diffuseReflection, diffuseColour.a) + ambientResult;
                
                return finalColour;
                

            }

            ENDCG
        }

        Pass
        {    
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One // additive blending 

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 
			#pragma target 3.0
			#pragma glsl
			
			#pragma multi_compile_lightpass
			

            #include "UnityCG.cginc"

            // User-specified properties
            uniform sampler2D _MainTex;
            uniform sampler2D _NormalDepth;
            uniform float4 _LightColor0;
            uniform float _AmplifyDepth;
            uniform float _LightWrap;
            uniform float _AttenuationExponent;
            
            uniform float4x4 _LightMatrix0; // transformation

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 posLight : TEXCOORD2;
            };

            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;

                output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
                output.posWorld = mul(_Object2World, input.vertex);

                output.uv = input.uv.xy;
                output.color = input.color;
				output.posLight = mul(_LightMatrix0, output.posWorld);
                return output;
            }

            float4 frag(VertexOutput input) : COLOR
            {
            	//Do texture reads first, because in theory that's a bit quicker...
                float4 diffuseColour = tex2D(_MainTex, input.uv);
				float4 normalDepth = tex2D(_NormalDepth, input.uv);			
				
				//Get the real vector for the normal, 
		        float3 normalDirection = (normalDepth.xyz - 0.5) * 2.0;
                normalDirection.z *= -1.0;
                normalDirection = normalize(normalDirection);
				
				
                float depthColour = normalDepth.a;
                
                
                float2 roundedUVs = input.uv;

				float3 posWorld = input.posWorld.xyz;

                posWorld.z -= depthColour * _AmplifyDepth;	//The fragment's Z position is modified based on the depth map value.
                float3 vertexToLightSource;
                float3 lightDirection;
                float attenuation;
                
#if (defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE))
            	//This handles directional lights
              	lightDirection = normalize(float3(_WorldSpaceLightPos0.xyz));
       	   		attenuation = 1.0;
#else
            	//This code is for point/spot lights. Note that light cookies aren't yet handled for spot lights yet (FIXME)
            	float cookieAttenuation = 1.0;
         	    float normalisedDistance = 1.0;
        		vertexToLightSource = float3(_WorldSpaceLightPos0.xyz) - posWorld;
            	
            	float lightDistance = length(vertexToLightSource);
            	
#if SPOT
            	//This number, 'distance from centre', is the distance this fragment is from the centre line
            	//of the spot light. If it is greater than 1.0, this fragment is outside the light cone and shouldn't be
            	//illuminated.
            	float distanceFromCentre = length (float2(input.posLight.xy) / input.posLight.w) * 2.0;
            	normalisedDistance = input.posLight.z;
            	
            	//Fairly simplistic implementation of a default spotlight shape. Not total rubbish like the last one was,
            	//and doesn't require a texture lookup, but still probably not perfect.
            	cookieAttenuation = (1.0 - distanceFromCentre);
            	cookieAttenuation = clamp(cookieAttenuation, 0.0, 1.0);
            	
#elif POINT_NOATT
	            normalisedDistance = 0.0;
	            cookieAttenuation = 1.0;
#else
	    		cookieAttenuation = 1.0;
	    		normalisedDistance = length(input.posLight.xyz);
            	
#endif
	        	lightDirection = float3(mul(float4(vertexToLightSource, 1.0), _Object2World).xyz);
	        	lightDirection = normalize(lightDirection);
                
                attenuation = 1.0 - normalisedDistance;
                attenuation = clamp(attenuation, 0.0, 1.0);
                attenuation  = pow(attenuation, _AttenuationExponent);
                
                attenuation *= cookieAttenuation;
	            
#endif
                
                //We calculate shadows here. Magic numbers incoming (FIXME).
    
                            float shadowMult = 1.0;
                float3 moveVec = lightDirection.xyz * 0.006 * float3(1.0, 1.0, -1.0);
                float thisHeight = depthColour * _AmplifyDepth;
               
                float3 tapPos = float3(roundedUVs, thisHeight + 0.1);
                //This loop traces along the light ray and checks if that ray is inside the depth map at each point.
                //If it is, darken that pixel a bit.
                for (int i = 0; i < 8; i++)
				{
					tapPos += moveVec;
					float tapDepth = tex2D(_NormalDepth, tapPos.xy).a * _AmplifyDepth;
					if (tapDepth > tapPos.z)
					{
						shadowMult -= 0.125;
					}
				}
                shadowMult = clamp(shadowMult, 0.0, 1.0);
                
                

                // Compute diffuse part of lighting
                float normalDotLight = dot(normalDirection, lightDirection);
                
                //Slightly awkward maths for light wrap.
                float diffuseLevel = clamp(normalDotLight + _LightWrap, 0.0, _LightWrap + 1.0) / (_LightWrap + 1.0) * attenuation * shadowMult;
                
				//The easy bits - assemble the final values based on light and map colours and combine.
                float3 diffuseReflection = diffuseColour.xyz * input.color * _LightColor0.xyz * diffuseLevel;
                
                float4 finalColour = float4(diffuseReflection * diffuseColour.a, 0.0);
                //finalColour = input.posLight;//float4(normalisedDistance, normalisedDistance, normalisedDistance, 1.0);
                //finalColour = float4(length(input.posLight.xyz), 0.0, 0.0, 1.0);
                return finalColour;
                
             }

             ENDCG
        }
    }
    // The definition of a fallback shader should be commented out 
    // during development:
    Fallback "Transparent/Diffuse"
}