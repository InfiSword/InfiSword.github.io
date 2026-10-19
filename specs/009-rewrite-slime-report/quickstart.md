# Validation Guide

1. Run npm run build from the repository root.
2. Set PLAYWRIGHT_BROWSERS_PATH to C:/Users/seif4/AppData/Local/ms-playwright and run npm test.
3. Expect all desktop/mobile tests to pass, including the Slime report's five disclosures, contents links and overflow checks.
4. Inspect the generated report screenshots and diff. Confirm gallery, code blocks, anchors and styles are preserved.
5. Confirm recovery automation pages is PAUSED. No remote deployment success is claimed by this editorial validation.
