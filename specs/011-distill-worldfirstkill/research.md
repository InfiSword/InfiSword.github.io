# Evidence and Decisions

- Read the current report and inspected desktop/mobile incumbent captures. Its nested diagrams, long subheadings and repeated explanations obscure two core systems.
- Actual source root: C:/MyProject/WorldFirstKillChallenge-NewAge/Assets/WFKCLegacyRestore/Runtime/Legacy/.
- ServerCSV_ConvertData compares local/server versions, reads a matching local file and falls back to download when missing, changed or unreadable.
- CSVParser.SetupFieldDict matches normalized headers to fields; parseCSV splits rows, converts values and registers typed objects. The report's older ordinal-field example is superseded by the current source-backed name mapping.
- GameSeed uses long BaseSeed, per-domain RNG/count dictionaries, a SplitMix64-like derived seed, Init, NextInt and RestoreRNG. The old report's RestoreFromSnapshot name does not match current source.
- SaveLoadManager builds a fixed-width token from the base seed and usage snapshot, reads it through RestoreUsageCount and calls RestoreRNG. The optional source excerpt preserves the actual 20/3/3/12 width constants; visible prose stays concise.
- Keep correctness scope to random-generation progress under the same data/generation logic. Remove universal world restoration claims and unmeasured nanosecond/zero-loading/compression figures.
- UI UX Pro Max supported progressive disclosure, readable mobile text and visible keyboard focus. Its generic new palette/font/motion suggestions do not override the existing DESIGN.md.
- Impeccable context permits scoped refinement without PRODUCT.md. No rebrand or unrelated product-context work is needed. Unslop guidance favors purposeful reading structure over card stacks and decoration.

- Reused the prior master meta/monster sheet images and inspected the two raw skill screenshots. Their captions identify different seeds and skill results. Omitted the old comparison slide because its embedded claims exceed what the screenshots establish.
- BuildSeedToken and RestoreUsageCount excerpts preserve the actual core source statements, with omitted surrounding setup and validation explicitly described. Pad zero-fills values and ReadUInt advances a fixed-width read position; these helper dependencies are described beside the optional code.

- Interviewer pass: verified CSVParser.GetFields, NormalizeField and SetFieldValue, plus Data_Monster.m_monsterName. The real alias rule strips m_ and maps monstername to name, which supports the visible Name-column example. SetFieldValue handles string/int/float through FieldType and SetValue.
- Dense historical slide 11/14 materials contain broad claims and duplicate the skill screenshots. Keep the useful raw sheet/skill images, and reuse only the prior extracted Minecraft seed-input image as a collapsed design-inspiration reference. Image bytes remain unchanged.
- Extracted 13/15/9/11/15/10-line core fragments. Compared each substantive statement to actual game sources; no game source edits. This improves code scanning while surrounding prose makes missing context explicit.

- Reflection rationale: the actual parser obtains fields from generic T, maps normalized names, and calls GetField/SetValue at runtime. This supports the shared-parser benefit and the stated lookup/assignment tradeoff. Name/type mismatches require conversion-time checking; the article does not claim exhaustive validation or measured performance.

- Domain rationale: GameSeed keeps a separate System.Random and usage counter per Domain, with DeriveSubSeed(baseSeed, domain ID). This isolates random-call progress across domains; the same-domain call sequence and stable IDs/derivation remain necessary for replay. The added prose explains management cost without claiming statistical independence or automatic compatibility across rule changes.

- Corrected user seed explanation: BaseSeed/Init use long; BuildSeedToken uses unchecked((ulong)baseSeed), and RestoreUsageCount reverses it with unchecked((long)baseSeedEncoded). Domain : uint has 14 values; _domainUsageCounts stores ulong. DeriveSubSeed performs 64-bit xor/shift/multiply mixing and masks to 0x7FFFFFFF before System.Random construction. It derives seeds; it does not directly emit the game's RNG sequence.
- Primary references: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types (signed long vs unsigned ulong and uint); https://prng.di.unimi.it/splitmix64.c (original Vigna 64-bit mix constants/steps); https://learn.microsoft.com/en-us/dotnet/api/system.random?view=net-10.0 (seeded sequence behavior and runtime-version caveats). Local mix uses a domain salt and 31-bit reduction, so label it SplitMix64-like rather than the complete original generator.
- The Map sequence 12,47,8,31 is explicitly hypothetical, not measured from seed 42. After consuming three values, replay must skip three before the expected next 31; reinitialization from seed alone starts at 12. The prose scopes restoration to RNG progress under matching data/logic/implementation rather than all mutable game state.

- SplitMix64-like rationale follows the actual stateless DeriveSubSeed function: xor/shift/multiply mixing diffuses input differences before the 31-bit mask; deterministic same-input derivation needs no separately saved mixing state. The mask permits collisions and changing the mix alters replay for existing inputs. Added concise purpose/benefit/tradeoff prose without claiming measured speed, collision freedom, cryptographic suitability or complete independence.
