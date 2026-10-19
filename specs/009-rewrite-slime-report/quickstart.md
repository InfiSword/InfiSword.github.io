# Validation Guide

1. Run npm run build from the repository root.
2. Set PLAYWRIGHT_BROWSERS_PATH to C:/Users/seif4/AppData/Local/ms-playwright and run npm test.
3. Expect all desktop/mobile tests to pass, including the Slime report's six disclosures, contents links and overflow checks.
4. Inspect the generated report screenshots and diff. Confirm the gallery, six code blocks, chapter 1/2 anchors and styles are preserved; chapter 3 and transformation code are absent; the section 2.4 comparison and contents link work; both adjacent code examples can stay open together.
5. Confirm recovery automation pages is PAUSED. No remote deployment success is claimed by this editorial validation.
