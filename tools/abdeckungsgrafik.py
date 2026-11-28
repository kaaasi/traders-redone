#!/usr/bin/env python3
"""Coverage overview of T.EXE in five colours (green, yellow, blue, grey, red), weighted by bytes.

Procedure and colour rules: docs/vorlagen/abdeckung-grafik.md (adopted from kaaasi/eishockey-manager-redone). Builds on
tools/funktionskarte.py (functions, findings) and tools/markerliste.py (class of each [APPROX] line).

Usage:
    python3 tools/abdeckungsgrafik.py <path/to/T.EXE> --html out.html [--json out.json] [--ohne-texte] [--stand SHA]

--ohne-texte leaves out the short strings of the original in the hints (version for the repo, AGENTS.md 2.1).
Writes nothing into the repo unless the output paths point there.

Colours (first match wins; "belongs to an [APPROX]" = the marker of the finding, see tools/funktionskarte.py step 3):
  blue   a finding of the function belongs to an [APPROX] of class E
  green  status "umgesetzt"
  yellow status "genähert" with an [APPROX] of class K or D; or "dokumentiert"/"unbekannt" with a documented
         reimplementation without address (YELLOW, with proof: file in the port)
  grey   only with proof (GREY): the docs name the function as dropped or as platform layer without counterpart
  red    everything else
"""

from __future__ import annotations

import argparse
import datetime
import html
import json
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import funktionskarte as fk  # noqa: E402
import markerliste as ml  # noqa: E402

COLOURS = ["grün", "gelb", "blau", "grau", "rot"]
HEX = {"grün": "#1b6e45", "gelb": "#e3b230", "blau": "#3a70d6", "grau": "#a3a8ad", "rot": "#f2735f"}
RULES = {
    "grün": "belegt umgesetzt: Status „umgesetzt“, keine Fundstelle gehört zu einem [APPROX] der Klasse E",
    "gelb": "umgesetzt ohne Beleg: Status „genähert“ mit [APPROX] der Klasse K oder D; oder ohne Adresse nachgebaut (mit Nachweis)",
    "blau": "anders umgesetzt: eine Fundstelle gehört zu einem [APPROX] der Klasse E (Vorrang vor grün und gelb)",
    "grau": "weggefallen: nur mit Beleg (Doku nennt die Funktion als entfallen bzw. Plattformschicht ohne Gegenstück)",
    "rot": "noch unbekannt: alles Übrige; im Zweifel rot",
}

# Grey: (segment, first offset, last offset or None = one function starting there, proof)
GREY = [
    (0x0000, 0x0000, 0x534F, "docs/analyse/BESTAND.md:40 (Turbo-C-Laufzeit, „nicht zu portieren“); Kennung: Borland-Copyright "
                             "der Laufzeit am Anfang der DGROUP (DS:0004)"),
    (0x28DA, 0x0000, 0x000F, "docs/analyse/BESTAND.md:83 (Segmentmarke der Laufzeit, nicht zu portieren)"),
    (0x28DB, 0x0000, 0x000F, "docs/analyse/BESTAND.md:84 (Segmentmarke der Laufzeit, nicht zu portieren)"),
    (0x2850, 0x0000, 0x089F, "docs/PORTIERUNGSPLAN.md:19, docs/analyse/BESTAND.md:357 (PC-Lautsprecher 2850 entfällt, Nutzer 11.11.2026)"),
    (0x23CD, 0x0797, None, "docs/PORTIERUNGSPLAN.md:19, docs/analyse/GRAFIK.md:53 (EGA-Teil des Treibers 23CD entfällt)"),
    (0x23CD, 0x1645, None, "docs/PORTIERUNGSPLAN.md:19, docs/analyse/GRAFIK.md:53 (Tandy-Teil des Treibers 23CD entfällt)"),
    (0x23CD, 0x3F0A, None, "docs/PORTIERUNGSPLAN.md:19, docs/analyse/GRAFIK.md:53 (Tandy-Emulation des Treibers 23CD entfällt)"),
    (0x186C, 0x0A6C, None, "docs/routines/186C_ressourcenlader.md:19 (Datei suchen/Diskette anfordern entfällt, R4 F2 = A)"),
    (0x186C, 0x0B63, None, "docs/routines/186C_ressourcenlader.md:19 (Datei suchen/Diskette anfordern entfällt, R4 F2 = A)"),
]
# The Turbo-C runtime segment 0000 also holds sound routines that are not runtime (BESTAND.md:40, "Zuordnung offen").
GREY_EXCEPT = [(0x0000, 0x06FE), (0x0000, 0x0724), (0x0000, 0x0750)]

