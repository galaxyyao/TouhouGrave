
float2 Draw_PageSize;
float2 Draw_PageUVSize;
float4x4 Draw_WorldViewProj;
float2 Draw_TextureSize;
float2 Draw_InvTextureSize;
float2 Draw_NumPages;
float2 Draw_InvNumPages;
float4 Draw_ColorScaling;

texture TheTexture;

sampler PointSampler = sampler_state
{
	texture = <TheTexture>;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MinFilter = POINT;
	MagFilter = POINT;
	MipFilter = POINT;
};

sampler LinearSampler = sampler_state
{
	texture = <TheTexture>;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};

void BlitVS(
	in float2 iPos	: POSITION0,
	in float2 iUV	: TEXCOORD0,
	out float4 oPos	: POSITION0,
	out float2 oUV	: TEXCOORD0)
{
	oPos = float4(iPos, 0, 1);
	oUV = iUV;
}

void BlitPS(
	in float2 iUV	: TEXCOORD0,
	out float4 oClr	: COLOR0)
{
	oClr = tex2D(PointSampler, iUV).aaaa;
}

void DrawVS(
	in float2 iCorner	: POSITION0,
	in float2 iPos		: TEXCOORD0,
	in float4 iUV_Mask	: COLOR0,
	in float4 iColor	: COLOR1,
	out float4 oPos		: POSITION0,
	out float2 oUV		: TEXCOORD0,
	out float4 oColor	: COLOR0,
	out float4 oChannel	: COLOR1)
{
	const float4 masks[] = {
		{ 0, 0, 0, 1 },
		{ 1, 0, 0, 0 },
		{ 0, 1, 0, 0 },
		{ 0, 0, 1, 0 },
	};

	oPos = float4(iPos + iCorner * Draw_PageSize, 0, 1);
	oPos = mul(oPos, Draw_WorldViewProj);
	oUV = (iUV_Mask.xy + iCorner) * Draw_PageUVSize;
	oColor = iColor * Draw_ColorScaling;
	oChannel = masks[(int)iUV_Mask.z];
}

float ComputeMipmapLevel(float2 dx, float2 dy, float2 textureSize)
{
	dx *= textureSize;
	dy *= textureSize;
	float d = max(dot(dx, dx), dot(dy, dy));
	return 0.5f * log2(d);
}

float4 GetPageUV(float2 uv)
{
	float2 pageXY = uv * Draw_NumPages;
	float2 rangeLo = floor(pageXY) * Draw_InvNumPages;
	float2 rangeHi = ceil(pageXY) * Draw_InvNumPages;
	return float4(rangeLo, rangeHi);
}

void DrawPS(
	in float2 iUV		: TEXCOORD0,
	in float4 iColor	: COLOR0,
	in float4 iChannel	: COLOR1,
	out float4 oColor	: COLOR0)
{
	float2 dx = ddx(iUV);
	float2 dy = ddy(iUV);
	float lod = ComputeMipmapLevel(dx, dy, Draw_TextureSize);
	float2 halfBorderSizeLod = exp2(lod).xx * Draw_InvTextureSize * 0.5f;
	float4 pageUVRange = GetPageUV(iUV);
	iUV = max(iUV, pageUVRange.xy + halfBorderSizeLod);
	iUV = min(iUV, pageUVRange.zw - halfBorderSizeLod);
	oColor = dot(tex2D(LinearSampler, iUV, dx, dy), iChannel).xxxx * iColor;
}

technique BlitToRT
{ pass {
	VertexShader = compile vs_2_0 BlitVS();
	PixelShader = compile ps_2_0 BlitPS();
} }

technique DrawText
{ pass {
	VertexShader = compile vs_3_0 DrawVS();
	PixelShader = compile ps_3_0 DrawPS();
} }
