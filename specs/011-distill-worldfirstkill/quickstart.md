# Validation Guide

- Spacing refinement: check a72px gap from the NextInt control to the following2.2 heading in closed/open states on desktop and Pixel5.

1. Run npm run build and npm test with PLAYWRIGHT_BROWSERS_PATH=C:/Users/seif4/AppData/Local/ms-playwright. Preserve screenshot artifacts by assigning a separate test output directory.
2. On desktop and Pixel5, check two chapters/four subsections, two four-step flows and five class rows. Read explanations without opening code.
3. Verify nine initially closed controls via pointer/touch, Enter/Space and focus, including DeriveSubSeed with its explanation outside details. Confirm three visible source blocks and twelve C# blocks total, translucent comments and no horizontal page overflow at375/768/1024/1440px.
4. Compare complete Domain/DeriveSubSeed/Init/NextInt sources and the four width declarations to GameSeed/SaveLoadManager. Compare token core logic to source, excluding preparation/validation outside the displayed sections. Verify standalone Pad/ReadUInt sections and code controls are absent, phase-specific helper explanations are visible, Init/NextInt stay folded, and editorial excerpt/omission prose remains absent. No game edits.
5. Check token-generation -> token-parsing -> rng-restoration order, initially collapsed source and visible explanations/tables. Verify the third phase label 난수 상태 복구, the 20/3/3/12 width constants, long/ulong types, Pad in generation, ReadUInt in parsing with cursor0->20->23 and two actual-width20/3 table rows matching seed42/count14 in generation, plain-language draw/discard/resume explanation in restoration, fixed-width42/14/Map3/Quest5 example and the hypothetical usage3 -> next31 example. Scope/cost appears once in ordinary language. Verify no visible Skip and that the updated2.2 contents link scrolls to 토큰 저장과 난수 이어 쓰기.
6. Verify six original figures load, full-size links work and six-image gallery navigation remains functional. Check the actual-game seed screen and skill comparison box: matching structure-box border/padding, two columns on desktop and one on mobile. Different-seed skill images must not claim save/restore proof.
7. Inspect a batched desktop/mobile screenshot set; run scoped Impeccable/unslop scans once per refinement. Fix findings in one batch and confirm once.
8. Refresh and retain the visible local preview. Keep changes uncommitted; do not push or publish.

9. Verify the actual Pad(uint/ulong) definitions share the BuildSeedToken fence, and actual ReadUInt shares the RestoreUsageCount fence. Existing caller statements stay unchanged. Both helpers appear only when their phase's existing disclosure is opened; summaries include helper names.

10. Check helper prose inside each disclosure body, above and outside its pre block: Pad(42UL,20) yields the 20-digit zero-filled seed; ReadUInt reads seed/count as 42/14 while idx advances 0->20->23, with ref updating the caller's cursor. Explanation and code appear together when opened. Keep only concise functional comments in source; statements match actual source ignoring comments, and highlighted comments retain rgba(212,212,212,0.7).

11. Section1.2: FieldInfo[] fields array explanation and official Microsoft FieldInfo reference stay visible, while the former detailed class explanation and usage example are absent. Preserve reflection-fieldinfo anchor and nearby CSV mapping disclosure.

12. Check the visible FieldInfo를 채택한 이유 section: three original bullets unchanged, no disclosure, same background/border/padding as domain-structures on desktop/mobile, with no list overflow.

13. Master-sheet caption uses commas between data type/version/download URL, explains per-row Link access and omits the click-original instruction. Original image link stays functional. Verify master-csv-download is always visible, titled 마스터 시트와 CSV 주소 관리 구조, with no details/summary around its concise Link -> CSV -> C# game-data narrative and dictionary example. Preserve existing cache code.

14. Within that visible group, verify csvURLDict type/field declaration with no new initializer and concise CSVDataType -> item name -> URL explanation. Redundant trailing lookup sentence, informal outer/inner/2-stage dictionary labels and former mapping table/caption are removed. No disclosure.

15. Master-sheet structure title, prose, declaration and table/lookup are contained in one bordered box matching domain-structures, with24px desktop/16px mobile padding and no overflow. Content remains always visible.
