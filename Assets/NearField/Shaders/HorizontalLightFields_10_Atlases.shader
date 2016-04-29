//////////////////////////////////////////////////
 // Copyright 2016
 // MxR Studio
 // School of Cinematic Arts, USC
 // 
 // Author: Ashok Mathew Kuruvilla
 // 		M.S. in CS (Multimedia & Creative Tech.)
 // 		Viterbi School of Engineering, USC
 // 
 // Email: akuruvil@usc.edu
 //////////////////////////////////////////////////
 
 Shader "NearField/HorizontalLightFields_10_Atlases" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Atlas0 ("Atlas0", 2D) = "white" {}
	_Atlas1 ("Atlas1", 2D) = "white" {}
	_Atlas2 ("Atlas2", 2D) = "white" {}
	_Atlas3 ("Atlas3", 2D) = "white" {}
	_Atlas4 ("Atlas4", 2D) = "white" {}
	_Atlas5 ("Atlas5", 2D) = "white" {}
	_Atlas6 ("Atlas6", 2D) = "white" {}
	_Atlas7 ("Atlas7", 2D) = "white" {}
	_Atlas8 ("Atlas8", 2D) = "white" {}
	_Atlas9 ("Atlas9", 2D) = "white" {}
	_ViewAngle("View Angle", Float) = 0.0 //Angle added to theta in derivation
	_ImagesPerTile("Images Per Tile", Float) = 36.0 //Angle added to theta in derivation
	_SurfaceSize("Surface Size", Float) = 1.0	//width or height of the playing quad
	_CaptureDistanceSizeRatio("Captured Distance Size Ratio", Float) = 1.0 //d / width in derivation
	_DegreePrecision("Degree Precision", Float) = 1.0
	_InterpolationPower("Interpolation power", Float) = 1.0
	_OffsetScale("Offset Scale", Float) = 1.2
}

