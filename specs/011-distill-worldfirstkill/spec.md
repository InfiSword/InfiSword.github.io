# Feature Specification: Distill WorldFirstKill Report

**Feature Branch**: `main` (unchanged)
**Created**: 2026-10-20
**Status**: Reviewed
**Input**: Keep the simplified WorldFirstKill report; add collapsible token generation and parsing code in section 2.2 and reuse suitable example images from the prior article. Add a visible worked token storage/parsing example and improve first-time interviewer comprehension. Do not commit.

## User Scenarios & Testing

### User Story 1 - Scan the architecture and optionally inspect code (Priority: P1)

A portfolio reader quickly understands how planning data reaches gameplay and how random-generation progress is saved/restored. They inspect only the relevant implementation excerpt.

**Why this priority**: The existing report repeats detailed explanations, large diagrams and unsupported performance claims that obscure the core responsibilities.
**Independent Test**: Read two compact system sections with code collapsed; inspect a flow and its class responsibility map, then open the relevant code.

**Acceptance Scenarios**:

1. The report explains data synchronization/conversion and random-state save/restore through two short flows and explicit class responsibilities.
2. Nine source-backed controls (CSV cache, CSV mapping, Domain enum, DeriveSubSeed, Init, NextInt, BuildSeedToken, RestoreUsageCount and RestoreRNG) start collapsed and support pointer/touch/keyboard. Three other code blocks are visible, including CSV URL structure; section 2.2 presents generation, parsing and 난수 상태 복구 in order, with descriptions and examples always visible.
3. The original six-image gallery remains functional.
4. A local build preview opens visibly for the user without publishing or committing changes.
5. Relevant prior images illustrate the meta sheet, monster data and seed-dependent skill results next to the corresponding explanations.

### Edge Cases

- Preserve the distinction between random-state restoration and saving the entire game world.
- Do not retain unsupported timing, compression or universal determinism guarantees.
- Keep long class names and code from widening mobile pages.
- Preserve unrelated project content and existing gallery controls.

## Requirements

### Current refinement — remove redundant lookup sentence

Remove only the trailing csvURLDict[CSVDataType.Unit]["Monster"] lookup example sentence from the WorldFirstKill CSV structure group. Retain preceding explanation, declaration and visible box. No commit.

### Current refinement — precise concise dictionary wording

Replace informal outer/inner dictionary labels with concise explanation of CSVDataType enum -> item name string -> CSV URL string. Rename the csvURLDict subheading accordingly and remove only the new-expression initializer from its declaration, leaving its actual nested type. Preserve visible box, flow and lookup example. No commit.

### Current refinement — remove CSV mapping example table

Remove only the two-row Unit/Monster and Skill/Active mapping table and its caption from csv-url-structure. Preserve all surrounding explanation, code, lookup example and always-visible box. No commit.

### Current refinement — explicit outer and inner dictionary examples

Clarify the visible csvURLDict prose using actual Skill/Unit enum keys and Monster/Active string keys. Explain that csvURLDict[CSVDataType.Unit] returns an inner Dictionary<string,string> where Monster maps to a CSV URL, with the same pattern for Skill/Active. Preserve declaration, table, lookup, grouping and all other behavior. No commit.

### Current refinement — border the visible master-sheet group

Enclose the always-visible master-csv-download title, explanation, declaration and example in one bordered box matching existing rationale/domain structures. Keep all content and non-collapsible behavior unchanged. No commit.

### Current refinement — show master-sheet structure without disclosure

Replace master-csv-download details/summary with an always-visible section titled 마스터 시트와 CSV 주소 관리 구조. Preserve all existing prose, declaration, example table and lookup expression plus the stable anchor. No new numbered subsection or TOC entry. Nine code disclosures, twelve fences total and three always-visible source blocks remain. No commit.

### Current refinement — reduce download details to the data flow

Remove the three method-by-method download explanations and their core code fences from master-csv-download. Retain the csvURLDict declaration, nested mapping explanation/table and lookup example. Add one short narrative explaining that the master-sheet Links lead to each CSV, which is converted to C# data and used in the game. Keep existing separate cache example. Ten controls, twelve fences total, two always visible. No commit.

