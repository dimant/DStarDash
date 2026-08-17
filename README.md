# DStarDash

A .NET console tool that scrapes D-STAR ham radio **reflector** dashboards, aggregates recent "heard user" activity across a whole network, and prints a sortable stats table to the terminal.

It supports two reflector networks, each with its own page formats:

- `Ref` — classic D-STAR reflectors listed on dstarinfo.com
- `Xlx` — XLX multi-protocol reflectors

## Build

```bash
dotnet build
```

## Usage

```bash
# Print stats from the locally-cached HTML (no network)
dotnet run -- --type Xlx

# Download fresh dashboards, then print
dotnet run -- --type Ref --download

# Sort and limit
dotnet run -- --type Xlx --sortby Heard --top 10
```

### Options

| Flag | Values | Meaning |
|------|--------|---------|
| `--type` | `Ref` \| `Xlx` | Which reflector network to query (default `Ref`) |
| `--download` | (flag) | Fetch fresh HTML before printing; otherwise uses cached files |
| `--sortby` | `Name` \| `Status` \| `Heard` \| `Last` | Sort order (default: by name) |
| `--top` | integer | Show only the first N reflectors |

The table columns are: name, location, status, heard-user count, last-heard time (local), and busiest module.

## Configuration

XLX dashboards are authored by hundreds of operators worldwide in many languages and layouts. The column-header names, accepted date formats, and the list of markers identifying non-dashboard pages live in `Parsers/XlxParsingConfig.cs` as defaults. To adapt to a new reflector variant **without recompiling**, drop an `xlx-parsing.json` file next to the executable overriding any of these arrays, e.g.:

```json
{ "DateFormats": ["yyyy/MM/dd HH:mm", "dd.MM.yyyy HH:mm"] }
```

## Notes

- The project targets **net6.0** (end-of-life). Building needs a compatible SDK; running needs the .NET 6 runtime installed.
- Downloaded HTML is cached in the working directory.
