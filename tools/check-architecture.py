"""Check project boundaries and obvious host dependencies in the deterministic core."""
from pathlib import Path
import re
import sys
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[1]
errors = []
# Allowed project references of the layered projects (forward direction).
_ALLOWED = {'Traders.Core': set(), 'Traders.Assets': set(),
            'Traders.Vga': {'Traders.Assets'}, 'Traders.Audio': {'Traders.Assets'},
            # Scenes (R4, lane menue): read the core, draw through the layers; no oracle, no host
            'Traders.Presentation': {'Traders.Core', 'Traders.Assets', 'Traders.Vga'},
            # Host (R3P-3): no oracle in the shipped program, nothing depends back on it
            'Traders.App': {'Traders.Core', 'Traders.Assets', 'Traders.Vga', 'Traders.Audio', 'Traders.Presentation'}}
# Reverse direction for every project in src/ (#76 P3-1): host, oracle and tools are leaves nobody references, and the
# scenes are referenced by the host only.
_LEAVES = {'Traders.App', 'Traders.RefEmu', 'Traders.Viewer', 'Traders.Tools'}


def reference_violations(references):
    """references: project -> set of referenced projects (every project in src/). Forward rule for the layered projects
    (a missing layered project is an error), reverse rule for all projects."""
    found = []
    for name, allowed in _ALLOWED.items():
        if name not in references:
            found.append(f'{name}: Projekt fehlt')
            continue
        for target in sorted(references[name] - allowed):
            found.append(f'{name}: unerlaubte Projektabhängigkeit {target}')
    for name, targets in sorted(references.items()):
        for target in sorted(targets):
            if target in _LEAVES:
                found.append(f'{name}: Rückrichtung auf {target} (Host, Orakel und Werkzeuge sind Blätter)')
            if target == 'Traders.Presentation' and name != 'Traders.App':
                found.append(f'{name}: Szenen nur vom Host referenziert (Traders.Presentation)')
    return found


def project_references(directory):
    """project name -> referenced project names for every */*.csproj below directory."""
    refs = {}
    for project in sorted(directory.glob('*/*.csproj')):
        refs[project.stem] = {node.attrib['Include'].replace('\\', '/').split('/')[-1].removesuffix('.csproj')
                              for node in ET.parse(project).iter('ProjectReference')}
    return refs


errors.extend(reference_violations(project_references(root / 'src')))

def strip_comments(text):
    return re.sub(r'/\*.*?\*/|//[^\n]*', '', text, flags=re.S)


def strip_code(text):
    """Comments, string and char literals removed (their braces and quotes would confuse the method scanner)."""
    text = strip_comments(text)
    text = re.sub(r'@"(?:[^"]|"")*"', '""', text)
    text = re.sub(r'\$?"(?:\\.|[^"\\\n])*"', '""', text)
    return re.sub(r"'(?:\\.|[^'\\\n])+'", "' '", text)


_KEYWORDS = {'if', 'for', 'foreach', 'while', 'switch', 'catch', 'using', 'lock', 'return', 'new', 'nameof', 'typeof',
             'sizeof', 'default', 'checked', 'unchecked', 'fixed', 'when', 'base', 'this', 'throw', 'await', 'stackalloc'}
_DIRECT_WRITE = re.compile(
    r'\.\s*Set\s*\('                                                  # Dgroup.Set and every other core setter
    r'|\bBytes\s*\[[^\]]*\]\s*(?:[-+*/%&|^]?=(?!=)|\+\+|--)'           # raw byte store
    r'|(?:\+\+|--)\s*(?:\w+\s*\.\s*)?Bytes\s*\['
    r'|\bBytes\s*\.\s*AsSpan\s*\([^;]*?\)\s*\.\s*(?:Fill|Clear)\b'
    r'|\bCopyTo\s*\([^;]*\bBytes\b'
    r'|\bArray\s*\.\s*(?:Copy|Clear|Fill)\s*\([^;]*\bBytes\b'
    r'|\bWrite\w*(?:Little|Big)Endian\s*\(')


def _block_end(text, start):
    """Index after the block that opens at text[start] == '{'."""
    depth = 0
    for i in range(start, len(text)):
        if text[i] == '{':
            depth += 1
        elif text[i] == '}':
            depth -= 1
            if depth == 0:
                return i + 1
    return len(text)