### Current refinement — explain nested CSV URL dictionary concisely

Inside master-csv-download, add the actual Dictionary<CSVDataType, Dictionary<string,string>> declaration and a compact example showing data type -> Attribute -> Link, using real enum names and explicitly illustrative URL labels. Explain both dictionary key/value levels and show lookup syntax. Shorten the three existing stage explanations while preserving their essential behavior and code. No new disclosure, commit or game-source change.

### Current refinement — collapsible metadata and CSV download flow

Add one initially closed native code disclosure to section1.1 after the master-sheet figure and before existing cache code. Inside, explain and show three compact stages: fetch MasterSheetURL and register per-row Link by data type/attribute, traverse registered URLs and request only files requiring download, and UnityWebRequest-based async text retrieval. Use actual ServerCSV_ConvertData logic and explain needsDownload through the existing cache control without duplicating its source. Preserve existing code, caption, imagery and other sections. Ten controls, fourteen total fences, two always visible; no commit or game changes.

### Current refinement — master metadata caption and access-code audit

Change the master-sheet caption to comma-separated data type, version and download URL wording; remove the click-original-image instruction and add that each Link provides access to its data sheet. Preserve the original-image link behavior. Check whether current article code includes master-sheet loading and per-row Link access, and report actual source locations without adding unsolicited code. No commit or publication.

### Current refinement — group FieldInfo choice rationale

Enclose the existing three reflection choice/benefit/caution bullets in a visible bordered section titled FieldInfo를 채택한 이유. Preserve exact bullet copy, nearby documentation reference and all code/images. Match the established domain-structures/comparison border, background and responsive padding. No new disclosure or commit.

### Current refinement — concise FieldInfo reference

Replace the detailed FieldInfo explanation and usage example with a short official Microsoft documentation reference. Retain the exact statement that FieldInfo[] fields is an array of field-information objects, always visible in section 1.2. Keep reflection-fieldinfo anchor and other content/code unchanged. Nine disclosures, two always-visible fences and eleven total code fences remain. No commit.

### Current refinement — fold DeriveSubSeed source

Wrap only the existing DeriveSubSeed source fence in an initially closed native disclosure with a method-specific summary. Keep splitmix-rationale and its explanation visible and preserve exact source. Nine controls, three always-visible fences and twelve total source fences remain. No commit or publication.

### Current refinement — visible FieldInfo explanation

Add an always-visible FieldInfo explanation and compact usage example immediately after the reflection introduction in section 1.2. Identify System.Reflection.FieldInfo as a .NET class, explain Name/FieldType and GetValue/SetValue, and connect GetField/GetFields to the FieldInfo[] array. Use the actual public Data_Monster.m_monsterName field as the example, with an official Microsoft reference. Preserve the existing mapping disclosure and all other behavior. Twelve source fences, four always visible and eight disclosures remain; allow 4,000 visible prose characters. No commit or game-source edit.

### Current refinement — helper prose inside disclosure, outside source

Move the detailed Pad/ReadUInt explanation and worked examples from code comments into ordinary prose inside their existing disclosure containers, immediately before the code fences. Retain concise helper separator and useful ReadUInt statement comments. Preserve actual code, external phase explanations, native disclosure behavior and all imagery. No commit or publication.

### Current refinement — explain helpers inside code comments

Add concise Korean comments inside the existing generation/parsing code fences explaining Pad's zero-fill with a real 20-digit seed example and uint/ulong overload roles, and ReadUInt's substring, caller-visible cursor update and numeric conversion with the same seed example. Preserve executable statements, outside prose, collapsed controls and existing semitransparent comment styling. No commit or game-source changes.

### Current refinement — include helpers in their calling code blocks

Append the actual uint/ulong Pad overloads to the generation code fence and the actual ReadUInt implementation to the parsing code fence, inside their existing disclosures. Separate helper definitions from calling statements with a short comment and blank line, and name both functions in each summary. Preserve existing code statements and visible explanations. Eight controls, three visible blocks and eleven source fences remain. The combined parsing block may contain 19 nonblank lines to keep the requested helper alongside its caller. No game edits or commit.

