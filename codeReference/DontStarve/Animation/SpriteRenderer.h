#pragma once

#include "../Component.h"
#include "Sprite.h"

class Transform;

// 월드 오브젝트의 이미지와 레이어를 관리하는 컴포넌트
class SpriteRenderer : public Component
{
protected:
	std::shared_ptr<Sprite> m_sprite;		// 렌더링할 스프라이트
	RenderLayer m_layer;					// 렌더 레이어
	bool m_preFlipped;						// 이미 좌우 반전된 리소스인지 여부 
	Gdiplus::Color m_rendererTintColor;		// 렌더링 틴트 색상

public:
	SpriteRenderer(GameObject* owner, RenderLayer layer = LAYER_WORLD_OBJECT);
	virtual ~SpriteRenderer();

	virtual void Init() override;
	virtual void Release() override;

	void Render();

	// 스프라이트 Getter/Setter
	Gdiplus::Bitmap* GetSprite() const { return m_sprite ? m_sprite->bitmap.get() : nullptr; }
	void SetSprite(const std::shared_ptr<Sprite>& sprite) { m_sprite = sprite; }
	std::shared_ptr<Sprite> GetSpriteHandle() const { return  m_sprite; }

	// 레이어 Getter/Setter
	RenderLayer GetLayer() const { return m_layer; }
	void SetLayer(RenderLayer layer) { m_layer = layer; }

	// 피벗 제어 (Sprite 데이터에 직접 접근)
	float GetPivotX() const { return m_sprite->pivot.X; }
	float GetPivotY() const { return m_sprite->pivot.Y; }
	void SetPivot(float x, float y) { if (m_sprite) { m_sprite->pivot.X = x; m_sprite->pivot.Y = y; } }

	// PreFlipped Getter/Setter
	bool IsPreFlipped() const { return m_preFlipped; }
	void SetPreFlipped(bool flipped) { m_preFlipped = flipped; }

	// 틴트 색상 Getter/Setter
	Gdiplus::Color GetTintColor() const { return m_rendererTintColor; }
	void SetTintColor(const Gdiplus::Color& color) { m_rendererTintColor = color; }
	void SetTintColor(BYTE r, BYTE g, BYTE b, BYTE a = 255) { m_rendererTintColor = Gdiplus::Color(a, r, g, b); }
};

