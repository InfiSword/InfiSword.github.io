#include "99_Default/pch.h"
#include "SpriteRenderer.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/CameraManager/CameraManager.h"
#include "../../../02_GameObject/GameObject.h"
#include "../../../02_GameObject/Component/Transform/Transform.h"

SpriteRenderer::SpriteRenderer(GameObject* owner, RenderLayer layer)
	: Component(owner), m_sprite(nullptr), m_layer(layer), m_preFlipped(false), m_rendererTintColor(255, 255, 255, 255)
{
}

SpriteRenderer::~SpriteRenderer()
{
	Release();
}

void SpriteRenderer::Init()
{

}

void SpriteRenderer::Render()
{
	Transform* pTransform = m_owner->GetComponent<Transform>();
	if (!pTransform || !m_sprite || !m_sprite->bitmap) return;

	float worldX = pTransform->GetX();
	float worldY = pTransform->GetY();
	
	// 정렬 기준점 계산 (피벗 고려)
	float height = m_sprite->sourceRect.Height * pTransform->GetScaleY();
	float sortingY = worldY + (1.0f - m_sprite->pivot.Y) * height;

	Gdiplus::Color tintColor = m_rendererTintColor;
	bool hasTint = (tintColor.GetValue() != Gdiplus::Color::MakeARGB(255, 255, 255, 255));

	// RenderManager에 월드 좌표와 변환 정보 전달
	RenderManager::GetInstance()->AddWorldObjectCommand(
		m_sprite->bitmap.get(), 
		m_sprite->sourceRect, 
		worldX, worldY, 
		pTransform->GetScaleX(), pTransform->GetScaleY(),
		m_sprite->pivot.X, m_sprite->pivot.Y,
		m_layer, sortingY, 
		pTransform->GetDirection(), 
		tintColor, hasTint, m_preFlipped
	);
}

void SpriteRenderer::Release()
{
	m_sprite.reset();
}