### Current refinement — understand each phase without opening source

Explain the input, transformation and output of the three token phases in visible prose. Generation describes seed/usage inputs, zero-filled fields, missing usage as zero and storing progress rather than generated values. Parsing describes reversing the encoded seed and rebuilding the Domain-to-usage dictionary before the handoff. Restoration explains per-domain sub-seed reconstruction, discarding prior draws and restoring counters for subsequent saves. Preserve code, collapsed controls and existing examples without repeating them. Allow up to 3,700 visible prose characters for this requested explanation; no commit or publication.

### Current refinement — fold the three token-phase code blocks

Rename the third phase to `3. 난수 상태 복구 — RestoreRNG`. Keep all phase explanations, tables and figures visible and unchanged; place only the generation, parsing and restoration code in separate initially closed native disclosures with method-specific summaries. Eight controls and three always-visible source blocks remain (eleven total). Preserve code bytes, anchors, comments and existing spacing. No commit or publication.

### Current refinement — boxed skill comparison and reused seed screen

Enclose the existing skill introduction and both unchanged skill figures in one bordered group matching the domain-structures box. Keep two columns on desktop and one on mobile. Add the existing actual-game seed-input image near the seed-design reference, with a concise caption, original-image link and responsive sizing. Preserve all code, disclosures, gallery and earlier copy. Six inline figures plus the original six-image gallery remain. No commit, publication or game-source edit.

### Functional Requirements

- **FR-001**: Condense the report into data synchronization/conversion and random-state save/restore sections.
- **FR-002**: Provide one four-step flow per system and a compact map of responsible classes.
- **FR-003**: Preserve optional essential code with accurate excerpt labels and source names.
- **FR-004**: Preserve the six-image gallery and public route.
- **FR-005**: Open a visible local preview after successful build and verification.
- **FR-006**: Modify portfolio files only; no commit, push, deployment or game-source changes.
- **FR-007**: Present core BuildSeedToken, RestoreUsageCount and RestoreRNG code. Introduce actual width constants, explain Pad within generation and ReadUInt within parsing, and connect parsed seed/usage to restoration. Remove standalone helper sections/code controls and retain the actual-width read table inside parsing; avoid editorial excerpt/omission notices.
- **FR-008**: Reuse the existing master/monster sheet screenshots and two raw seed-specific skill screenshots with short accurate captions and links to their full-size originals. Do not treat differing skill screenshots as proof of save/restore correctness.

- **FR-009**: In section 2.2, show a concise worked example using seed 42, Map usage 3, Quest usage 5 and zero for the other 12 actual domains. Explain 20/3/3/12 digit fields, the 14-domain count, ID ordering, omitted example rows and separator-free storage, then connect parsing to Skip restoration. Keep existing disclosures and images.

- **FR-010**: Explain unfamiliar terms (reflection/FieldInfo, RNG, domain, Seed, Token, Skip) at first use. Show a real CSV Name -> Data_Monster.m_monsterName example and explain the problem and design choice for each system.
- **FR-011**: Keep eight initially collapsed source controls and three visible source blocks. Keep Init/NextInt and all three token-phase descriptions visible with folded code. Use real function labels without editorial excerpt/omission prose. Label the third token phase 난수 상태 복구.
- **FR-012**: Reuse the four existing sheet/skill images unchanged with captions that explain what to inspect. Add the original prior Minecraft seed-input illustration as an always-visible figure with clear attribution/context; preserve the gallery. No fabricated performance/world-state guarantees.

- **FR-013**: Add three short visible bullets to section 1.2 explaining reflection choice (one parser for different data classes), reuse benefits and tradeoffs (runtime lookup/assignment overhead and name/type mismatches detected during data conversion). Do not imply automatic support for arbitrary types or that all validation is already implemented.

- **FR-014**: Remove the former domain separation reason/benefit paragraph as requested. Preserve the opening domain definitions, structures and concise seed derivation explanation.

