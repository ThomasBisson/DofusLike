Shader "Custom/SimplePBR"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0
	}
	SubShader
	{
		Tags { "LightMode" = "ForwardBase" "RenderType"="Opaque" "PerformanceChecks" = "False"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma multi_compile _NORMALMAP
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
			#pragma vertex vertBase
			#pragma fragment fragBase
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "UnityLightingCommon.cginc"
			struct VertexInput1
			{
				float4 vertex   : POSITION;
				half3 normal    : NORMAL;
				float2 uv0      : TEXCOORD0;
				float2 uv1      : TEXCOORD1;
				float2 uv2      : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutputForwardBase1
			{
				UNITY_POSITION(pos);
				float4 tex                          : TEXCOORD0;
				half3 eyeVec                        : TEXCOORD1;
				half4 tangentToWorldAndPackedData[3]    : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
				half4 ambientOrLightmapUV           : TEXCOORD5;    // SH or Lightmap UV
				UNITY_SHADOW_COORDS(6)
				UNITY_FOG_COORDS(7)
				float3 posWorld                 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 TexCoords(VertexInput1 v)
			{
				float4 texcoord;
				texcoord.xy = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
				return texcoord;
			}


			half3 ShadeSHPerVertex(half3 normal, half3 ambient)
			{
				ambient += max(half3(0, 0, 0), ShadeSH9(half4(normal, 1.0)));
				return ambient;
			}

			inline half4 VertexGIForward(VertexInput1 v, float3 posWorld, half3 normalWorld)
			{
				half4 ambientOrLightmapUV = 0;
				ambientOrLightmapUV.rgb = ShadeSHPerVertex(normalWorld, ambientOrLightmapUV.rgb);
				return ambientOrLightmapUV;
			}
			half3x3 CreateTangentToWorldPerVertex(half3 normal, half3 tangent, half tangentSign)
			{
				// For odd-negative scale transforms we need to flip the sign
				half sign = tangentSign * unity_WorldTransformParams.w;
				half3 binormal = cross(normal, tangent) * sign;
				return half3x3(tangent, binormal, normal);
			}
			VertexOutputForwardBase1 vertForwardBase(VertexInput1 v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				VertexOutputForwardBase1 o;
				UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase1, o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.posWorld = posWorld.xyz;


				o.pos = UnityObjectToClipPos(v.vertex);

				o.tex = TexCoords(v);
				o.eyeVec = posWorld.xyz - _WorldSpaceCameraPos;
				float3 normalWorld = UnityObjectToWorldNormal(v.normal);

				o.tangentToWorldAndPackedData[0].xyz = 0;
				o.tangentToWorldAndPackedData[1].xyz = 0;
				o.tangentToWorldAndPackedData[2].xyz = normalWorld;

				UNITY_TRANSFER_SHADOW(o, v.uv1);

				o.ambientOrLightmapUV = VertexGIForward(v, posWorld, normalWorld);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}


			float4 _Color;
			VertexOutputForwardBase1 vertBase(VertexInput1 v)
			{ 
				return vertForwardBase(v); 
			}

#define IN_VIEWDIR4PARALLAX(i) half3(0,0,0)

#define IN_WORLDPOS(i) i.posWorld

			struct FragmentCommonData
			{
				half3 diffColor, specColor;
				// Note: smoothness & oneMinusReflectivity for optimization purposes, mostly for DX9 SM2.0 level.
				// Most of the math is being done on these (1-x) values, and that saves a few precious ALU slots.
				half oneMinusReflectivity, smoothness;
				half3 normalWorld, eyeVec;
				half alpha;
				float3 posWorld;
			};
			half Alpha(float2 uv)
			{
				return tex2D(_MainTex, uv).a * _Color.a;
			}

			float _Glossiness;
			half4 SpecularGloss(float2 uv)
			{
				half4 sg;
				sg.rgb = _SpecColor.rgb;
				sg.a = _Glossiness;
				return sg;
			}
			half3 Albedo(float4 texcoords)
			{
				half3 albedo = _Color.rgb * tex2D(_MainTex, texcoords.xy).rgb;
				return albedo;
			}
			half SpecularStrength(half3 specular)
			{
				return specular.r; // Red channel - because most metals are either monocrhome or with redish/yellowish tint

			}
			// Diffuse/Spec Energy conservation
			inline half3 EnergyConservationBetweenDiffuseAndSpecular(half3 albedo, half3 specColor, out half oneMinusReflectivity)
			{
				oneMinusReflectivity = 1 - SpecularStrength(specColor);
				return albedo * oneMinusReflectivity;
			}

			FragmentCommonData SpecularSetup(float4 i_tex)
			{
				half4 specGloss = SpecularGloss(i_tex.xy);
				half3 specColor = specGloss.rgb;
				half smoothness = specGloss.a;

				half oneMinusReflectivity;
				half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular(Albedo(i_tex), specColor, /*out*/ oneMinusReflectivity);

				FragmentCommonData o = (FragmentCommonData)0;
				o.diffColor = diffColor;
				o.specColor = specColor;
				o.oneMinusReflectivity = oneMinusReflectivity;
				o.smoothness = smoothness;
				return o;
			}
			half3 NormalizePerPixelNormal(half3 n)
			{
				return normalize(n);
			}
			half3 UnpackScaleNormalRGorAG(half4 packednormal, half bumpScale)
			{
#if defined(UNITY_NO_DXT5nm)
				half3 normal = packednormal.xyz * 2 - 1;
				normal.xy *= bumpScale;
				return normal;
#else
				// This do the trick
				packednormal.x *= packednormal.w;
				half3 normal;
				normal.xy = (packednormal.xy * 2 - 1);
				normal.xy *= bumpScale;
				normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
				return normal;
#endif
			}

			half3 UnpackScaleNormal(half4 packednormal, half bumpScale)
			{
				return UnpackScaleNormalRGorAG(packednormal, bumpScale);
			}


			half3 PerPixelWorldNormal(float4 i_tex, half4 tangentToWorld[3])
			{
				half3 normalWorld = normalize(tangentToWorld[2].xyz);
				return normalWorld;
			}

			inline half3 PreMultiplyAlpha(half3 diffColor, half alpha, half oneMinusReflectivity, out half outModifiedAlpha)
			{

				outModifiedAlpha = alpha;
				return diffColor;
			}
			half        _Metallic;
			half2 MetallicGloss(float2 uv)
			{
				half2 mg;
				mg.r = _Metallic;
				mg.g = _Glossiness;
				return mg;
			}

			inline half OneMinusReflectivityFromMetallic(half metallic)
			{
				// We'll need oneMinusReflectivity, so
				//   1-reflectivity = 1-lerp(dielectricSpec, 1, metallic) = lerp(1-dielectricSpec, 0, metallic)
				// store (1-dielectricSpec) in unity_ColorSpaceDielectricSpec.a, then
				//   1-reflectivity = lerp(alpha, 0, metallic) = alpha + metallic*(0 - alpha) =
				//                  = alpha - metallic * alpha
				half oneMinusDielectricSpec = unity_ColorSpaceDielectricSpec.a;
				return oneMinusDielectricSpec - metallic * oneMinusDielectricSpec;
			}

			inline half3 DiffuseAndSpecularFromMetallic(half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
			{
				specColor = lerp(unity_ColorSpaceDielectricSpec.rgb, albedo, metallic);
				oneMinusReflectivity = OneMinusReflectivityFromMetallic(metallic);
				return albedo * oneMinusReflectivity;
			}
			inline FragmentCommonData MetallicSetup(float4 i_tex)
			{
				half2 metallicGloss = MetallicGloss(i_tex.xy);
				half metallic = metallicGloss.x;
				half smoothness = metallicGloss.y; // this is 1 minus the square root of real roughness m.

				half oneMinusReflectivity;
				half3 specColor;
				half3 diffColor = DiffuseAndSpecularFromMetallic(Albedo(i_tex), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

				FragmentCommonData o = (FragmentCommonData)0;
				o.diffColor = diffColor;
				o.specColor = specColor;
				o.oneMinusReflectivity = oneMinusReflectivity;
				o.smoothness = smoothness;
				return o;
			}
			UnityLight MainLight()
			{
				UnityLight l;

				l.color = _LightColor0.rgb;
				l.dir = _WorldSpaceLightPos0.xyz;
				return l;
			}

			// ----------------------------------------------------------------------------
			// GlossyEnvironment - Function to integrate the specular lighting with default sky or reflection probes
			// ----------------------------------------------------------------------------
			struct Unity_GlossyEnvironmentData
			{
				// - Deferred case have one cubemap
				// - Forward case can have two blended cubemap (unusual should be deprecated).

				// Surface properties use for cubemap integration
				half    roughness; // CAUTION: This is perceptualRoughness but because of compatibility this name can't be change :(
				half3   reflUVW;
			};

			half SmoothnessToPerceptualRoughness(half smoothness)
			{
				return (1 - smoothness);
			}

			Unity_GlossyEnvironmentData UnityGlossyEnvironmentSetup(half Smoothness, half3 worldViewDir, half3 Normal, half3 fresnel0)
			{
				Unity_GlossyEnvironmentData g;

				g.roughness /* perceptualRoughness */ = SmoothnessToPerceptualRoughness(Smoothness);
				g.reflUVW = reflect(-worldViewDir, Normal);

				return g;
			}

			inline void ResetUnityLight(out UnityLight outLight)
			{
				outLight.color = half3(0, 0, 0);
				outLight.dir = half3(0, 1, 0); // Irrelevant direction, just not null
				outLight.ndotl = 0; // Not used
			}

			inline void ResetUnityGI(out UnityGI outGI)
			{
				ResetUnityLight(outGI.light);
				outGI.indirect.diffuse = 0;
				outGI.indirect.specular = 0;
			}

			half3 ShadeSHPerPixel(half3 normal, half3 ambient, float3 worldPos)
			{
				half3 ambient_contrib = 0.0;
				return ambient;
			}

			inline UnityGI UnityGI_Base(UnityGIInput data, half occlusion, half3 normalWorld)
			{
				UnityGI o_gi;
				ResetUnityGI(o_gi);
				// Base pass with Lightmap support is responsible for handling ShadowMask / blending here for performance reason
				o_gi.light = data.light;
				o_gi.light.color *= data.atten;
				o_gi.indirect.diffuse = ShadeSHPerPixel(normalWorld, data.ambient, data.worldPos);
				o_gi.indirect.diffuse *= occlusion;
				return o_gi;
			}
#define UNITY_SPECCUBE_LOD_STEPS (6)
			// ----------------------------------------------------------------------------
			half perceptualRoughnessToMipmapLevel(half perceptualRoughness)
			{
				return perceptualRoughness * UNITY_SPECCUBE_LOD_STEPS;
			}
			// ----------------------------------------------------------------------------
			half3 Unity_GlossyEnvironment(UNITY_ARGS_TEXCUBE(tex), half4 hdr, Unity_GlossyEnvironmentData glossIn)
			{
				half perceptualRoughness = glossIn.roughness /* perceptualRoughness */;

				// TODO: CAUTION: remap from Morten may work only with offline convolution, see impact with runtime convolution!
				// For now disabled
				perceptualRoughness = perceptualRoughness * (1.7 - 0.7*perceptualRoughness);
				half mip = perceptualRoughnessToMipmapLevel(perceptualRoughness);
				half3 R = glossIn.reflUVW;
				half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(tex, R, mip);

				return DecodeHDR(rgbm, hdr);
			}

			inline half3 UnityGI_IndirectSpecular(UnityGIInput data, half occlusion, Unity_GlossyEnvironmentData glossIn)
			{
				half3 specular;


#ifdef _GLOSSYREFLECTIONS_OFF
				specular = unity_IndirectSpecColor.rgb;
#else
				half3 env0 = Unity_GlossyEnvironment(UNITY_PASS_TEXCUBE(unity_SpecCube0), data.probeHDR[0], glossIn);

				specular = env0;
#endif

				return specular * occlusion;
			}
			inline UnityGI UnityGlobalIllumination(UnityGIInput data, half occlusion, half3 normalWorld, Unity_GlossyEnvironmentData glossIn)
			{
				UnityGI o_gi = UnityGI_Base(data, occlusion, normalWorld);
				o_gi.indirect.specular = UnityGI_IndirectSpecular(data, occlusion, glossIn);
				return o_gi;
			}
			inline UnityGI FragmentGI(FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light)
			{
				UnityGIInput d;
				d.light = light;
				d.worldPos = s.posWorld;
				d.worldViewDir = -s.eyeVec;
				d.atten = atten;

				d.ambient = i_ambientOrLightmapUV.rgb;
				d.lightmapUV = 0;

				d.probeHDR[0] = unity_SpecCube0_HDR;
				d.probeHDR[1] = unity_SpecCube1_HDR;
				Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.smoothness, -s.eyeVec, s.normalWorld, s.specColor);
				// Replace the reflUVW if it has been compute in Vertex shader. Note: the compiler will optimize the calcul in UnityGlossyEnvironmentSetup itself

				return UnityGlobalIllumination(d, occlusion, s.normalWorld, g);

			}

			sampler2D unity_NHxRoughness;
			half3 BRDF3_Direct(half3 diffColor, half3 specColor, half rlPow4, half smoothness)
			{
				half LUT_RANGE = 16.0; // must match range in NHxRoughness() function in GeneratedTextures.cpp
									   // Lookup texture to save instructions
				half specular = tex2D(unity_NHxRoughness, half2(rlPow4, SmoothnessToPerceptualRoughness(smoothness))).UNITY_ATTEN_CHANNEL * LUT_RANGE;
#if defined(_SPECULARHIGHLIGHTS_OFF)
				specular = 0.0;
#endif

				return diffColor + specular * specColor;
			}

			half3 BRDF3_Indirect(half3 diffColor, half3 specColor, UnityIndirect indirect, half grazingTerm, half fresnelTerm)
			{
				half3 c = indirect.diffuse * diffColor;
				c += indirect.specular * lerp(specColor, grazingTerm, fresnelTerm);
				return c;
			}

			// Old school, not microfacet based Modified Normalized Blinn-Phong BRDF
			// Implementation uses Lookup texture for performance
			//
			// * Normalized BlinnPhong in RDF form
			// * Implicit Visibility term
			// * No Fresnel term
			//
			// TODO: specular is too weak in Linear rendering mode

			inline half2 Pow4(half2 x)
			{
				return x * x*x*x;
			}

			half4 BRDF3_Unity_PBS(half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,
				half3 normal, half3 viewDir,
				UnityLight light, UnityIndirect gi)
			{
				half3 reflDir = reflect(viewDir, normal);

				half nl = saturate(dot(normal, light.dir));
				half nv = saturate(dot(normal, viewDir));

				// Vectorize Pow4 to save instructions
				half2 rlPow4AndFresnelTerm = Pow4(half2(dot(reflDir, light.dir), 1 - nv));  // use R.L instead of N.H to save couple of instructions
				half rlPow4 = rlPow4AndFresnelTerm.x; // power exponent must match kHorizontalWarpExp in NHxRoughness() function in GeneratedTextures.cpp
				half fresnelTerm = rlPow4AndFresnelTerm.y;

				half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));

				half3 color = BRDF3_Direct(diffColor, specColor, rlPow4, smoothness);
				color *= light.color * nl;
				color += BRDF3_Indirect(diffColor, specColor, gi, grazingTerm, fresnelTerm);

				return half4(color, 1);
			}
			half4 OutputForward(half4 output, half alphaFromSurface)
			{
#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
				output.a = alphaFromSurface;
#else
				UNITY_OPAQUE_ALPHA(output.a);
#endif
				return output;
			}

			inline FragmentCommonData FragmentSetup(inout float4 i_tex, half3 i_eyeVec, half3 i_viewDirForParallax, half4 tangentToWorld[3], float3 i_posWorld)
			{
				half alpha = Alpha(i_tex.xy);
#if defined(_ALPHATEST_ON)
				clip(alpha - _Cutoff);
#endif

				FragmentCommonData o = MetallicSetup(i_tex);
				o.normalWorld = PerPixelWorldNormal(i_tex, tangentToWorld);
				o.eyeVec = NormalizePerPixelNormal(i_eyeVec);
				o.posWorld = i_posWorld;

				// NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
				o.diffColor = PreMultiplyAlpha(o.diffColor, alpha, o.oneMinusReflectivity, /*out*/ o.alpha);
				return o;
			}

			inline half3 Unity_SafeNormalize(half3 inVec)
			{
				half dp3 = max(0.001f, dot(inVec, inVec));
				return inVec * rsqrt(dp3);
			}
			half PerceptualRoughnessToRoughness(half perceptualRoughness)
			{
				return perceptualRoughness * perceptualRoughness;
			}

			inline half PerceptualRoughnessToSpecPower(half perceptualRoughness)
			{
				half m = PerceptualRoughnessToRoughness(perceptualRoughness);   // m is the true academic roughness.
				half sq = max(1e-4f, m*m);
				half n = (2.0 / sq) - 2.0;                          // https://dl.dropboxusercontent.com/u/55891920/papers/mm_brdf.pdf
				n = max(n, 1e-4f);                                  // prevent possible cases of pow(0,0), which could happen when roughness is 1.0 and NdotH is zero
				return n;
			}

			// approximage Schlick with ^4 instead of ^5
			inline half3 FresnelLerpFast(half3 F0, half3 F90, half cosA)
			{
				half t = Pow4(1 - cosA);
				return lerp(F0, F90, t);
			}
			// Based on Minimalist CookTorrance BRDF
			// Implementation is slightly different from original derivation: http://www.thetenthplanet.de/archives/255
			//
			// * NDF (depending on UNITY_BRDF_GGX):
			//  a) BlinnPhong
			//  b) [Modified] GGX
			// * Modified Kelemen and Szirmay-​Kalos for Visibility term
			// * Fresnel approximated with 1/LdotH
			half4 BRDF2_Unity_PBS(half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,
				half3 normal, half3 viewDir,
				UnityLight light, UnityIndirect gi)
			{
				half3 halfDir = Unity_SafeNormalize(light.dir + viewDir);

				half nl = saturate(dot(normal, light.dir));
				half nh = saturate(dot(normal, halfDir));
				half nv = saturate(dot(normal, viewDir));
				half lh = saturate(dot(light.dir, halfDir));

				// Specular term
				half perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
				half roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

#if UNITY_BRDF_GGX

				// GGX Distribution multiplied by combined approximation of Visibility and Fresnel
				// See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
				// https://community.arm.com/events/1155
				half a = roughness;
				half a2 = a * a;

				half d = nh * nh * (a2 - 1.h) + 1.00001h;
#ifdef UNITY_COLORSPACE_GAMMA
				// Tighter approximation for Gamma only rendering mode!
				// DVF = sqrt(DVF);
				// DVF = (a * sqrt(.25)) / (max(sqrt(0.1), lh)*sqrt(roughness + .5) * d);
				half specularTerm = a / (max(0.32h, lh) * (1.5h + roughness) * d);
#else
				half specularTerm = a2 / (max(0.1h, lh*lh) * (roughness + 0.5h) * (d * d) * 4);
#endif

				// on mobiles (where half actually means something) denominator have risk of overflow
				// clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
				// sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
#if defined (SHADER_API_MOBILE)
				specularTerm = specularTerm - 1e-4h;
#endif

#else

				// Legacy
				half specularPower = PerceptualRoughnessToSpecPower(perceptualRoughness);
				// Modified with approximate Visibility function that takes roughness into account
				// Original ((n+1)*N.H^n) / (8*Pi * L.H^3) didn't take into account roughness
				// and produced extremely bright specular at grazing angles

				half invV = lh * lh * smoothness + perceptualRoughness * perceptualRoughness; // approx ModifiedKelemenVisibilityTerm(lh, perceptualRoughness);
				half invF = lh;

				half specularTerm = ((specularPower + 1) * pow(nh, specularPower)) / (8 * invV * invF + 1e-4h);

#ifdef UNITY_COLORSPACE_GAMMA
				specularTerm = sqrt(max(1e-4h, specularTerm));
#endif

#endif

#if defined (SHADER_API_MOBILE)
				specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
#if defined(_SPECULARHIGHLIGHTS_OFF)
				specularTerm = 0.0;
#endif

				// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)

				// 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
				// 1-x^3*(0.6-0.08*x)   approximation for 1/(x^4+1)
#ifdef UNITY_COLORSPACE_GAMMA
				half surfaceReduction = 0.28;
#else
				half surfaceReduction = (0.6 - 0.08*perceptualRoughness);
#endif

				surfaceReduction = 1.0 - roughness * perceptualRoughness*surfaceReduction;

				half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));
				half3 color = (diffColor + specularTerm * specColor) * light.color * nl
					+ gi.diffuse * diffColor
					+ surfaceReduction * gi.specular * FresnelLerpFast(specColor, grazingTerm, nv);

				return half4(color, 1);
			}

			half4 fragForwardBaseInternal(VertexOutputForwardBase1 i)
			{
				UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

				FragmentCommonData s = FragmentSetup(i.tex, i.eyeVec, IN_VIEWDIR4PARALLAX(i), i.tangentToWorldAndPackedData, IN_WORLDPOS(i));

				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				UnityLight mainLight = MainLight();
				UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);

				half occlusion = 1;
				UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

				half4 c = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);

				UNITY_APPLY_FOG(i.fogCoord, c.rgb);
				return OutputForward(c, s.alpha);
			}
			half4 fragBase(VertexOutputForwardBase1 i) : SV_Target
			{ 
				return fragForwardBaseInternal(i) * 1.2;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
