---
applyTo: "**/*.cs"
---

# Ablauf der Anwendung

## Startverhalten

Beim Start der Anwendung:

1. Prüfen, ob Ordner "MW5VRC" im App-Root existiert
2. Falls nicht: Ordner erstellen
3. Prüfen, ob Unterordner existieren:
   - VR
   - NonVR
4. Falls nicht: erstellen

## Backup-Validierung

Für beide Modi (VR und NonVR) prüfen:

Existieren alle drei Dateien?

- modlist.json
- Game.ini
- GameUserSettings.ini

Ergebnis:

- Wenn vollständig: Backup ist gültig
- Wenn unvollständig: Backup ist ungültig

## UI-Verhalten

- Wahlschalter (VR / NonVR) ist deaktiviert, solange:
  - nicht beide Backups vollständig sind

- Beim Start wird versucht, die drei Originaldateien automatisch zu finden.
- Nur wenn die automatische Suche fehlschlägt, wählt der Nutzer die Pfade manuell.

- Nutzer muss vor erstem Backup auswählen:
  - ob aktuelle Dateien VR oder NonVR sind

## Backup-Erstellung

Ablauf:

1. Originaldateien werden automatisch ermittelt oder manuell ausgewählt
2. Nutzer bestätigt aktuellen Modus (VR oder NonVR)
3. Dateien werden in entsprechenden Ordner kopiert

## Umschalten

Beim Wechsel zwischen VR und NonVR:

1. Prüfen, ob Backup vollständig vorhanden
2. Dateien aus Backup laden
3. Originaldateien überschreiben

## Fehlerfälle

- Fehlende Dateien → klare Statusmeldung
- Keine stillen Fehler