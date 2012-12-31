float4x4 Transform;
float3 PileSize;
float4 UVAdjust;
float4 ColorBias;
float4 ColorScale;

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
    float3 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4 pos = float4(input.Position * PileSize, 1);
	output.Position = mul(pos, Transform);
	output.UV = input.UV * UVAdjust.xy + UVAdjust.zw;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(TheSampler, input.UV) * ColorScale + ColorBias;
}

technique Normal
{ pass {
    VertexShader = compile vs_2_0 VertexShaderFunction();
    PixelShader = compile ps_2_0 PixelShaderFunction();
} }
