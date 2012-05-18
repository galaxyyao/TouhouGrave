float4x4 Transform;
float4 Color;

struct VertexShaderInput
{
    float2 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = mul(float4(input.Position, 0, 1), Transform);
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return Color;
}

technique Normal
{
    pass
    {
        Texture[0] = null;
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
