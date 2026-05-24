#pragma once

#include "../../02_GameObject/Component/Sprite/Sprite.h"
#include "../../02_GameObject/Component/Sprite/SpriteSheet.h"

class ResourceManager : public CSingleTon<ResourceManager>
{
    friend class CSingleTon<ResourceManager>;
private:
    ResourceManager();
    ~ResourceManager();

public:
    void Init();
    void Release();

    std::shared_ptr<Sprite> LoadSprite(const std::wstring& fullPath, const Gdiplus::PointF& pivot = { 0.5f, 0.5f });

    // SpriteSheet 캐시 로드 - 동일 경로+반전 조합은 공유 포인터 반환
    std::shared_ptr<SpriteSheet> LoadSpriteSheet(
        const std::wstring& imagePath,
        UINT frameWidth, UINT frameHeight,
        UINT framesPerRow, UINT totalFrames,
        bool flipHorizontal = false,
        const Gdiplus::PointF& pivot = { 0.5f, 1.0f });

private:
    std::unordered_map<std::wstring, std::weak_ptr<Sprite>> m_spriteCache;
    std::unordered_map<std::wstring, std::weak_ptr<SpriteSheet>> m_spriteSheetCache;
};
