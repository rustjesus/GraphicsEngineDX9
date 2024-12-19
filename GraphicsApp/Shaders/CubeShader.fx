// Declare matrices
float4x4 WorldViewProjection; // Combined matrix
float4x4 ViewMatrix; // View matrix (optional if used separately)
float4x4 ProjectionMatrix; // Projection matrix (optional if used separately)

// Input and output structures
struct VertexInput
{
    float3 position : POSITION; // Vertex position
    float4 color : COLOR; // Vertex color
};

struct PixelInput
{
    float4 position : POSITION; // Transformed position
    float4 color : COLOR; // Interpolated color
};

// Vertex Shader
PixelInput VS(VertexInput input)
{
    PixelInput output;
    // Transform the position using the combined WorldViewProjection matrix
    output.position = mul(float4(input.position, 1.0), WorldViewProjection);
    output.color = input.color; // Pass color to the pixel shader
    return output;
}

// Pixel Shader
float4 PS(PixelInput input) : COLOR
{
    return input.color; // Output the color for each pixel
}

// Techniques
technique RenderTechnique
{
    pass Pass1
    {
        // Use shaders compiled for shader model 3.0
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS();
    }
}
