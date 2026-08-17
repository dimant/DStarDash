# DStarDash Improvement Plan

Prioritized work to fix correctness bugs and reduce the ongoing maintenance burden of the HTML scrapers.

## Tracking

| # | Task | Priority | Effort | Status |
|---|------|----------|--------|--------|
| 1 | Fix Xlx heard-users bug — `heardUsers.Add(remoteUser)` never called | Critical | XS | ☑ Done |
| 2 | Thread-safe progress counter in `Parallel.ForEach` (`Interlocked`) | High | XS | ☑ Done |
| 3 | Share a single `HttpClient`, make downloads async, add timeout | High | S | ☑ Done |
| 4 | Add parser tests around `sample-data/` fixtures | High | M | ☑ Done |
| 5 | Move Xlx headers / date formats / allowlist to config (data-driven) | Medium | M | ☑ Done |
| 6 | Surface download failures instead of swallowing them | Medium | S | ☑ Done |
| 7 | Sanitize filenames; write cache to a dedicated data directory | Medium | S | ◐ Partial (sanitize done; data dir pending) |
| 8 | Drop static mutable state in `Program` (note: `--type` already defaults to `Ref`) | Low | S | ☑ Done |
| 9 | Flesh out `README.md` and CLI `--help` description | Low | XS | ☑ Done |
| 10 | Fix `RefListHtmlParser` crash on rows without a Status URL (skip them) | High | XS | ☑ Done |

Status legend: ☐ Not started · ◐ In progress · ☑ Done

---

## Details

### 1. Fix the Xlx heard-users bug (Critical)
`Parsers/XlxHtmlParser.cs:45-64` — the loop builds each `remoteUser` and sets its fields but never adds it to `heardUsers`. Every Xlx reflector therefore reports 0 heard users, an empty busiest module, and `DateTime.MinValue` for last-heard. Since `--type Xlx` is the default in `launchSettings.json`, the primary use case is silently broken. Add `heardUsers.Add(remoteUser)` and confirm with fixtures.

### 2. Thread-safe progress counter (High) — Done
`ReflectorAggregator.cs` — `int i` was mutated inside `Parallel.ForEach` without synchronization, and `progress(i, n)` read it before the increment. Now uses `Interlocked.Increment` and reports the post-increment value (monotonic 1..n).

### 3. HTTP layer (High) — Done
`HttpDownloader.cs` newed up a fresh `HttpClient` per file and blocked on `.Result`. Now uses a single shared static `HttpClient` with a 30s timeout, a genuinely async path, and `EnsureSuccessStatusCode()` so error responses throw (and don't get written as bogus HTML files) instead of silently corrupting the cache. The client is injectable via an `internal` constructor for tests (covered in `HttpDownloaderTests`).

### 4. Parser tests (High)
`sample-data/` already holds ref/xlx listing + reflector HTML, copied to output on build — a ready-made, offline test corpus. Add a test project covering `RefListHtmlParser`, `XlxListHtmlParser`, `RefHtmlParser`, and `XlxHtmlParser`. This would have caught #1 immediately and locks in the parser behavior before refactoring #5.

### 5. Make Xlx special-casing data-driven (Medium) — Done
`XlxHtmlParser` baked three hardcoded lists into C#: column headers, date formats, and the ~25-entry non-dashboard allowlist. Extracted all of them into `Parsers/XlxParsingConfig.cs` (`CallsignHeaders`, `LastHeardHeaders`, `ModuleHeaders`, `DateFormats`, `NonDashboardMarkers`), overridable at runtime from an optional `xlx-parsing.json` via `XlxParsingConfig.Load` — edit data, not code. The 11-deep `ParseDate` try/catch ladder is now a single `DateTime.TryParseExact` over `DateFormats`. `ParseColumns` and the allowlist check consume the config. Covered by `XlxParsingConfigTests` (date formats, JSON override) and protected end-to-end by the existing `XlxHtmlParserTests` against the `XLX801` fixture.
**Note:** the table-location XPath search in `FindHeardUsersTable` (multilingual `th` matching, with a structural quirk for the `MyCall` layout) was left as-is — I have only one Xlx fixture (English) and can't verify the other-language paths, so changing that heuristic blindly would be unsafe.

### 6. Surface download failures (Medium) — Done
The empty `catch` made a failed download indistinguishable from a reflector with no activity; both later showed `Status.Fail` with no reason. Extracted a testable `ReflectorAggregator.DownloadReflectors(...)` that collects the names of reflectors whose download threw (via a `ConcurrentBag`) and returns them; `DownloadReflectorData` now prints a summary of failures to stderr. Introduced an `IFileDownloader` seam (implemented by `HttpDownloader`) so this is unit-tested with a fake in `ReflectorAggregatorTests` — no network needed.

### 7. Filenames and cache location (Medium) — Partial
Sanitization done: `{Name}.html` used the scraped reflector name directly as a path (collision / traversal / invalid-char risk). Added `ReflectorFile.NameFor` which replaces `Path.GetInvalidFileNameChars()` with `_`; both the download (`ReflectorAggregator`) and read (`Summarizer`) sides now go through it so they always agree. Covered by `ReflectorFileTests`.
**Still pending:** writing the cache to a dedicated data directory instead of the working dir (`bin/Debug/net6.0/`) — deferred because it also touches the listing-file paths constructed in `Program`.

### 8. Program entrypoint hygiene (Low) — Done
Removed the static mutable `aggregator`/`summarizer` fields. `Main` now builds the object graph via `Build(type)` and threads the instances through `PrintStats`. The `--sortby` logic is extracted into `Program.Sort(list, sortby)` and unit-tested in `ProgramSortTests`. (Correction to an earlier note: `--type` is **not** a crash risk — `ReflectorType` defaults to `Ref`, so the `switch` always binds; there is simply no `default` case.)

### 9. Docs (Low) — Done
Replaced the one-line `README.md` with build/usage instructions, an options table, a description of the two network types, the table columns, and how to override XLX parsing via `xlx-parsing.json` (from #5).

### 10. RefListHtmlParser crash on missing Status URL (High) — Done
`RefListHtmlParser.cs` did `new Uri(url ?? "")`, which throws `UriFormatException` on any listing row without a "Status" link, aborting the whole parse. Discovered by the characterization test in #4. Fixed by skipping such rows (`continue`), consistent with `ReflectorAggregator.Reflectors` which already drops empty-URL modules.
