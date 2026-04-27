---
applyTo: "**/*.cs"
---

# Projektstruktur und Klassenaufteilung

Projektname: MW5_VR-Changer  
Namespace: MW5_VR_Changer

## Ordnerstruktur

Verwende diese Struktur:

- Models
- ViewModels
- Views
- Services
- Services/Interfaces
- Helpers
- Enums

## Models

Erstelle einfache Datenklassen ohne UI-Logik.

Geplante Models:

- BackupStatus
- FilePathSet

## Enums

Geplante Enums:

- VrMode

Werte:

- VR
- NonVR

## Services

Dateisystem- und JSON-Logik gehört nicht direkt ins ViewModel.

Geplante Interfaces:

- IAppEnvironmentService
- IBackupService
- IFileDialogService
- IFileSwitchService
- IOriginalFileDiscoveryService

Geplante Implementierungen:

- AppEnvironmentService
- BackupService
- FileDialogService
- FileSwitchService
- OriginalFileDiscoveryService

## ViewModels

MainViewModel ist das zentrale ViewModel für die Hauptansicht.

Es verwaltet:

- ausgewählte Original-Dateipfade
- aktuellen VR-Modus
- Backup-Status
- Statusmeldungen
- Command-Verfügbarkeit

Es führt keine direkten File.Copy-, Directory.CreateDirectory- oder JSON-Operationen aus.

## Views

MainWindow.xaml bindet ausschließlich an MainViewModel.

Keine fachliche Logik in MainWindow.xaml.cs.

## Dependency Injection

Verwende Microsoft.Extensions.DependencyInjection.

Registriere Services und ViewModels zentral beim App-Start in App.xaml.cs.

## Pfadregeln

Das App-Root ist das Verzeichnis der ausführbaren Datei.

Der Arbeitsordner heißt:

MW5VRC

Darin liegen:

- VR/modlist.json
- VR/Game.ini
- VR/GameUserSettings.ini
- NonVR/modlist.json
- NonVR/Game.ini
- NonVR/GameUserSettings.ini