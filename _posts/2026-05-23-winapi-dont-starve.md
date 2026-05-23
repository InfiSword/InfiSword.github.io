---
layout: single
title: "PROJECT REPORT // WINAPI_DONT_STARVE"
excerpt: "C++, WinAPI를 이용한 Don't Starve 모사 및 성능 최적화"
categories: [project]
permalink: /project/winapi-dont-starve/
tags: [C++, WinAPI, Optimization, Game Logic]
toc: true
toc_sticky: true
---

C++와 WinAPI를 활용하여 'Don't Starve'의 핵심 메커니즘을 모사하고, 대규모 객체 환경에서의 성능 최적화를 연구한 프로젝트입니다.

## 1. 프로젝트 개요
- **개발 기간**: 2025. 12. 05 ~ 2026. 02. 04 (약 2개월)
- **개발 환경**: C++, WinAPI
- **목표**: 대규모 오브젝트가 존재하는 환경에서 안정적인 프레임을 유지하며, 생존 및 전투 루프가 완전한 게임 구현.

## 2. 주요 시스템 설계

### 2.1 컴포넌트 기반 객체 설계 (Component-Based)
단일 상속의 복잡성을 피하고 확장성을 높이기 위해, 기능을 컴포넌트 단위로 분리하여 설계했습니다.
- **Transform**: 위치, 크기, 그리드 좌표 관리
- **Renderer**: 스프라이트 및 애니메이션 출력
- **Collider**: 원형/사각형 충돌 판정

### 2.2 중앙 집중식 관리 객체 (Managers)
- **ObjectManager**: 모든 게임 객체의 생성, 소멸 및 그리드 업데이트 관리.
- **CameraManager**: 플레이어 추적 및 현재 화면에 보이는 영역(Viewfrustum) 계산.
- **ResourceManager**: 비트맵 자원의 중복 로딩 방지 및 캐싱.

## 3. 핵심 기술: 그리드 기반 컬링 (Grid-based Culling)

### 3.1 문제 상황
월드에 1,000개에서 10,000개 사이의 나무, 돌, 몬스터 등 오브젝트가 생성되면서 매 프레임 모든 객체를 업데이트하고 렌더링할 때 심각한 프레임 드랍이 발생했습니다.

### 3.2 해결 방안: 공간 분할 및 컬링
1. **공간 분할**: 전체 월드를 일정 크기의 그리드(Grid)로 분할하고, 각 객체가 속한 그리드 번호를 실시간으로 갱신하게 설계했습니다.
2. **범위 쿼리**: `ObjectManager::QueryObjectsInRectArea` 메서드를 구현하여, 현재 카메라가 비추는 Rect 영역에 해당하는 그리드 내 객체들만 리스트로 추출합니다.
3. **렌더링 최적화**: 추출된 객체들만 렌더링 파이프라인에 전달함으로써 불필요한 연산을 O(N)에서 O(M) (M은 화면 내 객체 수)으로 줄였습니다.

## 4. 게임 루프 및 콘텐츠
- **생존 시스템**: 시간 경과에 따른 낮/밤 변화 시스템과 플레이어의 허기(Hunger), 정신력(Sanity), 체력(Health) 수치 연동.
- **상태 기반 AI**: 몬스터의 FSM(Idle, Trace, Attack, RunAway)을 통해 지능적인 행동 패턴 구현.
- **최종 보스전**: 특정 조건을 만족할 때 등장하는 보스 몬스터와의 전투 패턴 구현.
