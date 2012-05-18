float4x4 Transform;
float4 PosAdjust;
float4 UVAdjust;
float4 ColorBias;
float4 ColorScale;

// x: saturate (blend factor from fully grayscaled (0) to normal (1))
// y: brightness
float2 ToneParameters;

texture TheTexture;
sampler TheSampler = sampler_state
{
	texture = <TheTexture>;
	AddressU = clamp;
	AddressV = clamp;
	MinFilter = Linear;
	MagFilter = Linear;
};

struct VertexShaderInput
{
    float2 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4 pos = float4(input.Position * PosAdjust.xy + PosAdjust.zw, 0, 1);
	output.Position = mul(pos, Transform);
	output.UV = input.Position * UVAdjust.xy + UVAdjust.zw;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(TheSampler, input.UV) * ColorScale + ColorBias;
}

float4 PixelShaderFunctionTone(VertexShaderOutput input) : COLOR0
{
	float4 clr = PixelShaderFunction(input);
	const float3 Intensity = { 0.30f, 0.59f, 0.11f };
	float grayscaled = dot(clr.rgb, Intensity);
	return float4(lerp(grayscaled.xxx, clr.rgb, ToneParameters.x) * ToneParameters.y, clr.a);
}

float4 PixelShaderFunctionSimple() : COLOR0
{
	return float4(0, 0, 0, 0);
}

technique Normal
{ pass {
    VertexShader = compile vs_2_0 VertexShaderFunction();
    PixelShader = compile ps_2_0 PixelShaderFunction();
} }

technique Tone
{ pass {
    VertexShader = compile vs_2_0 VertexShaderFunction();
    PixelShader = compile ps_2_0 PixelShaderFunctionTone();
} }

technique Simple
{ pass {
	VertexShader = compile vs_2_0 VertexShaderFunction();
	PixelShader = compile ps_2_0 PixelShaderFunctionSimple();

	Texture[0] = null;
	ColorWriteEnable = 0;
} }