def _types(text):
    """Classes, structs and records with their brace extent."""
    found = []
    for m in re.finditer(r'\b(?:class|struct|record(?:\s+(?:class|struct))?|interface)\s+(\w+)[^{;]*\{', text):
        found.append((m.start(), _block_end(text, m.end() - 1), m.group(1)))
    return found


def _methods(text):
    """(class, name, static, body, parameters) for every method, constructor, local function and expression-bodied
    member."""
    types = _types(text)
    head = re.compile(r'(\w+)\s*(?:<[^<>()]*>)?\s*\(')
    pos = 0
    while (m := head.search(text, pos)):
        pos = m.end()
        name = m.group(1)
        if name in _KEYWORDS:
            continue
        depth, i = 1, m.end()
        while i < len(text) and depth:
            depth += {'(': 1, ')': -1}.get(text[i], 0)
            i += 1
        tail = re.match(r'\s*(?::\s*(?:base|this)\s*\([^)]*\)\s*)?(?:where\b[^{;=]*)?(\{|=>)', text[i:])
        if not tail:
            continue
        before = text[:m.start()].rstrip()
        prefix = re.search(r'[^;{}]*$', before).group(0)
        owner = [t for t in types if t[0] < m.start() < t[1]]
        cls = min(owner, key=lambda t: t[1] - t[0])[2] if owner else None
        if not (prefix and (prefix[-1].isalnum() or prefix[-1] in '_>]?')) and name != cls:
            continue  # not a declaration: call or lambda argument
        if re.search(r'\b(?:return|new|await|throw|is|as|case)\s*$', prefix) or '=' in prefix:
            continue
        start = i + tail.end()
        if tail.group(1) == '{':
            end = _block_end(text, start - 1)
        else:
            end = text.find(';', start)
        yield cls, name, bool(re.search(r'\bstatic\b', prefix)), text[start:end], text[m.end():i - 1]
        pos = start


def _locals(text, known_types, aliases=True):
    """Variable, field, property and parameter names declared with a known core type: name -> type. `aliases=False` leaves
    out the `var` aliases (the caller scopes them itself, see `_alias_scopes`)."""
    found = {}
    for m in re.finditer(r'\b([A-Z]\w*)(?:<[^<>]*>)?\??\s+(_?[A-Za-z]\w*)\s*(?=[;=,){]|=>)', text):
        if m.group(1) in known_types:
            found[m.group(2)] = m.group(1)
    for m in re.finditer(r'\bvar\s+(\w+)\s*=\s*new\s+([A-Z]\w*)\s*\(', text):
        if m.group(2) in known_types:
            found[m.group(1)] = m.group(2)
    # #193 R7H-3: `var t = _texts;` / `var a = this._a!;` aliases a known variable; chains resolve by repeating until stable.
    changed = aliases
    while changed:
        changed = False
        for m in re.finditer(_ALIAS, text):
            if m.group(2) in found and m.group(1) not in found:
                found[m.group(1)] = found[m.group(2)]
                changed = True
    return found


_ALIAS = r'\bvar\s+(\w+)\s*=\s*(?:this\s*\.\s*)?(\w+)\s*!?\s*(?=[;,)])'


def _enclosing_block(text, pos):
    """(start, end) of the innermost block { ... } around text[pos]; the whole text outside any block."""
    depth = 0
    for i in range(pos - 1, -1, -1):
        if text[i] == '}':
            depth += 1
        elif text[i] == '{':
            if depth == 0:
                return i, _block_end(text, i)
            depth -= 1
    return 0, len(text)


def _alias_scopes(text, base):
    """#221 R8S-2: `var t = x;` aliases a known variable only from its declaration to the end of its own block (a method body),
    not in the whole file. Chains resolve by repeating until stable; the source may be a base variable (declared with a known
    type anywhere in the file) or an alias whose scope contains the declaration. Returns [(name, type, start, end)]."""
    found = []  # (name, type, scope start = end of the declaration, scope end)
    changed = True
    while changed:
        changed = False
        for m in re.finditer(_ALIAS, text):
            if any(f[0] == m.group(1) and f[2] == m.end() for f in found):
                continue
            source = m.group(2)
            kind = next((f[1] for f in found if f[0] == source and f[2] <= m.start() < f[3]), base.get(source))
            if kind:
                found.append((m.group(1), kind, m.end(), _enclosing_block(text, m.start())[1]))
                changed = True
    return found


