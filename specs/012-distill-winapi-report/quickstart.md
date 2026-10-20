# Local validation

1. npm run build
2. PLAYWRIGHT_BROWSERS_PATH=C:/Users/seif4/AppData/Local/ms-playwright를 사용해 npm test -- --reporter=line --output=test-results/ds-distill-regression 실행.
3. http://127.0.0.1:4173/project/winapi-dont-starve/ 로컬 페이지에서 6개 코드 기본 접힘, 데스크톱 클릭/모바일 탭, Enter, Space를 각각 열고 닫아 확인.
4. 375/768/1024/1440px에서 가로 넘침, 이미지 정상 로드, 원본 링크, 갤러리 8장 및 이전·다음/썸네일 확인.
5. 데스크톱·모바일 스크린샷과 JSON을 test-results/ds-distill/에 남기고 현재 브라우저 탭을 새로고침한다.
6. git diff --check, 기존 수정 보존 확인. 커밋·배포 없음.
