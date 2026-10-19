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
- New BuildSeedToken and RestoreUsageCount excerpts preserve the actual methods after removing comments and blank lines. Pad zero-fills values and ReadUInt advances a fixed-width read position; these helper dependencies are described beside the optional code.
