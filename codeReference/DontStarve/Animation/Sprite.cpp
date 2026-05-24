#include "99_Default/pch.h"
#include "Sprite.h"

Sprite::Sprite(std::shared_ptr<Gdiplus::Bitmap> bmp,
	const Gdiplus::RectF& srcRect,
	const Gdiplus::PointF& pvt,
	const std::wstring& k)
	: bitmap(std::move(bmp)), sourceRect(srcRect), pivot(pvt), key(k)
{

}

Sprite::~Sprite()
{

}

std::shared_ptr<Sprite> Sprite::CreateFromFile(const std::wstring& path, const Gdiplus::PointF& pvt)
{
	Gdiplus::Bitmap* pBitmap = Gdiplus::Bitmap::FromFile(path.c_str());
	if (!pBitmap || pBitmap->GetLastStatus() != Gdiplus::Ok) {
		if (pBitmap) delete pBitmap;
		return nullptr;
	}

	std::shared_ptr<Gdiplus::Bitmap> sharedBitmap(pBitmap);
	Gdiplus::RectF srcRect(0.0f, 0.0f, (float)pBitmap->GetWidth(), (float)pBitmap->GetHeight());

	return std::make_shared<Sprite>(sharedBitmap, srcRect, pvt, path);
}