_STEP_HANDLERS = {'IWeekStepHandler', 'IProgramStepHandler'}

# Writable game state outside the data segment (#163 R6E-2): the text memory behind the far text pointers. Its writes are
# array stores or interface calls the body scan cannot see, so they are named here (instance members); callers through a
# parameter, local or field of these types (IncidentText.FormatNumber) are found by the propagation.
_STATE_WRITERS = {'ITextMemory.Write': False, 'TextBufferMemory.Write': False}


def _step_handlers(texts):
    """Classes that implement a step handler interface of the phase machines, directly or through a base class (R5B-3)."""
    bases = {}
    for text in texts:
        for m in re.finditer(r'\b(?:class|struct|record(?:\s+(?:class|struct))?)\s+(\w+)([^{;]*)\{', text):
            head = re.sub(r'\([^()]*(?:\([^()]*\)[^()]*)*\)', '', m.group(2))  # primary constructor parameters
            bases[m.group(1)] = set(re.findall(r'\b([A-Z]\w*)\b', head.split(':', 1)[1])) if ':' in head else set()
    handlers, changed = set(), True
    while changed:
        changed = False
        for cls, names in bases.items():
            if cls not in handlers and names & (_STEP_HANDLERS | handlers):
                handlers.add(cls)
                changed = True
    return handlers


def core_members(sources):
    """Fields and properties of the core declared with a core type: name -> set of types (for chains st.Report!.Run())."""
    texts = [strip_code(t) for t in sources]
    classes = {name for text in texts for *_, name in _types(text)}
    members = {}
    for text in texts:
        for m in re.finditer(r'\b(?:public|internal|protected|private)\s+(?:(?:static|readonly|required|virtual|override|'
                             r'sealed|new)\s+)*([A-Z]\w*)(?:<[^<>]*>)?\??\s+(\w+)\s*(?=[;={]|=>)', text):
            if m.group(1) in classes:
                members.setdefault(m.group(2), set()).add(m.group(1))
    return members


def core_writers(sources):
    """Core members that change the game state (#101 R4M-2, #103 P3-1, #105 P3-1), decided from the bodies: a method writes
    if it stores into a data segment (Set, raw bytes) or calls a writing member - qualified (Class.Method), unqualified in
    the same class, through a variable, parameter (#139) or field of a core type, or as a constructor (new X(...)). Return type and parameters do not
    matter (Wealth.Wares returns int and writes DS:45D0). Every method of a step handler (IWeekStepHandler,
    IProgramStepHandler, also through a base class) counts as writing: it drives the core (R5B-3, ReportStep.Tick).
    Returns {'Class.Method': is_static}; 'X.X' marks a constructor."""
    members = []  # (cls, name, static, body, parameters)
    texts = [strip_code(t) for t in sources]
    for text in texts:
        members.extend(_methods(text))
    classes = {c for c, *_ in members if c}
    for text in texts:
        classes.update(name for *_, name in _types(text))
    # Fields, properties and primary constructor parameters of each class (#139: _a.Write(d) through a field). The scan
    # covers the whole class text, so a local of another method may count too - that errs towards "writing".
    fields = {}
    for text in texts:
        for start, end, cls in _types(text):
            fields.setdefault(cls, {}).update(_locals(text[start:end], classes))
    handlers = _step_handlers(texts)
    writers = dict(_STATE_WRITERS)
    for cls, name, static, body, _ in members:
        if cls and (_DIRECT_WRITE.search(body) or cls in handlers):
            writers[f'{cls}.{name}'] = static
    changed = True
    while changed:
        changed = False
        for cls, name, static, body, parameters in members:
            key = f'{cls}.{name}'
            if not cls or key in writers:
                continue
            calls = {f'{c}.{n}' for c, n in re.findall(r'\b([A-Z]\w*)\s*\.\s*(\w+)\s*\(', body)}
            calls |= {f'{cls}.{n}' for n in re.findall(r'(?<![.\w])(\w+)\s*\(', body)}
            calls |= {f'{c}.{c}' for c in re.findall(r'\bnew\s+([A-Z]\w*)\s*\(', body)}
            # locals of the body, parameters of the signature (#139 R5G-1) and fields of the owning class
            variables = {**fields.get(cls, {}), **_locals(parameters + ')', classes), **_locals(body, classes)}
            calls |= {f'{variables[v]}.{n}' for v, n in re.findall(r'\b(\w+)\s*\.\s*(\w+)\s*\(', body) if v in variables}
            if calls & writers.keys():
                writers[key] = static
                changed = True
    return writers