- **FR-015**: Render comments inside WorldFirstKill source disclosures in a muted semitransparent gray, including nested highlighted tokens, while keeping code readable and other syntax/report styles intact.

- **FR-016**: Correct the user-provided seed explanation against actual source: BaseSeed is signed long, token encoding preserves its bits as ulong and zero-pads to 20 decimal digits; DeriveSubSeed uses SplitMix64-like mixing then 31-bit int output to seed System.Random; Domain is a uint-backed enum grouping independent RNG streams; usage counts consumed RNG calls as ulong. Avoid SplitMax64, unsigned-long BaseSeed, int-backed Domain or universal full-world restoration claims.
- **FR-017**: Add a clearly hypothetical Map sequence 12 -> 47 -> 8 -> 31 after three consumed values. Compare seed-only reload (next 12) with seed+usage=3 replay (skip three, next 31), alongside the existing token example. Preserve all six source excerpts, figures and comment treatment.

- **FR-018**: Explain seed mixing briefly as spreading small seed/domain input differences, inspired by SplitMix64. Show the actual DeriveSubSeed method without a disclosure. Distinguish its int sub-seed from System.Random output; replace the previous rationale/pros/cons list without claiming adjacent seeds necessarily yield similar sequences or guaranteed collision prevention.

- **FR-019**: Add the complete actual GameSeed.Domain:uint enum immediately after the section 2.1 definition, inside one initially closed native code disclosure. Preserve all 14 names/IDs and the source comment; briefly explain ID ranges and total. Keep the other six excerpts and figures unchanged.

- **FR-020**: Put the requested Domain-ID/usage sentence in the first paragraph of section 2.1 and separate the RNG/domain definition into the following paragraph. Use integer ID wording consistent with Domain:uint rather than signed int. Below the enum, outside its disclosure, show the actual Dictionary<Domain,System.Random> and Dictionary<Domain,ulong> declarations and briefly explain their shared key and distinct values. Preserve all previous source blocks.

- **FR-021**: Keep only the enum inside domain-enum. Show the existing management-structure heading, key/value explanation and dictionary code immediately below as an always-visible normal block; toggling the enum must not hide the structures. Preserve all source text and declarations.

## Success Criteria

- **FR-022**: Visually group the always-visible domain-structures heading, explanation and code inside one bordered block. Preserve copy, source, non-collapsible behavior, enum disclosure and responsive code scrolling.
- **FR-023**: After seed derivation, display Init and NextInt as separate labeled code blocks outside disclosures. Use the actual method declarations and explain derived sub-seeds, System.Random initialization, per-domain usage tracking and int range semantics. Six closed controls, four visible blocks, ten C# blocks total.

### Measurable Outcomes

- **SC-001**: Main report has two numbered chapters and four subsections; visible explanatory text is at most 4,300 characters excluding code and gallery, accommodating the requested always-visible CSV address structure with explicit outer/inner key examples, FieldInfo reference, actual-width ReadUInt example, current-game total-length breakdown and phase input/process/output explanations.
- **SC-002**: Two four-step flows and five class responsibilities can be read without opening code.
- **SC-003**: Nine disclosures, three visible blocks, contents links, six gallery images and six reused reference/example images work on desktop/mobile without page overflow.
- **SC-005**: Keep source examples compact; the parsing block includes ReadUInt alongside its caller, with concise explanatory comments and real-width examples. Readers can understand the key terms and real CSV field mapping without opening code.
- **SC-004**: A visible browser preview displays the newly built local page.

## Assumptions

- Current plain-language request: replace visible Skip terminology with re-drawing/discarding already-used values and continuing with the next value. Rename2.2 to 토큰 저장과 난수 이어 쓰기 and restoration label to 난수 이어 쓰기; align flow/table/related RNG prose, scope and cost with ordinary language. Preserve actual method names/source, examples, constants and behavior. No commit or game edits.

- Current restructuring request: remove standalone Pad/ReadUInt headings, definitions, examples and code controls. Explain Pad once during generation, ReadUInt once during parsing, and connect parsed seed/usage to restoration without suggesting helpers run inside RestoreRNG. Move the actual20/3 ReadUInt table into parsing and keep its explanatory anchor. Preserve the current233-digit calculation, core phase code, images and Init/NextInt folds. Five closed controls, six visible blocks, eleven C# blocks; no commit or game edit.