# Yellow without address: (segment, first offset, last offset, proof)
YELLOW = [
    (0x2821, 0x0000, 0x02EF, "src/Traders.Assets/PowerPacker.cs (PP20 aus Formatwissen); docs/analyse/BESTAND.md:81 "
                             "„vorhanden, unbelegt“"),
]
# Handlers of the drawing command list: the port dispatches on their offsets (`case 0xOOOO`) in this file.
HANDLER_FILE = Path("src/Traders.Vga/Layers/CommandList.cs")


def handler_cases(repo: Path) -> list[tuple[int, str]]:
    lines = (repo / HANDLER_FILE).read_text(encoding="utf-8").splitlines()
    out = []
    inside = False
    for no, line in enumerate(lines, 1):
        if "void Execute(ushort segment, ushort offset)" in line:
            inside = True
        elif inside and line.startswith("    }"):
            break
        m = re.search(r"case 0x([0-9A-F]{4}):", line) if inside else None
        if m:
            out.append((int(m.group(1), 16), f"{HANDLER_FILE.as_posix()}:{no}"))
    return out


def colour_of(f: fk.Function, classes: dict[str, str], an: fk.Analyzer, yellow: dict[int, str]):
    """(colour, classes of its [APPROX] lines, proof or '')."""
    ks = sorted({classes.get(a, "") or "?" for a in f.approx})
    e_lines = sorted(a for a in f.approx if classes.get(a) == "E")
    if e_lines:
        return "blau", ks, ", ".join(e_lines)
    if f.status == "umgesetzt":
        return "grün", ks, ""
    if f.status == "genähert" and {"K", "D"} & set(ks):
        return "gelb", ks, ", ".join(sorted(f.approx))
    if f.status in ("dokumentiert", "unbekannt") and f.start in yellow:
        return "gelb", ks, yellow[f.start]
    seg, off = f.region.seg, f.offset
    if (seg, off) not in GREY_EXCEPT and not any(e[0] == seg and e[1] >= off and e[1] < off + f.size for e in GREY_EXCEPT):
        for gseg, lo, hi, proof in GREY:
            if gseg == seg and ((hi is None and off == lo) or (hi is not None and lo <= off <= hi)):
                return "grau", ks, proof
    return "rot", ks, ""


def build(exe: Path, repo: Path, with_texts: bool):
    an = fk.Analyzer(exe.read_bytes())
    an.run()
    rc = fk.RefCollector(an)
    rc.scan_code(repo)
    rc.scan_docs(repo)
    classes = {f"{p}:{n}": k for (p, n), k in ml.marker_classes(repo).items()}
    yellow: dict[int, str] = {}
    for seg, lo, hi, proof in YELLOW:
        for f in an.functions:
            if f.region.seg == seg and lo <= f.offset <= hi:
                yellow[f.start] = proof
    for off, where in handler_cases(repo):
        a = an.lin(0x0A96, off)
        f = an.func_at(a) if a is not None else None
        if f is not None and f.start == a:
            yellow[f.start] = f"{where} (Befehlsausführung über den Versatz, ohne Marker)"
    rows = []
    for f in an.functions:
        col, ks, proof = colour_of(f, classes, an, yellow)
        refs = [x for st in fk.STATUS_ORDER for x in f.refs.get(st, [])]
        rows.append({
            "a": f.address(), "f": f"0x{f.file_offset():05X}", "s": f.region.name, "g": f.region.group,
            "o": f.offset, "n": f.size, "st": f.status, "c": col, "k": "".join(k for k in ks if k != "?") or "–",
            "r": refs[:6], "nr": len(refs), "b": proof, "h": fk.hint(f, an, with_texts),
        })
    return an, rows


def summary(rows):
    tot_b = sum(r["n"] for r in rows)
    out = {}
    for c in COLOURS:
        rs = [r for r in rows if r["c"] == c]
        out[c] = {"funktionen": len(rs), "bytes": sum(r["n"] for r in rs),
                  "anteil_bytes": round(100 * sum(r["n"] for r in rs) / tot_b, 1),
                  "anteil_funktionen": round(100 * len(rs) / len(rows), 1)}
    return out, tot_b


