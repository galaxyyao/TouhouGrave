float2 Corners[4];
float3x3 Alignments[4];

float4x4 Transform;
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
    float3 Position	: POSITION0;
	float3 Indices	: POSITION1;
	float4 UV		: TEXCOORD0;
	float3 SizeAndRotation : TEXCOORD1;
	float4 Color	: COLOR0;
};

struct VertexShaderOutput
{
    float4 Position	: POSITION;
	float2 UV		: TEXCOORD0;
	float4 Color	: COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float2 corner = Corners[input.Indices.x];
	float3x3 alignment = Alignments[input.Indices.y];

	float sinTheta, cosTheta;
	sincos(input.SizeAndRotation.z / 180 * 3.1415926f, sinTheta, cosTheta);
	float2 rotatedCorner = { dot(float2(cosTheta, sinTheta), corner),
							 dot(float2(-sinTheta, cosTheta), corner) };
	float2 expand = rotatedCorner * input.SizeAndRotation.xy;

	output.Position = mul(float4(input.Position + mul(float3(expand, 0), alignment), 1), Transform);

	output.UV = corner * input.UV.xy + input.UV.zw;
	output.Color = input.Color;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return tex2D(TheSampler, input.UV) * input.Color;
}

technique Normal
{
    pass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