- Current game-length explanation: state current14 domains produce233 digits, explain seed20 + domain-count3 +14 records of(ID3+usage12)=210, and include zero-usage domains. Replace the formula-only sentence with this concrete breakdown. No commit or implementation changes.

- Current token-length clarification: show the full233-digit length directly below the width declarations, using20 +3 +14*(3+12). Preserve actual constants and avoid adding a new implementation constant. No commit.

- Current request: replace the shortened ReadUInt example with the same real token example used in generation: seed42 encoded in20 digits, domain count14 encoded as014 in3 digits. Table cursors are0->20->23; show the raw23-character prefix and explain domain ID/usage records follow. Replace the generation coverage note with the requested separator explanation, using precise domain-count/domain-ID wording. Preserve source and code folds; no commit.

- Current clarification: explain ReadUInt's purpose (extract a numeric field from a token and advance the shared read position), define idx/width and ref in ordinary language, and replace the terse arrow example with a two-row sequential-read table. Use a clearly shortened example, not the actual20-digit seed format. Keep helper code folded and unchanged. Allow3,250 visible prose characters for the added explanation; no commit or game edits.

- Current spacing refinement: increase only the gap between domain-initialization (ending in NextInt) and the following section2.2 heading. Preserve code/disclosure behavior and other spacing. No commit or publication.

- Latest request: collapse only the Pad, ReadUInt, Init and NextInt code blocks. Keep existing visible headings, prose/examples and outer anchors. Add four native initially closed controls with function-specific summaries. Seven controls, six visible blocks, thirteen C# blocks overall; no content or game-source changes.

- Current request: explain Pad and ReadUInt once in a visible token-helpers group after width constants, using actual helper code and simple numeric/cursor examples. Remove prose that says code was excerpted or setup/validation omitted, keeping functional explanations and table coverage clear. Editorial wording is removed across the article; behavioral comments remain. Three controls, ten visible blocks, thirteen C# blocks total. No commit or game edit.

- Latest 2.2 refinement: introduce the four real width constants in a visible token-format group, distinguish decimal field widths from type size, and connect Pad/ReadUInt to encoding/decoding. Show only core excerpts for generation/parsing/restoration, labeling omitted setup and validation. Preserve examples and figures. Three closed controls and eight visible blocks now make eleven C# blocks.

- Section 2.2 refinement: present token generation, parsing and RNG restoration in three labeled always-visible groups with concise narrative explanation and distinct source blocks. Consolidate repeated encoding/Skip descriptions, preserve the token/usage examples and all original figures, and label omitted parser validation. Current total: three closed controls, seven visible source blocks, ten C# blocks. No game-source edit, commit or publication.

- Latest request: unfold domain-initialization and present Init and NextInt in distinct labeled code blocks. Explain sub-seed derivation, per-domain RNG initialization, usage increments and calling NextInt for an int value. Keep actual seed derivation separate from generated random values. Use actual method declarations for readability. Six disclosures, four visible source blocks, ten C# blocks total.

- Latest ordering request: domain-initialization remains collapsed but moves directly after the visible DeriveSubSeed section. Remove the domain separation reason/benefit paragraph entirely. Preserve code, helper annotations and all other content.

- Latest editorial refinement: replace the domain separation reason/benefit/caution list with one paragraph containing only the reason and benefit. Preserve all code and layout; no commit.

- Latest refinement supersedes FR-023 visibility: domain-initialization stays immediately below the structures box but returns to an initially closed native disclosure. Use the exact phrase +1이나 -1 in the seed paragraph. Seven disclosures, two visible source blocks, nine C# blocks total.

- This is refinement within the existing portfolio identity, not a rebrand.
- Existing colors, typefaces, site shell, gallery and optional-code convention are the visual authority.
- The local WorldFirstKill legacy restore sources provide implementation evidence. Read them without edits.
- Earlier files-only instructions remain active.
