#ifndef MARMOSET_SH_CGINC
#define MARMOSET_SH_CGINC
float	_SHScale;
float3	_SH0;
float3	_SH1;
float3	_SH2;
float3	_SH3;
float3	_SH4;
float3	_SH5;
float3	_SH6;
float3	_SH7;
float3	_SH8;


void SHLookup(float3 dir, out float3 band0, out float3 band1, out float3 band2) {
	//l = 0 band (constant)
	band0 = _SH0.xyz;

	//l = 1 band
	band1 =  _SH1.xyz * dir.y;
	band1 += _SH2.xyz * dir.z;
	band1 += _SH3.xyz * dir.x;

	//l = 2 band
	float3 swz = dir.yyz * dir.xzx;
	band2 =  _SH4.xyz * swz.x;
	band2 += _SH5.xyz * swz.y;
	band2 += _SH7.xyz * swz.z;
	float3 sqr = dir * dir;
	band2 += _SH6.xyz * ( 3.0*sqr.z - 1.0 );
	band2 += _SH8.xyz * ( sqr.x - sqr.y );
}
/*
// normal should be normalized, w=1.0
half3 ShadeSH9 (half4 normal)
{
	half3 x1, x2, x3;
	
	// Linear + constant polynomial terms
	x1.r = dot(unity_SHAr,normal);
	x1.g = dot(unity_SHAg,normal);
	x1.b = dot(unity_SHAb,normal);
	
	// 4 of the quadratic polynomials
	half4 vB = normal.xyzz * normal.yzzx;
	x2.r = dot(unity_SHBr,vB);
	x2.g = dot(unity_SHBg,vB);
	x2.b = dot(unity_SHBb,vB);
	
	// Final quadratic polynomial
	float vC = normal.x*normal.x - normal.y*normal.y;
	x3 = unity_SHC.rgb * vC;
    return x1 + x2 + x3;
} 
*/
void SHLookupUnity(float3 dir, out float3 band0, out float3 band1, out float3 band2) {
	//constant term
	band0 = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);

	// Linear term
	band1.r = dot(unity_SHAr.xyz, dir.xyz);
	band1.g = dot(unity_SHAg.xyz, dir.xyz);
	band1.b = dot(unity_SHAb.xyz, dir.xyz);
	
	// 4 of the quadratic polynomials
	half4 vB = dir.xyzz * dir.yzzx;
	band2.r = dot(unity_SHBr,vB);
	band2.g = dot(unity_SHBg,vB);
	band2.b = dot(unity_SHBb,vB);

	// Final quadratic polynomial
	float vC = dir.x*dir.x - dir.y*dir.y;
	band2 += unity_SHC.rgb * vC;
}

float3 SHConvolve(float3 band0, float3 band1, float3 band2, float3 weight) {
	float3 conv1 = lerp( float3(1.0,1.0,1.0), float3(0.6667,0.6667,0.6667), weight);
	float3 conv2 = lerp( float3(1.0,1.0,1.0), float3(0.25,0.25,0.25), weight);
	conv1 = lerp(conv1, conv1*conv1, weight);
	conv2 = lerp(conv2, conv2*conv2, weight);
	return band0 + band1*conv1 + band2*conv2;
}

float3 lightingSH(float3 band0, float3 band1, float3 band2) {
	return (band0 + band1 + band2);// * _SHScale;
}

float3 wrapLightingSH(float3 band0, float3 band1, float3 band2, float3 scatter) {
	return SHConvolve(band0,band1,band2,scatter);
}


#endif
