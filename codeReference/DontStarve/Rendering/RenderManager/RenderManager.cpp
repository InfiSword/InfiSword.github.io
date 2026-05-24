#include "99_Default/pch.h"
#include "RenderManager.h"
#include "../../01_Manager/CameraManager/CameraManager.h"
#include "../../02_GameObject/Component/Sprite/SpriteSheet.h"
#include "../../99_Default/ClientOptimatzationOption.h"

RenderManager::RenderManager()
{
	for (int i = 0; i < LAYER_COUNT; ++i) {
		m_layerCommands[i].reserve(512);
	}

}

RenderManager::~RenderManager()
{
	Release();
}

void RenderManager::Init()
{
	// GDI+ 객체 캐싱 초기화
	m_pCachedPen = new Gdiplus::Pen(Gdiplus::Color(0, 0, 0, 0));

	m_pCachedBrush = new Gdiplus::SolidBrush(Gdiplus::Color(0, 0, 0, 0));

	m_pCachedAttr = new Gdiplus::ImageAttributes();
}

void RenderManager::LateInit()
{
}

void RenderManager::Update(float deltaTime)
{
}

void RenderManager::LateUpdate()
{
}

void RenderManager::BeginFrame(const Gdiplus::PointF& cameraPos)
{
	m_frameCameraPos = cameraPos;
	m_hasFrameCameraPos = true;
#ifdef _DEBUG
	ResetRenderStats();
#endif
}

void RenderManager::Release()
{
	Clear();

	Utils::SafeDelete(m_pCachedPen);
	Utils::SafeDelete(m_pCachedBrush);
	Utils::SafeDelete(m_pCachedAttr);
}

void RenderManager::AddWorldObjectCommand
(Gdiplus::Bitmap* pBitmap, const Gdiplus::RectF& sourceRect, float worldX, float worldY,
	float scaleX, float scaleY, float pivotX, float pivotY, RenderLayer layer, float zOrder,
	Direction direction, const Gdiplus::Color& tintColor, bool hasTint, bool preFlipped)
{
	const Gdiplus::PointF currentCamPos = m_hasFrameCameraPos ? m_frameCameraPos : CameraManager::GetInstance()->GetCameraPos();
	float screenX = worldX - currentCamPos.X + (float)WINCX * 0.5f;
	float screenY = worldY - currentCamPos.Y + (float)WINCY * 0.5f;
	Gdiplus::PointF screenPos(screenX, screenY);

	float width = sourceRect.Width * scaleX;
	float height = sourceRect.Height * scaleY;

	if (width <= 0.0f || height <= 0.0f) {
		return;
	}

	float renderX = screenPos.X - width * pivotX;
	float renderY = screenPos.Y - height * pivotY;

	DrawCommand cmd;
	cmd.type = DRAW_COMMAND_ENTITY;
	cmd.destRect = Gdiplus::RectF(renderX, renderY, width, height);
	cmd.layer = layer;
	cmd.zOrder = zOrder;

	cmd.sprite.pBitmap = pBitmap;
	cmd.sprite.sourceRect = sourceRect;
	cmd.sprite.srcUnit = Gdiplus::UnitPixel;
	cmd.sprite.direction = direction;
	cmd.sprite.tintColor = tintColor;
	cmd.sprite.hasTint = hasTint;
	cmd.sprite.preFlipped = preFlipped;

	m_layerCommands[layer].push_back(cmd);
}

void RenderManager::AddUICommand
(Gdiplus::Bitmap* pBitmap, const Gdiplus::RectF& sourceRect, float screenX, float screenY,
	float scaleX, float scaleY, float pivotX, float pivotY, RenderLayer layer, float zOrder,
	const Gdiplus::Color& tintColor, bool hasTint)
{
	float width = sourceRect.Width * scaleX;
	float height = sourceRect.Height * scaleY;
	float renderX = screenX - width * pivotX;
	float renderY = screenY - height * pivotY;

	DrawCommand cmd;
	cmd.type = DRAW_COMMAND_UI_IMAGE;
	cmd.destRect = Gdiplus::RectF(renderX, renderY, width, height);
	cmd.layer = layer;
	cmd.zOrder = zOrder;

	cmd.sprite.pBitmap = pBitmap;
	cmd.sprite.sourceRect = sourceRect;
	cmd.sprite.srcUnit = Gdiplus::UnitPixel;
	cmd.sprite.tintColor = tintColor;
	cmd.sprite.hasTint = hasTint;

	m_layerCommands[layer].push_back(cmd);
}

void RenderManager::AddTextCommand(const std::wstring* text, Gdiplus::Font* pFont, Gdiplus::Brush* pBrush,
	Gdiplus::StringFormat* pStringFormat, const Gdiplus::RectF& destRect, RenderLayer layer, float zOrder)
{
	DrawCommand cmd;
	cmd.type = DRAW_COMMAND_TEXT;
	cmd.destRect = destRect;
	cmd.layer = layer;
	cmd.zOrder = zOrder;

	cmd.text.textPtr = text;
	cmd.text.pFont = pFont;
	cmd.text.pBrush = pBrush;
	cmd.text.pStringFormat = pStringFormat;

	m_layerCommands[layer].push_back(cmd);
}

