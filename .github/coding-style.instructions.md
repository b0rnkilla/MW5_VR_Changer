---
applyTo: "**/*.cs"
---

# Coding-Style für C#-Dateien

## Allgemein

- Codebestandteile sind immer Englisch.
- Kommentare und XML-Summaries sind immer Deutsch.
- Keine Emojis oder dekorativen ASCII-Zeichen in Kommentaren.
- Verwende nullable reference types korrekt.
- Vermeide unnötige Abkürzungen.

## XML-Summaries

- Für Klassen und Methoden XML-Dokumentation (`///`) erstellen, wenn sie nicht trivial sind.
- `summary` ist immer Deutsch.
- Wenn möglich und sinnvoll: `<param>` und/oder `<returns>` verwenden.
- Einzeilige Summary so schreiben: `/// <summary> Beschreibung </summary>`
- Mehrzeilige Summary: jede Textzeile mit `<br/>` abschließen.

## Klassenstruktur

Regions sind optional und nur bei größeren Dateien/Klassen sinnvoll.

Wenn Regions verwendet werden, dann in dieser Reihenfolge:

1. Fields
2. Properties
3. Constructor
4. Methods
5. Events

Keine leeren `#region`-Blöcke erstellen.
Keine leeren Konstruktoren erstellen.

Beispiel:

```csharp
#region Fields

private readonly ISettingsService _settingsService;

#endregion

#region Properties

public string StatusMessage { get; private set; } = string.Empty;

public IRelayCommand SaveCommand { get; }

#endregion

#region Constructor

public MainViewModel(ISettingsService settingsService)
{
    _settingsService = settingsService;
}

#endregion

#region Methods & Events

private void Save()
{
}

#endregion

## MVVM Toolkit

* ViewModels erben von ObservableObject.
* Nutze `[ObservableProperty]` für bindbare Properties.
* Nutze `[RelayCommand]` für Commands.
* Command-Methoden enden nicht auf `Command`.

Beispiel:

```csharp
[ObservableProperty]
private string _statusMessage = string.Empty;

[RelayCommand]
private void SaveSettings()
{
}
```

## Services

* Services werden über Interfaces verwendet.
* Services enthalten keine UI-Abhängigkeiten, außer `FileDialogService`.
* Services werfen bei schwerwiegenden Fehlern Exceptions oder liefern klare Result-Objekte.
* ViewModels entscheiden über Statusmeldungen für die Oberfläche.

## Dateinamen

Die drei verwalteten Dateien heißen exakt:

* modlist.json
* Game.ini
* GameUserSettings.ini

Dateinamen nicht mehrfach hart codieren, sondern zentral über Konstanten oder Helper bereitstellen.

