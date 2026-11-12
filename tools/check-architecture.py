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

def strip_comments(text):
    return re.sub(r'/\*.*?\*/|//[^\n]*', '', text, flags=re.S)


def core_writers(sources):
    """Static core methods that change a data segment (R4M-2, #101): every static method with a Dgroup parameter that
    returns void, plus the ones known to write while returning a value. Returns 'Class.Method' names."""
    writers = set()
    for text in sources:
        text = strip_comments(text)
        classes = [(m.start(), m.group(1)) for m in re.finditer(r'\bclass\s+(\w+)', text)]
        for m in re.finditer(r'\bstatic\s+([\w<>\[\]?,. ]+?)\s+(\w+)\s*\(([^)]*)\)', text):
            returns, name, params = m.group(1).split()[-1], m.group(2), m.group(3)
            if not re.search(r'\bDgroup\b', params):
                continue
            owner = [c for pos, c in classes if pos < m.start()]
            if owner and (returns == 'void' or name in ('Read', 'Write', 'Run', 'Store')):
                writers.add(f'{owner[-1]}.{name}')
    return writers


def scene_violations(source, writers):
    """Scenes only read the game state (AGENTS.md §4): no writes to a Dgroup (Set, raw Bytes), no phase machine and no
    static core call that writes the state (KeyTable.Write, MainMenu.ApplyLevel, …)."""
    source = strip_comments(source)
    found = []
    if re.search(r'\.Set\s*\(|\.Bytes\b|\bPhaseMachine\b|\bProgramMachine\b|\bMenuRunner\b|\bIWeekStepHandler\b|\bIProgramStepHandler\b', source):
        found.append('Szene schreibt Spielzustand oder treibt den Kern')
    for w in sorted(writers):
        cls, method = w.split('.')
        if re.search(rf'\b{cls}\s*\.\s*{method}\s*\(', source):
            found.append(f'Szene ruft schreibende Kernroutine {w}')
    return found


def raw_offset_violations(source):
    """#81 B4: outside Core and RefEmu no data segment access by raw offset (d.U32(0x10D2)); names from DgroupLayout."""
    source = strip_comments(source)
    found = []
    if re.search(r'\b(?:U8|S8|U16|S16|U32|S32|Set)\s*\(\s*0x[0-9A-Fa-f]+\b', source):
        found.append('Rohoffset im Zugriff auf das Datensegment (Name aus DgroupLayout bzw. Layoutklasse verwenden)')
    if re.search(r'\b0x10[Dd]2\b', source):
        found.append('Rohoffset des Zufallszustands DS:10D2')
    return found


# Self-test: the rules must catch their counterexamples (counter-check of #101 R4M-2 and #81 B4).
_writers = core_writers(['public static class KeyTable { public static void Write(Dgroup state, IReadOnlyCollection<byte> held) { } }',
                         'public sealed class MainMenu { public static void ApplyLevel(Dgroup d) { } public static int? Peek(Dgroup d) => 0; }'])
for probe, expected in [(scene_violations('KeyTable.Write(_state, keys);', _writers), 1),
                        (scene_violations('MainMenu.ApplyLevel(_state);', _writers), 1),
                        (scene_violations('var x = MainMenu.Peek(_state); // KeyTable.Write(_state, k);', _writers), 0),
                        (raw_offset_violations('uint seed = d.U32(0x10D2);'), 2),
                        (raw_offset_violations('short top = d.S16(MenuLayout.PlaqueTop);'), 0)]:
    if len(probe) != expected:
        errors.append(f'Selbstprüfung check-architecture.py: {expected} Befund(e) erwartet, {len(probe)} gefunden')

writers = core_writers(p.read_text() for p in (root / 'src/Traders.Core').rglob('*.cs') if 'obj' not in p.parts and 'bin' not in p.parts)
for path in (root / 'src/Traders.Presentation').rglob('*.cs'):
    if 'obj' in path.parts or 'bin' in path.parts:
        continue
    for v in scene_violations(path.read_text(), writers):
        errors.append(f'{path.relative_to(root)}: {v}')
for path in (root / 'src').rglob('*.cs'):
    rel = path.relative_to(root)
    if 'obj' in path.parts or 'bin' in path.parts or rel.parts[1] in ('Traders.Core', 'Traders.RefEmu'):
        continue
    for v in raw_offset_violations(path.read_text()):
        errors.append(f'{rel}: {v}')
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
