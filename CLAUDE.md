# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A .NET 6 console app that scrapes D-STAR ham radio *reflector* dashboards (web pages hosted by individual reflector operators), aggregates "heard user" activity across them, and prints a sortable stats table to the terminal. It supports two distinct reflector networks — `Ref` (dstarinfo.com) and `Xlx` (oe1phs.ddns.net) — each with its own listing page format and per-reflector dashboard format.

## Commands

```bash
dotnet build                                       # build
dotnet run -- --type Xlx                           # run against locally-cached HTML (no download)
dotnet run -- --type Ref --download                # download fresh HTML, then print stats
dotnet run -- --type Xlx --sortby Heard --top 10   # sort + limit output
```

CLI flags come from `Program.Main`'s parameters via **System.CommandLine.DragonFruit** — adding a parameter to `Main` creates a new flag automatically. Flags: `--type` (`Ref`|`Xlx`), `--download` (bool), `--sortby` (`Name`|`Status`|`Heard`|`Last`), `--top` (int). `--type` is effectively required — the `switch` in `Main` has no default, so omitting it leaves the aggregator/summarizer null and `PrintStats` throws.

There are no tests. Verification is manual: run against the checked-in `sample-data/` fixtures.

## Data flow

1. **Download** (`ReflectorAggregator.DownloadReflectorData`, only with `--download`): fetches the network's *listing* page to `{type}-reflectors.html`, parses it into `ReflectorModule`s, then downloads each reflector's dashboard **in parallel** (`Parallel.ForEach`) to `{Name}.html` in the working directory. Individual download failures are swallowed.
2. **Aggregate** (`ReflectorAggregator.Reflectors`): groups `ReflectorModule`s by their dashboard `Url` — one reflector can expose multiple modules, so the key is the URL and the value is a `List<ReflectorModule>`.
3. **Summarize** (`Summarizer.Summarize`): for each reflector, reads the cached `{Name}.html`, parses it into a `Reflector` (name + `HeardUsers`), and computes a `StatsRow`: heard-user count, most-recent `LastHeard`, and busiest module. A missing `{Name}.html` yields `Status.Fail`.
4. **Print** (`Program.PrintStats`): sorts, applies `--top`, renders with **ConsoleTables**.

All downloaded/cached HTML files land in the **current working directory** (e.g. `bin/Debug/net6.0/` under `dotnet run`), not a dedicated data dir.

## Parser architecture

Two parser families, each with an interface, a shared base, and per-network implementations. The `Ref`/`Xlx` pair is selected together in `Program.Main`.

- **Listing parsers** (`IReflectorListHtmlParser` → `ReflectorListHtmlParser` base → `RefListHtmlParser`, `XlxListHtmlParser`): parse a network's index page into `ReflectorModule`s. Override `Parse(HtmlDocument, uri)`; the base provides `ParseFromFile`/`ParseFromUrl`.
- **Dashboard parsers** (`IReflectorHtmlParser` → `ReflectorHtmlParser` base → `RefHtmlParser`, `XlxHtmlParser`): parse one reflector's dashboard into a `Reflector` with its `HeardUsers`.

Parsing uses **HtmlAgilityPack** XPath against real-world, inconsistent operator-authored HTML. The `Xlx` dashboards are the hard case: `XlxHtmlParser` must locate the "heard users" table and its columns across **many languages and layouts** — hence the long chains of `th` text matches in `FindHeardUsersTable`/`ParseColumns` (Callsign/Rufzeichen/Nominativo/…) and the fallback list of date formats in `ParseDate`. When a page can't be parsed, it throws *unless* the page matches one of a hardcoded allowlist of known-non-dashboard pages (nginx defaults, redirects, specific operator sites). Extending Xlx support usually means adding a new column-header string, date format, or allowlist entry here rather than restructuring.

## Sample data

`sample-data/*.html` (ref/xlx listing + reflector fixtures) is copied to the output directory on build (`CopyToOutputDirectory` in the `.csproj`). Use these to exercise parsers without hitting the network.
