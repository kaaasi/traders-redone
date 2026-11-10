# Referenzbilder

Dieser Ordner enthält die vorhandenen Video-Referenzbilder (beim Audit 1.826 Dateien).
Die Videos zeigen teilweise eine andere Spielversion; bei Abweichungen ist die eingecheckte
T.EXE im RefEmu maßgeblich. Bilder sind Beobachtungsmaterial, keine alleinige Spezifikation.

Für Dateiliste/Unterordner: `git ls-tree -r --name-only HEAD reference/`.
Reine Codeaufgaben können reference per Sparse Checkout auslassen (`git sparse-checkout set --no-cone '/*' '!/reference/'`).
Neue Bildserien mit Videoquelle, Zeitstempelintervall und Erzeugungsbefehl dokumentieren.
Keine LFS- oder Historienmigration ohne konkreten Speicherbedarf.
