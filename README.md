<p align="center">
  <img src="Assets/AppLogo.png" alt="MW5 VR Changer" width="160" />
</p>

<h1 align="center">MW5 VR Changer</h1>

App zum schnellen Umschalten zwischen **VR** und **NonVR** Konfigurationen für *MechWarrior 5: Mercenaries*.

Im Mittelpunkt steht der **Toggle-Switch**: Je nach Stellung werden die gesicherten Dateien aus dem passenden Backup-Ordner in die Originalpfade kopiert.

## Features

- Automatische Erkennung der benötigten Originaldateien
  - Steam-Installations-/Library-Suche (inkl. zusätzlicher Libraries)
  - AppData-Config Pfad-Suche (primär aktueller Benutzer, Fallback über weitere Benutzerprofile)
- Manuelle Auswahl als Fallback (Browse-Buttons)
- Backup-Statusanzeige (VR/NonVR)
- Umschalten (VR <-> NonVR) nur möglich, wenn beide Backups vollständig sind

## Benötigte Dateien

Die App arbeitet mit diesen drei Dateien:

- `modlist.json`
- `Game.ini`
- `GameUserSettings.ini`

## Voraussetzungen

- Windows
- .NET 9 (https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Installiertes *MechWarrior 5: Mercenaries*

## Nutzung

1. App starten
2. Prüfen, ob die Buttons `Game` und `AppData` grün sind
   - Grün = Pfad ist hinterlegt (automatisch erkannt oder manuell gewählt)
   - Rot = Pfad fehlt → per `Browse` auswählen
3. `Create Backup` ausführen
   - Die App fragt, ob die aktuell aktiven Dateien **VR** oder **NonVR** enthalten
   - Danach werden die drei Dateien in den entsprechenden Backup-Ordner kopiert
4. Schritt 3 für den anderen Modus wiederholen
5. Sobald **VR** und **NonVR** Backups vollständig sind, wird der Toggle aktiv
6. Toggle umlegen → Dateien aus dem gewählten Backup überschreiben die Originaldateien

Tipp: Mit `Auto Detect` kann die automatische Suche jederzeit erneut angestoßen werden.

## Wo werden die Backups gespeichert?

Neben der ausführbaren Datei wird ein Arbeitsordner angelegt:

- `MW5VRC/VR/`
- `MW5VRC/NonVR/`

In beiden Ordnern liegen jeweils:

- `modlist.json`
- `Game.ini`
- `GameUserSettings.ini`

## Typische Standardpfade (Beispiele)

- Spiel (Steam):
  - `C:\Program Files (x86)\Steam\steamapps\common\MechWarrior 5 Mercenaries\`
- AppData (Config Root):
  - `%LOCALAPPDATA%\MW5Mercs\`
  - Config-Ordner: `%LOCALAPPDATA%\MW5Mercs\Saved\Config\WindowsNoEditor\`

## Hinweise

- Das Tool überschreibt Dateien in den Originalpfaden. Nutze es nur, wenn du die Auswirkungen verstehst.
- Wenn Auto-Detect fehlschlägt (z.B. mehrere Steam-Libraries/Benutzerprofile oder Spiel nicht über Steam installiert), wähle die Pfade manuell über `Browse`.
