# Validation

## Section 3.4 A* request comparison

- Added two initially closed, independent disclosures for CustomerAI.MoveToPosition and Staff_AI.MoveToPosition. Both use the shared FindPath call; customerMovement receives the route, while staff movement receives the route plus isCook.
- Both functions match actual game sources after removing debug logging/comments and normalizing whitespace. Path validity checks and failure resets are preserved. Game sources were read only.
- Existing prose, gallery and four earlier source examples are unchanged.
- npm run build: passed. Existing npm test suite: 74 passed (26.7s).
- Local desktop Chromium and Pixel 5 checks passed for all six initially closed disclosures, click/tap, Enter and Space, valid contents links and no page overflow. The customer/staff pair can remain open together.
- Expanded customer and staff screenshots captured on both devices and inspected. A first one-off check selected inline isCook text along with the code block; narrowing the locator to pre code resolved that inspection-only issue.
- Edits remain uncommitted; no push or deployment.

## Initial consolidation

- Removed chapters 4/5 and their diagrams, state lists and Staff_AI serving example. Added section 3.4 with two short paragraphs and a two-row customer/staff comparison (324 source characters).
- Corrected the nearby A* diagram to describe lowest-FCost candidate selection, consistent with its displayed List-based implementation.
- Source comparison confirms chapters 1/2, the seven-image gallery and four retained code disclosures are unchanged.
- npm run build: passed. Existing npm test suite: 74 passed (28.4s), including desktop/mobile MoonLight gallery coverage.
- Local desktop Chromium and Pixel 5 review confirmed exactly three numbered chapters, no obsolete/broken contents links, working section 3.4 navigation and the two-row relationship table.
- All four retained examples passed closed defaults, click/tap, Enter and Space checks. No page-level horizontal overflow with the table or code open.
- Desktop summary and mobile table screenshots inspected.
- Files remain uncommitted. No push, deployment, security changes or game-source edits.
