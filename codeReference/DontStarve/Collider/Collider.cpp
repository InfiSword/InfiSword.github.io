#include "99_Default/pch.h"
#include "Collider.h"
#include "../../../01_Manager/ColliderManager/ColliderManager.h"
#include "../../GameObject.h"

Collider::Collider(GameObject* owner) 
    : Component(owner)
{
    m_isInteractionCollider = true;
}

void Collider::Init()
{
    Component::Init();
}

void Collider::Release()
{
    Component::Release();
}

void Collider::Update(float deltaTime)
{
    Component::Update(deltaTime);
}

