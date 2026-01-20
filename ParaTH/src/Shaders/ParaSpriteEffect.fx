#define BEGIN_CONSTANTS
#define MATRIX_CONSTANTS
#define END_CONSTANTS

#define _vs(r)  : register(vs, r)
#define _ps(r)  : register(ps, r)
#define _cb(r)

#define DECLARE_TEXTURE(Name, index) \
    texture2D Name; \
    sampler Name##Sampler : register(s##index) = sampler_state { Texture = (Name); };

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name##Sampler, texCoord)

DECLARE_TEXTURE(Texture, 0);

BEGIN_CONSTANTS
MATRIX_CONSTANTS
    float4x4 MatrixTransform    _vs(c0) _cb(c0);
END_CONSTANTS

void SpriteVertexShader(inout float4 color    : COLOR0,
                        inout float2 texCoord : TEXCOORD0,
                        inout float4 position : SV_Position)
{
    position = mul(position, MatrixTransform);
}

float4 SpritePixelShader(float4 color : COLOR0,
                         float2 texCoord : TEXCOORD0) : SV_Target0
{
    return SAMPLE_TEXTURE(Texture, texCoord) * color;
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile vs_2_0 SpriteVertexShader();
        PixelShader  = compile ps_2_0 SpritePixelShader();
    }
}
