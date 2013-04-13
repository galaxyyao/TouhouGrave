
float2 Draw_PageSize;
float2 Draw_PageUVSize;
float4x4 Draw_WorldViewProj;
float2 Draw_TextureSize;
float2 Draw_InvTextureSize;
float2 Draw_NumPages;
float2 Draw_InvNumPages;
float4 Draw_ColorScaling;
float4 Draw_OutlineColor;

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
	float4 texColor = tex2D(PointSampler, iUV);
	oClr = texColor.r * texColor.a;
}

void DrawVS(
	in float2 iCorner		: POSITION0,
	in float2 iPos			: TEXCOORD0,
	in float4 iUV_Mask		: COLOR0,
	in float4 iColor		: COLOR1,
	out float4 oPos			: POSITION0,
	out float4 oUV_PageXY	: TEXCOORD0,
	out float4 oColor		: COLOR0,
	out float4 oChannel		: COLOR1)
{
	oPos = float4(iPos + iCorner * Draw_PageSize, 0, 1);
	oPos = mul(oPos, Draw_WorldViewProj);
	oUV_PageXY.xy = (iUV_Mask.xy + iCorner) * Draw_PageUVSize;
	oUV_PageXY.zw = iUV_Mask.xy;
	oColor = iColor * Draw_ColorScaling;
	oChannel.xy = iUV_Mask.zw;
}

float ComputeMipmapLevel(float2 dx, float2 dy, float2 textureSize)
{
	dx *= textureSize;
	dy *= textureSize;
	float d = max(dot(dx, dx), dot(dy, dy));
	return 0.5f * log2(d);
}

float4 GetPageUV(float2 pageXY)
{
	return float4(pageXY, pageXY + 1) * Draw_InvNumPages.xyxy;
}

void DrawPS(
	in float4 iUV_PageXY	: TEXCOORD0,
	in float4 iColor		: COLOR0,
	in float4 iChannel		: COLOR1,
	out float4 oColor		: COLOR0)
{
	float2 iUV = iUV_PageXY.xy;
	float2 dx = ddx(iUV);
	float2 dy = ddy(iUV);
	float lod = ceil(max(ComputeMipmapLevel(dx, dy, Draw_TextureSize), 0));
	float2 halfBorderSizeLod = exp2(lod).xx * Draw_InvTextureSize * 0.5f;
	float4 pageUVRange = GetPageUV(iUV_PageXY.zw);
	iUV = max(iUV, pageUVRange.xy + halfBorderSizeLod);
	iUV = min(iUV, pageUVRange.zw - halfBorderSizeLod);

	const float4 masks[] = {
		{ 0, 0, 0, 1 },
		{ 1, 0, 0, 0 },
		{ 0, 1, 0, 0 },
		{ 0, 0, 1, 0 },
	};

	float texColor = dot(tex2D(LinearSampler, iUV, dx, dy), masks[(int)iChannel.x]);

	if (iChannel.y == 1)
	{
		// outline
		texColor *= 2;
		if (texColor < 1)
		{
			oColor = texColor * Draw_OutlineColor;
		}
		else
		{
			oColor = lerp(Draw_OutlineColor, iColor, texColor - 1);
		}
	}
	else
	{
		oColor = texColor * iColor;
	}
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
