#pragma once
#include <memory>
#include <vector>
#include <string>
#include <gdiplus.h>
#include <Struct.h>
#include "Sprite.h"

// SpriteSheet 클래스: 스프라이트 시트 이미지를 관리하고 Sprite 객체들을 생성/보관
class SpriteSheet {
public:
    SpriteSheet(std::vector<std::shared_ptr<Sprite>> sprites);
    
    // 파일로부터 SpriteSheet 생성하는 팩토리 메서드
    // flipHorizontal: true면 로드 시 비트맵을 좌우 반전해 저장 (렌더 시 Transform 불필요)
    static std::shared_ptr<SpriteSheet> CreateFromFile(
        const std::wstring& imagePath,
        UINT frameWidth, UINT frameHeight,
        UINT framesPerRow, UINT totalFrames,
        const Gdiplus::PointF& pivot = { 0.5f, 1.0f },
        bool flipHorizontal = false
    );
    
    ~SpriteSheet();

    // 복사/이동 방지 
    SpriteSheet(const SpriteSheet&) = delete;
    SpriteSheet& operator=(const SpriteSheet&) = delete;
    SpriteSheet(SpriteSheet&&) = delete;
    SpriteSheet& operator=(SpriteSheet&&) = delete;

    // 비트맵 포인터 반환 (첫 번째 스프라이트의 비트맵 반환)
    Gdiplus::Bitmap* GetBitmap() const;
    
    // 스프라이트 목록 반환
    const std::vector<std::shared_ptr<Sprite>>& GetSprites() const { return m_sprites; }

private:
    std::vector<std::shared_ptr<Sprite>> m_sprites;
};
