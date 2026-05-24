#pragma once

#include "../Component.h"
#include "Sprite.h"

class GameObject;
class RectTransform;	

namespace ComponentElement 
{
	struct ImageStyle {
		RenderLayer layer;
		float sortKey;
	};

	class Image : public Component
	{
	protected:
		std::shared_ptr<Sprite> m_sprite;
		RenderLayer m_layer;
		float m_sortKey;
		Gdiplus::Color m_tintColor;  // 이미지 틴트 색상

	public:
		Image(GameObject* owner, RenderLayer layer = LAYER_UI_BACKGROUND, float sortKey = 0.0f);
		virtual ~Image();

		virtual void Init() override;
		virtual void Release() override;

		void Render();

		Gdiplus::Bitmap* GetSprite() const { return m_sprite ? m_sprite->bitmap.get() : nullptr; }
		std::shared_ptr<Sprite> GetSpriteHandle() const { return m_sprite; }
		void SetSprite(const std::shared_ptr<Sprite>& sprite) { m_sprite = sprite; }

		RenderLayer GetLayer() const { return m_layer; }
		void SetLayer(RenderLayer layer) { m_layer = layer; }

		float GetSortKey() const { return m_sortKey; }
		void SetSortKey(float sortKey) { m_sortKey = sortKey; }

		// 색상 Getter/Setter
		Gdiplus::Color GetTintColor() const { return m_tintColor; }
		void SetTintColor(const Gdiplus::Color& color) { m_tintColor = color; }
		void SetTintColor(BYTE r, BYTE g, BYTE b, BYTE a = 255) { m_tintColor = Gdiplus::Color(a, r, g, b); }
		
		// 알파값만 설정
		void SetAlpha(BYTE alpha) { m_tintColor = Gdiplus::Color(alpha, m_tintColor.GetR(), m_tintColor.GetG(), m_tintColor.GetB()); }

		// 피벗 제어 (Sprite 데이터에 직접 접근)
		float GetPivotX() const { return m_sprite ? m_sprite->pivot.X : 0.5f; }
		float GetPivotY() const { return m_sprite ? m_sprite->pivot.Y : 0.5f; }
		void SetPivot(float x, float y) { if (m_sprite) { m_sprite->pivot.X = x; m_sprite->pivot.Y = y; } }

		void ApplyStyle(const ImageStyle& style) { m_layer = style.layer; m_sortKey = style.sortKey; }

		void LoadSprite(const std::wstring& fullPath);

		/// 소유 오브젝트의 RectTransform에 목표 표시 크기로 스케일 적용 
		void SetDisplaySize(float width, float height) const;
		/// 원본 비율 유지하며 긴 변이 maxSize가 되도록 스케일 적용
		void SetDisplaySizeProportional(float maxSize) const;
	};
}