# Named in the R4 reviews (#103 P3-1): field cursor, grid writes and the control readers are never called from a scene,
# whether or not the body scan sees them write.
_FORBIDDEN_CALLS = re.compile(r'\b(?:FieldCursor\s*\.\s*\w+|FieldGrid\s*\.\s*Set|ControlReader\s*\.\s*\w+)\s*\(')


def scene_violations(source, writers, members=None):
    """Scenes only read the game state (AGENTS.md §4): no writes to a Dgroup (Set, raw Bytes), no phase machine, no static
    core call that writes the state (KeyTable.Write, Wealth.Wares, ...), no instance of a writing core class
    (new MainMenu(...).Run()), no writing call through a variable of a core type, through a field or property of the core
    (st.Report!.Run(), `members` from core_members) or as a method group (Func<Dgroup, int> f = Wealth.Wares; R5B-3)."""
    members = members or {}
    source = strip_code(source)
    found = []
    if re.search(r'\.Set\s*\(|\.Bytes\b|\bPhaseMachine\b|\bProgramMachine\b|\bMenuRunner\b|\bIWeekStepHandler\b|\bIProgramStepHandler\b', source):
        found.append('Szene schreibt Spielzustand oder treibt den Kern')
    for m in _FORBIDDEN_CALLS.finditer(source):
        found.append(f'Szene ruft Kernroutine {re.sub(r"\s+", "", m.group(0))[:-1]} (Cursor, Raster, Steuerung schreiben)')
    writing_classes = {w.split('.')[0] for w in writers}
    for w, static in sorted(writers.items()):
        cls, method = w.split('.')
        # call or method group (no '(' needed)
        if static and re.search(rf'\b{cls}\s*\.\s*{method}\b', source):
            found.append(f'Szene ruft schreibende Kernroutine {w}')
    for cls in sorted(writing_classes):
        if re.search(rf'\bnew\s+(?:\w+\s*\.\s*)*{cls}\s*\(', source):
            found.append(f'Szene erzeugt schreibende Kerninstanz {cls}')
    variables = _locals(source, writing_classes, aliases=False)
    # declared variables count in the whole file, `var` aliases only inside their block (#221)
    scopes = [(0, len(source), variables)] + [(a, b, {name: kind}) for name, kind, a, b in _alias_scopes(source, variables)]
    for start, end, scoped in scopes:
        for var, method in re.findall(r'\b(\w+)\s*[!?]*\s*\.\s*(\w+)\b', source[start:end]):
            if var in scoped and f'{scoped[var]}.{method}' in writers and not writers[f'{scoped[var]}.{method}']:
                found.append(f'Szene ruft schreibende Kernroutine {scoped[var]}.{method} über {var}')
    for member, method in re.findall(r'\.\s*(\w+)\s*[!?]*\s*\.\s*(\w+)\b', source):
        for cls in sorted(members.get(member, ())):
            if f'{cls}.{method}' in writers and not writers[f'{cls}.{method}']:
                found.append(f'Szene ruft schreibende Kernroutine {cls}.{method} über die Kette .{member}')
    return found


_ACCESSOR = r'\b(?:U8|S8|U16|S16|U32|S32|Set)\s*\('


