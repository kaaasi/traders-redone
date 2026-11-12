"""Check project boundaries and obvious host dependencies in the deterministic core."""
from pathlib import Path
import re
import sys
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[1]
errors = []
for name, allowed in {'Traders.Core': set(), 'Traders.Assets': set(),
                      'Traders.Vga': {'Traders.Assets'}, 'Traders.Audio': {'Traders.Assets'},
                      # Scenes (R4, lane menue): read the core, draw through the layers; no oracle, no host
                      'Traders.Presentation': {'Traders.Core', 'Traders.Assets', 'Traders.Vga'},
                      # Host (R3P-3): no oracle in the shipped program, nothing depends back on it
                      'Traders.App': {'Traders.Core', 'Traders.Assets', 'Traders.Vga', 'Traders.Audio', 'Traders.Presentation'}}.items():
    project = root / 'src' / name / (name + '.csproj')
    for node in ET.parse(project).iter('ProjectReference'):
        target = node.attrib['Include'].replace('\\', '/').split('/')[-1].removesuffix('.csproj')
        if target not in allowed:
            errors.append(f'{name}: unerlaubte Projektabhängigkeit {target}')
# Reverse direction for every project in src/ (#76 P3-1): host, oracle and tools are leaves nobody references, and the
# scenes are referenced by the host only.
leaves = {'Traders.App', 'Traders.RefEmu', 'Traders.Viewer', 'Traders.Tools'}
for project in sorted((root / 'src').glob('*/*.csproj')):
    name = project.stem
    for node in ET.parse(project).iter('ProjectReference'):
        target = node.attrib['Include'].replace('\\', '/').split('/')[-1].removesuffix('.csproj')
        if target in leaves:
            errors.append(f'{name}: Rückrichtung auf {target} (Host, Orakel und Werkzeuge sind Blätter)')
        if target == 'Traders.Presentation' and name != 'Traders.App':
            errors.append(f'{name}: Szenen nur vom Host referenziert (Traders.Presentation)')
# Scenes only read the game state (AGENTS.md §4): no writes to a Dgroup (Set, raw Bytes) and no phase machine calls.
for path in (root / 'src/Traders.Presentation').rglob('*.cs'):
    if 'obj' in path.parts or 'bin' in path.parts:
        continue
    source = re.sub(r'/\*.*?\*/|//[^\n]*', '', path.read_text(), flags=re.S)
    if re.search(r'\.Set\s*\(|\.Bytes\b|\bPhaseMachine\b|\bMenuRunner\b|\bIWeekStepHandler\b', source):
        errors.append(f'{path.relative_to(root)}: Szene schreibt Spielzustand oder treibt den Kern')
for path in (root / 'src/Traders.Core').rglob('*.cs'):
    if 'obj' in path.parts or 'bin' in path.parts:
        continue
    # Comments are excluded. This check supplements review; it is not a C# semantic analyzer.
    source = re.sub(r'/\*.*?\*/|//[^\n]*', '', path.read_text(), flags=re.S)
    if re.search(r'\b(?:DateTime\.(?:Now|UtcNow|Today)|Random\.Shared|new\s+(?:System\.)?Random\s*\(|File\.|Directory\.|Console\.|Thread\.Sleep|Task\.Delay|Environment\.TickCount|Stopwatch|Guid\.NewGuid)', source):
        errors.append(f'{path.relative_to(root)}: Host-/Zufallszugriff im Core')
# Random numbers only in the core (AGENTS.md §4, F3 = A): scenes, output and host never draw or seed random() themselves;
# per-frame draws are core events of the phase machine. Traders.App gets its own rule with lane host (R3).
for path in (root / 'src').rglob('*.cs'):
    rel = path.relative_to(root)
    if 'obj' in path.parts or 'bin' in path.parts or rel.parts[1] in ('Traders.Core', 'Traders.App'):
        continue
    source = re.sub(r'/\*.*?\*/|//[^\n]*', '', path.read_text(), flags=re.S)
    if re.search(r'\bOriginalRandom\b|\bRandomSeed\b', source):
        errors.append(f'{rel}: Zufall außerhalb des Kerns (OriginalRandom/RandomSeed)')
print('\n'.join(errors) if errors else 'Architekturgrenzen geprüft')
sys.exit(bool(errors))