PAGE = """<!DOCTYPE html>
<html lang="de"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>Abdeckung T.EXE</title>
<style>
:root{color-scheme:light;--bg:#fbfbfa;--card:#fff;--ink:#1d1f21;--ink2:#5b6168;--line:#e3e4e6;
--grün:#1b6e45;--gelb:#e3b230;--blau:#3a70d6;--grau:#a3a8ad;--rot:#f2735f}
*{box-sizing:border-box}body{margin:0;font:14px/1.45 system-ui,-apple-system,"Segoe UI",sans-serif;background:var(--bg);color:var(--ink)}
main{max-width:1280px;margin:0 auto;padding:24px 20px 60px}h1{font-size:22px;margin:0 0 4px}h2{font-size:17px;margin:28px 0 10px}
.meta{color:var(--ink2);margin:0 0 16px}.legend{display:grid;grid-template-columns:repeat(auto-fit,minmax(230px,1fr));gap:8px;margin:12px 0}
.legend div{background:var(--card);border:1px solid var(--line);border-radius:8px;padding:8px 10px;font-size:12.5px;color:var(--ink2)}
.legend b{color:var(--ink);display:flex;align-items:center;gap:6px;font-size:13.5px}
.sw{display:inline-block;width:12px;height:12px;border-radius:3px}
.bars{background:var(--card);border:1px solid var(--line);border-radius:8px;padding:12px 14px}
.bar{display:flex;height:22px;border-radius:4px;overflow:hidden;gap:2px;background:var(--card);margin:4px 0 2px}
.bar span{display:block;height:100%}.barlbl{display:flex;flex-wrap:wrap;gap:4px 16px;font-size:12.5px;color:var(--ink2);margin-bottom:8px}
.barlbl b{color:var(--ink)}.filters{display:flex;flex-wrap:wrap;gap:8px;margin:8px 0 12px;align-items:center}
.filters label{display:flex;align-items:center;gap:6px;background:var(--card);border:1px solid var(--line);border-radius:16px;padding:3px 10px;cursor:pointer;user-select:none}
.grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(380px,1fr));gap:12px}
.seg{background:var(--card);border:1px solid var(--line);border-radius:8px;padding:10px 12px}
.seg h3{margin:0;font-size:14px;display:flex;justify-content:space-between;gap:8px}.seg h3 small{color:var(--ink2);font-weight:400}
.seg .mini{display:flex;height:6px;gap:1px;margin:6px 0 8px;border-radius:2px;overflow:hidden}
svg rect.fn{cursor:pointer}svg rect.fn.dim{opacity:.12}svg rect.fn.hl{stroke:#1d1f21;stroke-width:1.5}
#tip{position:fixed;pointer-events:none;background:#1d1f21;color:#fff;border-radius:6px;padding:8px 10px;font-size:12px;max-width:420px;display:none;z-index:9;line-height:1.4}
#tip code{color:#fff}table{border-collapse:collapse;width:100%;background:var(--card);border:1px solid var(--line);border-radius:8px;overflow:hidden;font-size:12.5px}
th,td{text-align:left;padding:5px 8px;border-bottom:1px solid var(--line);vertical-align:top}th{background:#f3f4f5;font-weight:600}
td.num{text-align:right;font-variant-numeric:tabular-nums}code{font:12px ui-monospace,SFMono-Regular,Menlo,monospace}
details{margin-top:8px}summary{cursor:pointer;color:var(--ink2)}.note{color:var(--ink2);font-size:12.5px}
</style></head><body><main>
<h1>Abdeckung von <code>T.EXE</code> im Port</h1>
<p class="meta">Stand <code>__STAND__</code> · erzeugt __DATUM__ · __NF__ Funktionen, __NB__ Byte Code · Regeln: <code>docs/vorlagen/abdeckung-grafik.md</code></p>
<div class="legend" id="legend"></div>
<h2>Gesamt</h2>
<div class="bars"><div>nach Bytes</div><div class="bar" id="barB"></div><div class="barlbl" id="lblB"></div>
<div>nach Anzahl Funktionen</div><div class="bar" id="barN"></div><div class="barlbl" id="lblN"></div></div>
<h2>Karte je Segment</h2>
<p class="note">Je Segment in Adressreihenfolge, eine Zeile = __ROW__ Byte; jede Funktion ein Rechteck, Fläche nach Größe (lange Funktionen laufen über mehrere Zeilen). Grau ist schraffiert.</p>
<div class="filters" id="filters"></div>
<div class="grid" id="grid"></div>
<h2>Die 20 größten roten Funktionen</h2><div id="tRot"></div>
<h2>Die 20 größten gelben Funktionen</h2><div id="tGelb"></div>
<h2>Graue Funktionen mit Beleg</h2><div id="tGrau"></div>
<h2>Probe</h2><div id="probe"></div>
<div id="tip"></div>
<script>
const D=__DATA__;const C=["grün","gelb","blau","grau","rot"];const ROW=__ROW__;
const esc=s=>String(s).replace(/[&<>"]/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;"}[c]));
const col=c=>`var(--${c})`;const fill=c=>c==="grau"?"url(#hatch)":col(c);
document.getElementById("legend").innerHTML=C.map(c=>`<div><b><span class="sw" style="background:${col(c)}"></span>${c}</b>${esc(D.rules[c])}</div>`).join("");
function bar(id,lid,key,pk,unit){document.getElementById(id).innerHTML=C.map(c=>`<span title="${c}" style="width:${D.sum[c][pk]}%;background:${col(c)}"></span>`).join("");
document.getElementById(lid).innerHTML=C.map(c=>`<span><span class="sw" style="background:${col(c)}"></span> ${c} <b>${D.sum[c][pk].toLocaleString("de-DE")} %</b> (${D.sum[c][key].toLocaleString("de-DE")} ${unit})</span>`).join("");}
bar("barB","lblB","bytes","anteil_bytes","Byte");bar("barN","lblN","funktionen","anteil_funktionen","Funktionen");
const on=new Set(C);document.getElementById("filters").innerHTML="Filter: "+C.map(c=>`<label><input type="checkbox" data-c="${c}" checked><span class="sw" style="background:${col(c)}"></span>${c}</label>`).join("");
document.querySelectorAll("#filters input").forEach(i=>i.addEventListener("change",()=>{i.checked?on.add(i.dataset.c):on.delete(i.dataset.c);
document.querySelectorAll("rect.fn").forEach(r=>r.classList.toggle("dim",!on.has(D.rows[r.dataset.i].c)));}));
const tip=document.getElementById("tip");const grid=document.getElementById("grid");const W=356,H=9,G=1;
for(const s of D.segs){const rs=D.rows.map((r,i)=>[r,i]).filter(([r])=>r.s===s.s);const rows=Math.ceil(s.size/ROW);
let rects="";for(const [r,i] of rs){let a=r.o,e=r.o+r.n;while(a<e){const row=Math.floor(a/ROW),x0=a-row*ROW,x1=Math.min(e-row*ROW,ROW);
rects+=`<rect class="fn" data-i="${i}" x="${(x0/ROW*W).toFixed(2)}" y="${row*(H+G)}" width="${Math.max(((x1-x0)/ROW*W)-0.6,0.6).toFixed(2)}" height="${H}" fill="${fill(r.c)}"/>`;a=row*ROW+x1;}}
const by={};for(const c of C)by[c]=0;for(const [r] of rs)by[r.c]+=r.n;
grid.insertAdjacentHTML("beforeend",`<div class="seg"><h3><span><code>${s.s}</code> ${esc(s.g)}</span><small>0x${s.size.toString(16).toUpperCase()} Byte · ${rs.length} Fkt.</small></h3>
<div class="mini">${C.filter(c=>by[c]).map(c=>`<span style="flex:${by[c]};background:${col(c)}"></span>`).join("")}</div>
<svg width="${W}" height="${rows*(H+G)}" viewBox="0 0 ${W} ${rows*(H+G)}"><defs><pattern id="hatch" width="4" height="4" patternUnits="userSpaceOnUse" patternTransform="rotate(45)"><rect width="4" height="4" fill="var(--grau)"/><line x1="0" y1="0" x2="0" y2="4" stroke="#fff" stroke-width="1.2"/></pattern></defs>${rects}</svg></div>`);}
grid.addEventListener("mousemove",ev=>{const t=ev.target;document.querySelectorAll("rect.hl").forEach(x=>x.classList.remove("hl"));
if(!t.classList||!t.classList.contains("fn")){tip.style.display="none";return;}const r=D.rows[t.dataset.i];
document.querySelectorAll(`rect.fn[data-i="${t.dataset.i}"]`).forEach(x=>x.classList.add("hl"));
tip.innerHTML=`<b><code>${r.a}</code></b> · Datei ${r.f} · ${r.n} Byte<br>Farbe <b>${r.c}</b> · Status ${r.st} · Klasse ${r.k}<br>`+
(r.b?`Beleg: ${esc(r.b)}<br>`:"")+(r.r.length?`Fundstellen (${r.nr}): ${r.r.map(esc).join(", ")}${r.nr>r.r.length?" …":""}<br>`:"Fundstellen: keine<br>")+`Hinweis: ${esc(r.h)}`;
tip.style.display="block";const x=Math.min(ev.clientX+14,innerWidth-440),y=Math.min(ev.clientY+14,innerHeight-tip.offsetHeight-8);tip.style.left=x+"px";tip.style.top=y+"px";});
grid.addEventListener("mouseleave",()=>{tip.style.display="none";});
function table(id,c,n,withProof){const rs=D.rows.filter(r=>r.c===c).sort((a,b)=>b.n-a.n).slice(0,n||1e9);
document.getElementById(id).innerHTML=rs.length?`<table><tr><th>Adresse</th><th>Dateioffset</th><th class="num">Größe</th><th>Segment</th><th>Status</th><th>${withProof?"Beleg / Nachweis":"Hinweis"}</th></tr>`+
rs.map(r=>`<tr><td><code>${r.a}</code></td><td><code>${r.f}</code></td><td class="num">${r.n}</td><td>${r.s} ${esc(r.g)}</td><td>${r.st}</td><td>${esc(withProof?r.b:r.h)}</td></tr>`).join("")+"</table>":"<p class='note'>keine</p>";}
table("tRot","rot",20,false);table("tGelb","gelb",20,true);
(function(){const rs=D.rows.filter(r=>r.c==="grau");const g={};for(const r of rs){(g[r.b]=g[r.b]||[]).push(r);}
document.getElementById("tGrau").innerHTML=`<table><tr><th>Beleg</th><th class="num">Funktionen</th><th class="num">Bytes</th><th>Funktionen</th></tr>`+
Object.entries(g).map(([b,l])=>`<tr><td>${esc(b)}</td><td class="num">${l.length}</td><td class="num">${l.reduce((s,r)=>s+r.n,0)}</td><td><details><summary>${l.length} Adressen</summary>${l.map(r=>`<code>${r.a}</code> (${r.n})`).join(", ")}</details></td></tr>`).join("")+"</table>";})();
document.getElementById("probe").innerHTML=D.probe;
</script></main></body></html>
"""


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    ap.add_argument("exe", type=Path)
    ap.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    ap.add_argument("--html", type=Path, required=True)
    ap.add_argument("--json", type=Path)
    ap.add_argument("--ohne-texte", action="store_true")
    ap.add_argument("--stand", default="")
    ap.add_argument("--row", type=int, default=512)
    ap.add_argument("--probe", default="", help="HTML snippet with the result of the check (step 5)")
    args = ap.parse_args()
    repo = args.repo.resolve()
    an, rows = build(args.exe, repo, not args.ohne_texte)
    summ, tot = summary(rows)
    data = {"rules": RULES, "sum": summ, "rows": rows,
            "segs": [{"s": r.name, "g": r.group, "size": r.size} for r in an.regions], "probe": args.probe}
    page = (PAGE.replace("__DATA__", json.dumps(data, ensure_ascii=False))
            .replace("__STAND__", html.escape(args.stand)).replace("__DATUM__", datetime.date.today().isoformat())
            .replace("__NF__", str(len(rows))).replace("__NB__", f"{tot:,}".replace(",", "."))
            .replace("__ROW__", str(args.row)))
    args.html.write_text(page, encoding="utf-8")
    if args.json:
        args.json.write_text(json.dumps({"summe": summ, "bytes": tot, "funktionen": rows}, ensure_ascii=False, indent=1),
                             encoding="utf-8")
    for c in COLOURS:
        s = summ[c]
        print(f"{c:5} {s['funktionen']:5} Fkt. {s['bytes']:7} Byte {s['anteil_bytes']:5.1f} %")
    print(f"Summe {len(rows)} Fkt. {tot} Byte")
    return 0


if __name__ == "__main__":
    sys.exit(main())
