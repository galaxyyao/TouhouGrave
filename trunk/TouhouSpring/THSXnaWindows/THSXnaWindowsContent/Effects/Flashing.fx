float4x4 World;
float4x4 View;
float4x4 Projection;
float AppTime;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 UV		: TEXCOORD0;
	float4 Color	: COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4x4 worldViewProj = mul(mul(World, View), Projection);
	output.Position = mul(input.Position, worldViewProj);
	output.UV = input.UV;
	output.Color = abs(sin(AppTime)).xxxx;
    return output;
}

texture TheTexture;
sampler TheSampler = sampler_state
{
	texture = <TheTexture>;
	AddressU = clamp;
	AddressV = clamp;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(TheSampler, input.UV) * input.Color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
