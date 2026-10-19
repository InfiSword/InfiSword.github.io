# Validation

## Section 3.2 rabbit wall jump example

- Added one initially closed code example in section 3.2, using Rabbit_ability.OnKeyboard from the actual project without modifying game files.
- Source parity check passed after whitespace normalization.
- npm run build: passed. npm test -- --reporter=line: 74 passed (24.8s), including the six disclosures on desktop Chromium and Pixel 5.
- Existing interaction checks include initial closed state, pointer, Enter, Space, visible focus and no page-level horizontal overflow.
- Expanded rabbit example screenshots inspected on desktop and mobile.
- Edits remain uncommitted; no push or deployment performed.

## Files-only rewrite revision

- Rewrote the introduction and all main explanatory paragraphs around shared state rules, behavior reuse and player composition.
- npm run build: passed; npm test -- --reporter=line: 74 passed (28.3s) on desktop Chromium and Pixel 5.
- All five code disclosures, gallery, styles and headings remain unchanged. Existing tests passed for collapsed defaults, pointer/keyboard interactions, contents links, prose length and no horizontal overflow.
- Desktop overview and mobile expanded-code screenshots inspected.
- All revision edits remain uncommitted. No push, deployment or recovery action was performed.

## Earlier revision

- Recovery heartbeat `pages`: PAUSED as requested.
- npm run build: passed.
- npm test -- --reporter=line: 74 passed across desktop Chromium and Pixel 5.
- Existing Slime report tests confirm five initially closed code sections, pointer and keyboard toggles, visible focus, contents anchors, prose length and no page overflow.
- Source comparison: all five code disclosures, gallery, styles and headings are unchanged.
- Desktop closed-state and mobile expanded-state screenshots inspected: readable, no page clipping; code remains independently scrollable.
- First sandboxed browser run could not access the existing preview server and reported an occupied port; rerun with local server access passed.
- No changes to Unity sources, Pages settings or security configuration. No deployment recovery or live deployment verification is part of this rewrite.
