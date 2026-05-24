#include "99_Default/pch.h"
#include "ResourceManager.h"

ResourceManager::ResourceManager()
{
}

ResourceManager::~ResourceManager()
{
	Release();
}

void ResourceManager::Init()
{
}

void ResourceManager::Release()
{
    m_spriteCache.clear();
    m_spriteSheetCache.clear();
}

std::shared_ptr<Sprite> ResourceManager::LoadSprite(const std::wstring& fullPath, const Gdiplus::PointF& pivot)
{
	if (fullPath.empty()) return nullptr;

	auto found = m_spriteCache.find(fullPath);
	if (found != m_spriteCache.end()) {
		if (std::shared_ptr<Sprite> cached = found->second.lock()) {
			return cached;
		}
		m_spriteCache.erase(found);
	}

	std::shared_ptr<Sprite> sprite = Sprite::CreateFromFile(fullPath, pivot);
	if (!sprite) return nullptr;

	m_spriteCache[fullPath] = sprite;
	return sprite;
}

std::shared_ptr<SpriteSheet> ResourceManager::LoadSpriteSheet(
    const std::wstring& imagePath,
    UINT frameWidth, UINT frameHeight,
    UINT framesPerRow, UINT totalFrames,
    bool flipHorizontal,
    const Gdiplus::PointF& pivot)
{
    std::wstring key = imagePath
        + L"_" + std::to_wstring(frameWidth)
        + L"x" + std::to_wstring(frameHeight)
        + L"_" + std::to_wstring(framesPerRow)
        + L"x" + std::to_wstring(totalFrames)
        + (flipHorizontal ? L"_flip" : L"");

    auto it = m_spriteSheetCache.find(key);
    if (it != m_spriteSheetCache.end()) {
        if (auto cached = it->second.lock()) {
            return cached;
        }
        m_spriteSheetCache.erase(it);
    }

    std::shared_ptr<SpriteSheet> sheet = SpriteSheet::CreateFromFile(imagePath, frameWidth, frameHeight, framesPerRow, totalFrames, pivot, flipHorizontal);
    if (!sheet) return nullptr;

    m_spriteSheetCache[key] = sheet;
    return sheet;
}
