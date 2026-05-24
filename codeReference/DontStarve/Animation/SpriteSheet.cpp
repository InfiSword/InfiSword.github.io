#include "99_Default/pch.h"
#include "SpriteSheet.h"
#include "Sprite.h"

SpriteSheet::SpriteSheet(std::vector<std::shared_ptr<Sprite>> sprites)
    : m_sprites(std::move(sprites)) {
}

std::shared_ptr<SpriteSheet> SpriteSheet::CreateFromFile(
    const std::wstring& imagePath,
    UINT frameWidth, UINT frameHeight,
    UINT framesPerRow, UINT totalFrames,
    const Gdiplus::PointF& pivot,
    bool flipHorizontal) {
    
    Gdiplus::Bitmap* bitmap = new Gdiplus::Bitmap(imagePath.c_str());
    if (!bitmap || bitmap->GetLastStatus() != Gdiplus::Ok) {
        if (bitmap) delete bitmap;
        OutputDebugStringW((L"SpriteSheet: 파일 로드 실패 - " + imagePath + L"\n").c_str());
        return nullptr;
    }

    if (flipHorizontal) {
        UINT w = bitmap->GetWidth();
        UINT h = bitmap->GetHeight();
        Gdiplus::Bitmap* flipped = new Gdiplus::Bitmap(static_cast<INT>(w), static_cast<INT>(h));
        if (flipped && flipped->GetLastStatus() == Gdiplus::Ok) {
            Gdiplus::Graphics g(flipped);
            Gdiplus::RectF destRect(0.0f, 0.0f, static_cast<float>(w), static_cast<float>(h));
            g.DrawImage(bitmap, destRect, static_cast<float>(w), 0.0f, -static_cast<float>(w), static_cast<float>(h), Gdiplus::UnitPixel);
            delete bitmap;
            bitmap = flipped;
        } else {
            if (flipped) {
                delete flipped;
            }
        }
    }

    std::shared_ptr<Gdiplus::Bitmap> sharedBitmap(bitmap);

    // 이미지 실제 크기 정보
    UINT sheetWidth = sharedBitmap->GetWidth();
    UINT sheetHeight = sharedBitmap->GetHeight();
    
    // 최종 프레임 크기 (자동 계산 또는 전달받은 값 사용)
    UINT finalFrameWidth = frameWidth;
    UINT finalFrameHeight = frameHeight;

    if ((finalFrameWidth == 0 || finalFrameHeight == 0) && 
        framesPerRow > 0 && totalFrames > 0 && 
        sheetWidth > 0 && sheetHeight > 0) 
    {
        UINT totalRows = (totalFrames + framesPerRow - 1) / framesPerRow;
        if (finalFrameWidth == 0) finalFrameWidth = sheetWidth / framesPerRow;
        if (finalFrameHeight == 0) finalFrameHeight = sheetHeight / totalRows;
    }

    if (finalFrameWidth == 0 || finalFrameHeight == 0) {
        return nullptr;
    }

    std::vector<std::shared_ptr<Sprite>> sprites;
    for (UINT i = 0; i < totalFrames; ++i) {
        UINT row = i / framesPerRow;
        UINT col = i % framesPerRow;

        UINT colForX = flipHorizontal ? (framesPerRow - 1 - col) : col;
        float x = (float)(colForX * finalFrameWidth);
        float y = (float)(row * finalFrameHeight);

        Gdiplus::RectF sourceRect(x, y, (float)finalFrameWidth, (float)finalFrameHeight);
        auto sprite = std::make_shared<Sprite>(sharedBitmap, sourceRect, pivot, imagePath);
        sprites.push_back(sprite);
    }

    return std::make_shared<SpriteSheet>(std::move(sprites));
}

SpriteSheet::~SpriteSheet() {
    m_sprites.clear();
}

Gdiplus::Bitmap* SpriteSheet::GetBitmap() const {
    if (m_sprites.empty()) return nullptr;
    return m_sprites[0]->bitmap.get();
}