void RenderManager::AddDrawRectCommand(const Gdiplus::RectF& rect, const Gdiplus::Color& color, float thickness, RenderLayer layer, float zOrder)
{
	DrawCommand cmd;
	cmd.type = DRAW_COMMAND_RECTANGLE;
	cmd.destRect = rect;
	cmd.layer = layer;
	cmd.zOrder = zOrder;

	cmd.primitive.color = color;
	cmd.primitive.thickness = thickness;
	cmd.primitive.isFilled = false;

	m_layerCommands[layer].push_back(cmd);
}

void RenderManager::AddFillRectangleCommand(const Gdiplus::RectF& rect, const Gdiplus::Color& color, RenderLayer layer, float zOrder)
{
	DrawCommand cmd;
	cmd.type = DRAW_COMMAND_FILL_RECTANGLE;
	cmd.destRect = rect;
	cmd.layer = layer;
	cmd.zOrder = zOrder;

	cmd.primitive.color = color;
	cmd.primitive.thickness = 0.0f;
	cmd.primitive.isFilled = true;

	m_layerCommands[layer].push_back(cmd);
}

void RenderManager::Clear()
{
	for (int i = 0; i < LAYER_COUNT; ++i) {
		m_layerCommands[i].clear();
	}
	m_hasFrameCameraPos = false;
#ifdef _DEBUG
	ResetRenderStats();
#endif
}

void RenderManager::Flush(Gdiplus::Graphics* pGraphics)
{
	if (!pGraphics) return;

	for (int i = LAYER_TILE_BACKGROUND; i < LAYER_COUNT; ++i) {
		if (m_layerCommands[i].empty()) continue;

#ifdef _DEBUG
		if (g_bEnableOptimizationMode && m_layerCommands[i].size() > 1) {
#else
		if (m_layerCommands[i].size() > 1) {
#endif
			std::stable_sort(m_layerCommands[i].begin(), m_layerCommands[i].end(), [](const DrawCommand& a, const DrawCommand& b) {
				if (a.zOrder != b.zOrder) {
					return a.zOrder < b.zOrder;
				}
				return a.layer < b.layer;
				});
		}

		for (const auto& cmd : m_layerCommands[i]) {
			ExecuteDrawCommand(pGraphics, cmd);
		}
		m_layerCommands[i].clear();
	}
	m_hasFrameCameraPos = false;
}

void RenderManager::ExecuteDrawCommand(Gdiplus::Graphics * pGraphics, const DrawCommand & cmd)
{
	if (!pGraphics) return;

	switch (cmd.type) {
	case DRAW_COMMAND_ENTITY:
	case DRAW_COMMAND_UI_IMAGE:
		RenderSprite(pGraphics, cmd.sprite, cmd.destRect);
		break;
	case DRAW_COMMAND_TEXT:
		if (cmd.text.textPtr && cmd.text.pBrush) {
			pGraphics->DrawString(cmd.text.textPtr->c_str(), -1, cmd.text.pFont, cmd.destRect, cmd.text.pStringFormat, cmd.text.pBrush);
		}
		break;
	case DRAW_COMMAND_RECTANGLE:
		if (m_pCachedPen) {
			m_pCachedPen->SetColor(cmd.primitive.color);
			m_pCachedPen->SetWidth(cmd.primitive.thickness);
			pGraphics->DrawRectangle(m_pCachedPen, cmd.destRect);
		}
		break;
	case DRAW_COMMAND_FILL_RECTANGLE:
		if (m_pCachedBrush) {
			m_pCachedBrush->SetColor(cmd.primitive.color);
			pGraphics->FillRectangle(m_pCachedBrush, cmd.destRect);
		}
		break;
	case DRAW_COMMAND_HIGHLIGHT:
	default:
		break;
	}

}

void RenderManager::RenderSprite(Gdiplus::Graphics* pGraphics, const DrawCommand::SpriteData& data, const Gdiplus::RectF& destRect)
{
	if (!data.pBitmap) return;

	if (data.hasTint && m_pCachedAttr) {
		float r = data.tintColor.GetR() / 255.0f;
		float g = data.tintColor.GetG() / 255.0f;
		float b = data.tintColor.GetB() / 255.0f;
		float a = data.tintColor.GetA() / 255.0f;
		Gdiplus::ColorMatrix matrix = { r,0,0,0,0, 0,g,0,0,0, 0,0,b,0,0, 0,0,0,a,0, 0,0,0,0,1 };
		m_pCachedAttr->SetColorMatrix(&matrix);
		pGraphics->DrawImage(data.pBitmap, destRect, data.sourceRect.X, data.sourceRect.Y, data.sourceRect.Width, data.sourceRect.Height, data.srcUnit, m_pCachedAttr);
	}
	else {
		pGraphics->DrawImage(data.pBitmap, destRect, data.sourceRect.X, data.sourceRect.Y, data.sourceRect.Width, data.sourceRect.Height, data.srcUnit);
	}
}