SubShader {

Cull Off

Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
LOD 200
	
CGPROGRAM
#pragma surface surf Lambert alpha:fade vertex:vert

sampler2D _Atlas0;
sampler2D _Atlas1;
sampler2D _Atlas2;
sampler2D _Atlas3;
sampler2D _Atlas4;
sampler2D _Atlas5;
sampler2D _Atlas6;
sampler2D _Atlas7;
sampler2D _Atlas8;
sampler2D _Atlas9;
fixed4 _Color;
float _ViewAngle;
float _ImagesPerTile;
float _SurfaceSize;
float _CaptureDistanceSizeRatio;
float _DegreePrecision;
float _InterpolationPower;
float _OffsetScale;

struct Input {
	float2 uv_Atlas0;
};

float3 uvCoords(float theta)
{
	const float PI = 3.14159;
	
	theta = (theta + 2*PI);
	theta -= floor(theta/(2.0*PI))*2.0*PI;
	//Dividing theta into 1 degree units
	theta *= 180.0/PI/1.0;
	//Offset theta to 0.5 degree around each 1 degree angle
	theta += 0.5;
	//Adjust for overflow due to above addition
	theta -= floor(theta/360.0)*360.0;
	theta = floor(theta);
	
	float imagesPerTile = round (_ImagesPerTile);
	float atlasIdx = floor(theta/imagesPerTile);
	theta -= atlasIdx*imagesPerTile;
	
	float imagesPerRow = ceil( round(sqrt (imagesPerTile) * 100) / 100);
	
	float row = floor(theta/imagesPerRow);
	float column = theta - row*imagesPerRow;
	
	return float3(column, row, atlasIdx);
}

fixed4 getAtlasColor(float atlasIdx, float2 uvCoords) 
{
	fixed4 atlasColor = fixed4 (0,0,0,0);
	if(atlasIdx < 1.0) atlasColor = tex2D(_Atlas0, uvCoords);
	else if(atlasIdx < 2.0) atlasColor = tex2D(_Atlas1, uvCoords);
	else if(atlasIdx < 3.0) atlasColor = tex2D(_Atlas2, uvCoords);
	else if(atlasIdx < 4.0) atlasColor = tex2D(_Atlas3, uvCoords);
	else if(atlasIdx < 5.0) atlasColor = tex2D(_Atlas4, uvCoords);
	else if(atlasIdx < 6.0) atlasColor = tex2D(_Atlas5, uvCoords);
	else if(atlasIdx < 7.0) atlasColor = tex2D(_Atlas6, uvCoords);
	else if(atlasIdx < 8.0) atlasColor = tex2D(_Atlas7, uvCoords);
	else if(atlasIdx < 9.0) atlasColor = tex2D(_Atlas8, uvCoords);
	else atlasColor = tex2D(_Atlas9, uvCoords);
	
	return atlasColor;
}

fixed4 getBlinearInterpolatedColor(Input IN, float2 coords, float xSign, float gamma, float alpha, float alphaLessTheta)
{
	const float PI = 3.14159;
	
	float width = _SurfaceSize;
	
	float alphaDegree = alpha * 180.0 / PI;
	
	float alpha1 = floor(alphaDegree/_DegreePrecision)*_DegreePrecision * PI / 180.0;
	float alpha2 = ceil(alphaDegree/_DegreePrecision)*_DegreePrecision * PI / 180.0;

	float weight12 = (alpha2*180.0/PI - alphaDegree)/_DegreePrecision; 
	weight12 = pow(weight12, _InterpolationPower);
	float weight21 = 1.0 - weight12;
	
	float3 minUV = uvCoords(alpha1);
	float3 maxUV = uvCoords(alpha2);
	
	float imagesPerTile = round (_ImagesPerTile);
	float imagesPerRow = ceil( round(sqrt (imagesPerTile) * 2) / 2);
	float2 alphaMinCoords = (minUV.xy + float2(coords.x, coords.y)) / imagesPerRow;
	float2 alphaMaxCoords = (maxUV.xy + float2(coords.x, coords.y)) / imagesPerRow;
	
	//Linear interpolation
	//float minRatio = (alpha2 - alphaDegree)/_DegreePrecision;

	//Cosine interpolation - Worse because the switch is sudden between 0.5 and 0.71 (equivalent to 30 degree and 
	//float minRatio = sin((alpha2 - alphaDegree)/_DegreePrecision * PI / 2.0);

	//Quadratic interpolation - Worse because of low blending => prominent seperation (more tending to 0)
	//float minRatio = (alpha2 - alphaDegree)/_DegreePrecision; minRatio *= minRatio;
	
	//Square root / (nth root) interpolation
	float minRatio = (alpha2*180.0/PI - alphaDegree)/_DegreePrecision; 
	minRatio = pow(minRatio, _InterpolationPower);
	float maxRatio = 1.0 - minRatio;
	
	fixed4 minColor = getAtlasColor(minUV.z, alphaMinCoords);
	fixed4 maxColor = getAtlasColor(maxUV.z, alphaMaxCoords);
	
	return (minColor*minRatio + maxColor*maxRatio) * _Color;
}

fixed4 LightFieldBlend(Input IN)
{
	float clampedY = (_OffsetScale*IN.uv_Atlas0.y - 0.5) + 0.5;
	if(clampedY <= 0.001 || clampedY >= 0.99) return fixed4(0,0,0,0);
	
	const float PI = 3.14159265359;
	
	float d = _CaptureDistanceSizeRatio * _SurfaceSize;
	float width = _SurfaceSize;
	
	float3 userCamDirection = mul(_World2Object, float4(_WorldSpaceCameraPos, 1)).xyz;

	float viewDistance = length (userCamDirection.xyz);
	float s = viewDistance*_OffsetScale;
	
	float cosView = userCamDirection.z / length(userCamDirection.xz);
	float sinView = userCamDirection.x / length(userCamDirection.xz);
	
	float camAngle = acos (cosView);
	if (sinView > 0) camAngle = -camAngle;
	
	float theta = camAngle - _ViewAngle + PI;
	theta -= floor(theta/(PI*2)) * PI*2;
	
	float x = (IN.uv_Atlas0.x - 0.5)*width;
	float xSign = x/abs(x);
	//Work with positive x, since x is symmetric, and consistent across trigonometric functions.
	x *= xSign;
	float gamma = atan(s/x);
	float alphaLessTheta = acos(s/d * cos(gamma)) - gamma;
	//Alpha is a difference WRT theta
	float alpha = theta + xSign*alphaLessTheta;
	
	//_x for x' in the derivation. Set it's sign according to xSign
	float _x = d * tan(asin(x/d*sin(gamma)));
	_x *= xSign;
	float normalized_x = _x/width;
	float clampedX = normalized_x*_OffsetScale + 0.5;
	if(clampedX <= 0.001 || clampedX >= 0.99) return fixed4(0,0,0,0);
	
	return getBlinearInterpolatedColor(IN, float2(clampedX, clampedY), xSign, gamma, alpha, alphaLessTheta);	
}

void vert (inout appdata_full v) {
	
	float3 userCamDirection = mul(_World2Object, float4(_WorldSpaceCameraPos, 1)).xyz;
	float cosTheta = userCamDirection.z / length(userCamDirection.xz);
	float sinTheta = userCamDirection.x / length(userCamDirection.xz);
	v.vertex.xz = float2(	v.vertex.x * cosTheta + v.vertex.z * sinTheta, 
							v.vertex.z * cosTheta - v.vertex.x * sinTheta);

}

void surf (Input IN, inout SurfaceOutput o) {
	
	//fixed4 c = tex2D(_Atlas0, thetaCoords) * _Color;
	fixed4 c = LightFieldBlend(IN);
	
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Transparent/Cutout/Diffuse"
}
