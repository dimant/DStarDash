# DStarDash Improvement Plan

Prioritized work to fix correctness bugs and reduce the ongoing maintenance burden of the HTML scrapers.

## Tracking

| # | Task | Priority | Effort | Status |
|---|------|----------|--------|--------|
| 1 | Fix Xlx heard-users bug — `heardUsers.Add(remoteUser)` never called | Critical | XS | ☑ Done |
| 2 | Thread-safe progress counter in `Parallel.ForEach` (`Interlocked`) | High | XS | ☑ Done |
| 3 | Share a single `HttpClient`, make downloads async, add timeout | High | S | ☑ Done |
| 4 | Add parser tests around `sample-data/` fixtures | High | M | ☑ Done |
| 5 | Move Xlx headers / date formats / allowlist to config (data-driven) | Medium | M | ☐ Not started |
| 6 | Surface download failures instead of swallowing them | Medium | S | ☐ Not started |
| 7 | Sanitize filenames; write cache to a dedicated data directory | Medium | S | ☐ Not started |
| 8 | Require/validate `--type`; drop static mutable state in `Program` | Low | S | ☐ Not started |
| 9 | Flesh out `README.md` and CLI `--help` description | Low | XS | ☐ Not started |
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

### 5. Make Xlx special-casing data-driven (Medium)
`XlxHtmlParser` bakes three hardcoded lists into C#: multilingual column headers (`FindHeardUsersTable`/`ParseColumns`), date formats (`ParseDate`), and a ~25-entry non-dashboard allowlist. Every new broken reflector means a recompile. Move these to a config file so the maintenance loop is edit-data-not-code. Collapse the `ParseDate` try-catch ladder into a `string[]` of formats via `DateTime.TryParseExact`.

### 6. Surface download failures (Medium)
`ReflectorAggregator.cs:45-47` — the empty `catch` makes a failed download indistinguishable from a reflector with no activity; both later show `Status.Fail` with no reason. At minimum log and count failures.

### 7. Filenames and cache location (Medium)
`{Name}.html` uses the scraped reflector name directly as a path (collision / path-safety risk) and lands in the working directory (`bin/Debug/net6.0/`). Sanitize names and write to a dedicated data directory.

### 8. Program entrypoint hygiene (Low)
`Program` uses static mutable `aggregator`/`summarizer` fields, and `--type` has no default — omitting it NREs in `PrintStats`. Build a proper object graph in `Main` and require/validate `--type`.

### 9. Docs (Low)
`README.md` is a single line. Add usage, the two network types, and a CLI description surfaced through `--help`.

### 10. RefListHtmlParser crash on missing Status URL (High) — Done
`RefListHtmlParser.cs` did `new Uri(url ?? "")`, which throws `UriFormatException` on any listing row without a "Status" link, aborting the whole parse. Discovered by the characterization test in #4. Fixed by skipping such rows (`continue`), consistent with `ReflectorAggregator.Reflectors` which already drops empty-URL modules.
