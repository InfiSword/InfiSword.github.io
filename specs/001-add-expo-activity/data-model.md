# Data Model

기존 pfGalleryData 구조 사용: id, title, date, category, desc, tag, src.
EXPO 사진에는 thumbnail(320px 썸네일), original(원본) 경로를 추가한다. src는 최대 1920px 표시용 이미지다.
새 category는 expo, ID는 expo2026-1부터 expo2026-9까지 고유하게 부여.
date는 '행사 기간: 2026.09.30 ~ 2026.10.02'로 개별 촬영일과 구분.
수량 표시는 실제 배열 길이 및 category별 합계에서 계산.
