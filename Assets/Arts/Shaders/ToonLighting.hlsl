#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

void ToonLighting_float(
    float3 NormalWS,
    float3 ViewDirWS,
    float3 LightDirWS,
    float3 LightColor,
    float3 BaseColor,
    UnityTexture2D RampTex,
    float2 RampUV,

    float ShadowThreshold,   // 0〜1
    float ShadowSmooth,      // 0〜0.5
    float ShadowStrength,    // 0〜1
    float SpecPower,
    float SpecStrength,      // 0〜1
    float RimPower,
    float RimStrength,       // 0〜1
    float LightIntensity,    // 0〜2

    out float3 OutColor)
{
    float3 N = normalize(NormalWS);
    float3 V = normalize(ViewDirWS);
    float3 L = normalize(-LightDirWS);

    float NdotL = saturate(dot(N, L));

    // -------- 影段階制御 --------
    float shadow = smoothstep(
        ShadowThreshold - ShadowSmooth,
        ShadowThreshold + ShadowSmooth,
        NdotL);

    float3 shadowColor = BaseColor * ShadowStrength;
    float3 diffuse = lerp(shadowColor, BaseColor, shadow);

    diffuse *= LightIntensity;

    // -------- Spec --------
    float3 H = normalize(L + V);
    float spec = pow(saturate(dot(N, H)), SpecPower);
    diffuse += spec * SpecStrength;

    // -------- Rim --------
    float rim = pow(1 - saturate(dot(N, V)), RimPower);
    diffuse += rim * RimStrength;

    OutColor = diffuse;
}

#endif