float2 Corners[4];
float2x3 ExpandMatrices[13];

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
	float3 Position			: POSITION0;
	float2 Corner_Expand	: POSITION1;
	float3 SizeAndRotation	: POSITION2;
	float4 UV				: TEXCOORD0;
	float4 LocalFrameCol0	: TEXCOORD1;
	float4 LocalFrameCol1	: TEXCOORD2;
	float4 LocalFrameCol2	: TEXCOORD3;
	float4 LocalFrameCol3	: TEXCOORD4;
	float4 Color			: COLOR0;
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

	float2 corner = Corners[input.Corner_Expand.x];

	float sinTheta, cosTheta;
	sincos(input.SizeAndRotation.z / 180 * 3.1415926f, sinTheta, cosTheta);
	float2 rotatedCorner = { dot(float2(cosTheta, sinTheta), corner),
							 dot(float2(-sinTheta, cosTheta), corner) };
	float2 expand = rotatedCorner * input.SizeAndRotation.xy;
	float3 offset = mul(expand, ExpandMatrices[input.Corner_Expand.y]);

	float4 hPos = float4(input.Position + offset, 1);
	float4 tPos = { dot(hPos, input.LocalFrameCol0),
					dot(hPos, input.LocalFrameCol1),
					dot(hPos, input.LocalFrameCol2),
					dot(hPos, input.LocalFrameCol3) };
	output.Position = mul(tPos, Transform);

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