def raw_offset_violations(source):
    """#81 B4, #105 P3-2: outside Core and RefEmu no data segment access by raw offset; names from DgroupLayout or a
    layout class in the core. Hex literals in any accessor; on a Dgroup variable also decimal literals and constants
    of the file itself (const int Req = 0x3CCC; d.U8(Req))."""
    source = strip_code(source)
    found = []
    if re.search(_ACCESSOR + r'\s*0x[0-9A-Fa-f]+\b', source):
        found.append('Rohoffset im Zugriff auf das Datensegment (Name aus DgroupLayout bzw. Layoutklasse verwenden)')
    if re.search(r'\b0x10[Dd]2\b', source):
        found.append('Rohoffset des Zufallszustands DS:10D2')
    groups = set(re.findall(r'\bDgroup\??\s+(\w+)', source))
    groups |= set(re.findall(r'\bvar\s+(\w+)\s*=\s*new\s+Dgroup\b', source))
    constants = set(re.findall(r'\b(?:const|static\s+readonly)\s+\w+\s+(\w+)\s*=\s*(?:\(\w+\)\s*)?(?:0x[0-9A-Fa-f]+|\d+)\s*;', source))
    for var, arg in re.findall(r'\b(\w+)\s*\.\s*(?:U8|S8|U16|S16|U32|S32|Set)\s*\(\s*([^,()]+?)\s*[,)]', source):
        if var not in groups:
            continue
        literal = re.fullmatch(r'\d+(?:\s*[-+]\s*\w+)?', arg)
        constant = re.fullmatch(r'(\w+)(?:\s*[-+]\s*\w+)?', arg)
        if literal:
            found.append(f'Rohoffset {arg} im Zugriff auf das Datensegment (Dezimalliteral)')
        elif constant and constant.group(1) in constants:
            found.append(f'Rohoffset über lokale Konstante {constant.group(1)} im Zugriff auf das Datensegment')
    return found


# Self-test: every rule must catch its counterexample and leave the reading forms alone (counter-checks of #101 R4M-2,
# #103 P3-1, #105 P3-1/P3-2 and #81 B4). A `catch` probe must be reported at least once (one violation may trip several
# overlapping rules, e.g. FieldGrid.Set); a `clean` probe must not be reported at all.
CATCH, CLEAN = True, False
_core = ['public static class KeyTable { public static void Write(Dgroup state, IReadOnlyCollection<byte> held) { state.Set(1, (byte)0); } }',
         'public sealed class MainMenu { private readonly Dgroup _d; public MainMenu(Dgroup d) { _d = d; }'
         ' public static void ApplyLevel(Dgroup d) { d.Set(2, (byte)1); } public static int? Peek(Dgroup d) => d.U8(3);'
         ' public IEnumerable<int> Run() { yield return Step(); } private int Step() { _d.Bytes[4] = 1; return 0; } }',
         'public static class Wealth { public static int Wares(Dgroup d) { int s = 0; for (int i = 0; i < 4; i++) { d.Set(Loop, (short)i); s += d.S16(i); } return s; }'
         ' public static int Total(Dgroup d) => Wares(d) + 1; public static int Cash(Dgroup d) => d.S32(5); }',
         'public static class Ranking { public static void Order(Dgroup d) { var m = new MainMenu(d); foreach (var x in m.Run()) { } }'
         ' public static void Apply(Dgroup d) { d.Set(8, (byte)0); } }',
         'public static class FieldCursor { public static bool IsVisible(Dgroup d) => d.U8(6) != 0; }',
         'public static class FieldGrid { public static void Set(Dgroup d, int x, byte v) { d.Set(x, v); } public static byte Get(Dgroup d, int x) => d.U8(x); }',
         'public static class ControlReader { public static int Peek(Dgroup d) => d.U8(7); }',
         # R5B-3: a step handler drives the core even if its body only moves an enumerator (also through a base class);
         # its Report property makes a chain to a writing core class
         'public interface IWeekStepHandler { StepOutcome Tick(StepContext c); }',
         'public sealed class Report { public bool Aborted { get; } public IEnumerable<int> Run() { Ranking.Apply(_d); yield return 0; } }',
         'public class ReportStep(int arg = 1) : IWeekStepHandler { private IEnumerator<int>? _run;'
         ' public Report? Report { get; private set; } public StepOutcome Tick(StepContext c) { _run!.MoveNext(); return default; } }',
         'public sealed class LaterStep : ReportStep { public new StepOutcome Tick(StepContext c) => default; }',
         # #139 R5G-1: writes reached through a method parameter (B.Go) and through a field of the class (Holder.Go)
         'public class A { public void Write(Dgroup d) { d.Set(0, (byte)1); } public int Read(Dgroup d) => d.U8(0); }',
         'public static class B { public static void Go(A a, Dgroup d) { a.Write(d); } public static int Look(Dgroup d) => d.U8(1); }',
         'public sealed class Holder { private readonly A _a = new(); public void Go(Dgroup d) { _a.Write(d); }'
         ' public int Peek(Dgroup d) => _a.Read(d); }',
         # #163 R6E-2: the text memory outside the data segment and a routine that writes into it through a parameter
         'public interface ITextMemory { byte Read(ushort segment, ushort offset); void Write(ushort segment, ushort offset, byte value); }',
         'public sealed class TextBufferMemory(Dgroup state, Func<byte[]?> buffer) : ITextMemory {'
         ' public byte Read(ushort s, ushort o) => buffer()![o]; public void Write(ushort s, ushort o, byte v) { var b = buffer()!; b[o] = v; } }',
         'public static class IncidentText { public static void FormatNumber(Dgroup d, ITextMemory memory, uint n, short w, ushort o, ushort s)'
         ' { for (int i = 0; i < w; i++) memory.Write(s, (ushort)(o + i), d.U8(1)); }'
         ' public static ushort Length(ITextMemory memory, ushort o, ushort s) { ushort n = 0; while (memory.Read(s, (ushort)(o + n)) != 0) n++; return n; } }']
