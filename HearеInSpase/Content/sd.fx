float4x4 View;
float4x4 Projection;
texture Texture;
float2 Size;

sampler2D textureSampler = sampler_state {
	Texture = (Texture);
	MagFilter = Linear;
	MinFilter = Linear;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position :  SV_Position0;
	float2 TextureCoordinate : TEXCOORD0; 
	float2 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position0;
	float2 TextureCoordinate : TEXCOORD1; 
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	//Делаем так чтобы наш квадрат всегда был лицом к камере.
	VertexShaderOutput output;
	//Переводим позицию вертекса в трехмерный вектор.
	float3 position = input.Position;
	//Расчитываме позицию вертекса в матрицие камеры.
	// float4(position, 1) - это мировая матрица направленная на 1 по оси Z; Identity матрица. с центром в поции вертекса.
	float4 viewPosition = mul(float4(position, 1), View); 
	//Смешаем вертекс по оси X и Y на дистанцию равную размерам квадрата. В соответстви его позии в квадрате (0,0 0,1 1,1 1,0) или ( -1,1 1,1 1, -1, -1,1).
	viewPosition.xy += input.Normal * Size;
	//Находим позицию вертекса в матрице проекции.
	output.Position = mul(viewPosition, Projection);
	//Устанавливаем коодинаты для текстуры вектекса.
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_Target0
{
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	return textureColor;
}

technique sd
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile  ps_4_0 PixelShaderFunction();
	}
}

/*float4 worldPosition = mul(input.Position, World);
float4 viewPosition = mul(worldPosition, View);
output.Position = mul(viewPosition, Projection);*/