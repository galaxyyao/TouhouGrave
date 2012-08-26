
float2 Draw_VPPageSize;		// page size in render target space
float2 Draw_SrcPageSize;	// page size in source texture space
float4x4 Draw_WorldViewProj;

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

	oPos = float4(iPos + iCorner * Draw_VPPageSize, 0, 1);
	oPos = mul(oPos, Draw_WorldViewProj);
	oUV = (iUV_Mask.xy + iCorner) * Draw_SrcPageSize;
	oColor = iColor;
	oChannel = masks[(int)iUV_Mask.z];
}

void DrawPS(
	in float2 iUV		: TEXCOORD0,
	in float4 iColor	: COLOR0,
	in float4 iChannel	: COLOR1,
	out float4 oColor	: COLOR0)
{
	oColor = dot(tex2D(LinearSampler, iUV), iChannel).xxxx * iColor;
}

technique BlitToRT
{ pass {
	VertexShader = compile vs_2_0 BlitVS();
	PixelShader = compile ps_2_0 BlitPS();
} }

technique DrawText
{ pass {
	VertexShader = compile vs_2_0 DrawVS();
	PixelShader = compile ps_2_0 DrawPS();
} }