_layers = {**{k: set(v) for k, v in _ALLOWED.items()}, 'Traders.RefEmu': {'Traders.Core', 'Traders.Assets'},
           'Traders.Viewer': {'Traders.Assets', 'Traders.Vga'}, 'Traders.Tools': {'Traders.Assets'}}
_writers = core_writers(_core)
_members = core_members(_core)
for probe, mode in [(scene_violations('KeyTable.Write(_state, keys);', _writers), CATCH),
                    (scene_violations('MainMenu.ApplyLevel(_state);', _writers), CATCH),
                    (scene_violations('int w = Wealth.Wares(_state);', _writers), CATCH),
                    (scene_violations('int t = Wealth.Total(_state);', _writers), CATCH),
                    (scene_violations('Ranking.Order(_state);', _writers), CATCH),
                    (scene_violations('foreach (var w in new MainMenu(_state).Run()) { }', _writers), CATCH),
                    (scene_violations('private readonly MainMenu _menu; void F() { _menu.Run(); }', _writers), CATCH),
                    (scene_violations('if (FieldCursor.IsVisible(_state)) { }', _writers), CATCH),
                    (scene_violations('FieldGrid.Set(_state, 1, 2);', _writers), CATCH),
                    (scene_violations('int k = ControlReader.Peek(_state);', _writers), CATCH),
                    (scene_violations('var x = MainMenu.Peek(_state); int c = Wealth.Cash(_state); byte g = FieldGrid.Get(_state, 1);'
                                      ' string s = "KeyTable.Write(_state, k);"; // KeyTable.Write(_state, k);', _writers), CLEAN),
                    # R5B-3 counter-checks (one per rule) and the reading forms they must leave alone
                    (scene_violations('ReportStep st = _st; foreach (var x in st.Report!.Run()) { }', _writers, _members), CATCH),
                    (scene_violations('ReportStep st = _st; st.Tick(default!);', _writers, _members), CATCH),
                    (scene_violations('LaterStep st = _st; st.Tick(default!);', _writers, _members), CATCH),
                    (scene_violations('Func<Dgroup, int> f = Wealth.Wares; f(_state);', _writers, _members), CATCH),
                    (scene_violations('Action<Dgroup> a = Ranking.Apply;', _writers, _members), CATCH),
                    (scene_violations('Func<IEnumerable<int>> r = _st.Report!.Run;', _writers, _members), CATCH),
                    (scene_violations('ReportStep st = _st; bool a = st.Report!.Aborted; Func<Dgroup, int> c = Wealth.Cash;',
                                      _writers, _members), CLEAN),
                    # #139 R5G-1: a writing core call through a parameter or a field of a core type is writing too
                    (scene_violations('B.Go(a, _state);', _writers, _members), CATCH),
                    (scene_violations('Holder h = _h; h.Go(_state);', _writers, _members), CATCH),
                    (scene_violations('int c = B.Look(_state); Holder h = _h; int p = h.Peek(_state);', _writers, _members), CLEAN),
                    # #163 R6E-2: writes into the text memory (interface, implementation, routine) and the reading forms
                    (scene_violations('ITextMemory t = _texts; t.Write(1, 2, 3);', _writers, _members), CATCH),
                    (scene_violations('IncidentText.FormatNumber(_d, _texts, 5, 4, 0, 0);', _writers, _members), CATCH),
                    (scene_violations('TextBufferMemory t = _t; t.Write(1, 2, 3);', _writers, _members), CATCH),
                    (scene_violations('private readonly ITextMemory _texts; void F() { _texts.Write(1, 2, 3); }', _writers, _members), CATCH),
                    # #193 R7H-3: writes through a `var` alias of a field, a local or a parameter (also chained) are writes too
                    (scene_violations('private readonly ITextMemory _texts; void F() { var t = _texts; t.Write(1, 2, 3); }', _writers, _members), CATCH),
                    (scene_violations('void F(TextBufferMemory m) { var a = m; var b = a; b.Write(1, 2, 3); }', _writers, _members), CATCH),
                    (scene_violations('private readonly Holder _h; void F() { var h = this._h!; h.Go(_state); }', _writers, _members), CATCH),
                    (scene_violations('private readonly ITextMemory _texts; void F() { var t = _texts; byte b = t.Read(1, 2); }', _writers, _members), CLEAN),
                    # #221 R8S-2: an alias counts in its own method only; the same name in another method stays clean, in a nested block it catches
                    (scene_violations('private readonly ITextMemory _texts; void F() { var t = _texts; byte b = t.Read(1, 2); }'
                                      ' void G(Other t) { t.Write(1, 2, 3); }', _writers, _members), CLEAN),
                    (scene_violations('private readonly ITextMemory _texts; void F() { var t = _texts; byte b = t.Read(1, 2); }'
                                      ' void G() { if (x) { var t = _texts; t.Write(1, 2, 3); } }', _writers, _members), CATCH),
                    (scene_violations('private readonly ITextMemory _texts; void F() { var t = _texts; if (x) { t.Write(1, 2, 3); } }',
                                      _writers, _members), CATCH),
                    (scene_violations('private readonly ITextMemory _texts; void F() { if (x) { var t = _texts; } t.Write(1, 2, 3); }',
                                      _writers, _members), CLEAN),
                    (scene_violations('private readonly ITextMemory _texts; void F() { byte b = _texts.Read(1, 2);'
                                      ' ushort n = IncidentText.Length(_texts, 1, 2); }', _writers, _members), CLEAN),
                    # #76 P3-1: one violation per reference rule, and the real layering stays clean
                    (reference_violations({**_layers, 'Traders.Vga': {'Traders.Assets', 'Traders.Core'}}), CATCH),
                    (reference_violations({**_layers, 'Traders.Tools': {'Traders.App'}}), CATCH),
                    (reference_violations({**_layers, 'Traders.Viewer': {'Traders.RefEmu'}}), CATCH),
                    (reference_violations({**_layers, 'Traders.Viewer': {'Traders.Presentation'}}), CATCH),
                    (reference_violations({k: v for k, v in _layers.items() if k != 'Traders.Core'}), CATCH),
                    (reference_violations(_layers), CLEAN),
                    (raw_offset_violations('uint seed = d.U32(0x10D2);'), CATCH),
                    (raw_offset_violations('void F(Dgroup d) { short v = d.S16(2298); }'), CATCH),
                    (raw_offset_violations('const int Req = 0x3CCC; void F(Dgroup d) { byte v = d.U8(Req); }'), CATCH),
                    (raw_offset_violations('private const int Row = 4; void F(Dgroup d, Span<int> s) { byte v = d.U8(MenuLayout.PlaqueTop + Row); s[Row] = 0; }'), CLEAN),
                    (raw_offset_violations('void F(Dgroup d, Grid g) { short top = d.S16(MenuLayout.PlaqueTop); g.Set(3, 1); }'), CLEAN)]:
    if (mode == CATCH and not probe) or (mode == CLEAN and probe):
        errors.append(f'Selbstprüfung check-architecture.py: {"Befund erwartet" if mode else "kein Befund erwartet"}, {len(probe)} gefunden')

core_sources = [p.read_text() for p in (root / 'src/Traders.Core').rglob('*.cs') if 'obj' not in p.parts and 'bin' not in p.parts]
writers, members = core_writers(core_sources), core_members(core_sources)
for path in (root / 'src/Traders.Presentation').rglob('*.cs'):
    if 'obj' in path.parts or 'bin' in path.parts:
        continue
    for v in scene_violations(path.read_text(), writers, members):
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
