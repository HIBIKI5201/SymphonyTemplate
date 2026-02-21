#ifndef CUSTOM_OUTLINE_INCLUDED
#define CUSTOM_OUTLINE_INCLUDED

void AdvancedOutline_float(
    float3 PositionOS,
    float3 NormalOS,
    float Thickness,
    float FadeDistance,
    float Mask,
    float3 LightDirWS,
    out float3 OutPositionOS,
    out float OutlineAlpha
)
{
    // ===== 空間変換 =====
    float3 positionWS = mul(unity_ObjectToWorld, float4(PositionOS, 1.0)).xyz;
    float3 normalWS = normalize(mul((float3x3)unity_ObjectToWorld, NormalOS));

    // ===== ライト方向による強弱 =====
    float ndl = saturate(dot(normalWS, normalize(LightDirWS)));
    float lightFade = 1.0 - ndl; // 光が当たっている側は細く、影側は太く

    // ===== アウトラインオフセット =====
    // 法線方向で押し出す方式に変更（カメラ距離に依存しない）
    float3 offsetOS = NormalOS * Thickness * lightFade;
    OutPositionOS = PositionOS + offsetOS;

    // ===== 距離フェード（任意） =====
    float camDistance = distance(positionWS, _WorldSpaceCameraPos);
    float distanceFade = saturate(1.0 - camDistance / FadeDistance);

    // ===== 最終アルファ =====
    OutlineAlpha = Mask * distanceFade; // ライトフェードは押し出し量に反映済み
}

#endif