#!/usr/bin/env python3
"""Generate source-derived UML inventories for every maintained src project."""
from __future__ import annotations
import json, os, subprocess, sys, xml.etree.ElementTree as ET
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
SOLUTION=ROOT/"TheSingularityWorkshop.GUI.slnx"
GENERATOR=ROOT/"samples/SourceUmlGenerator/SourceUmlGenerator.csproj"
OUTPUT=ROOT/"artifacts/solution-uml"
REPOSITORY="TrentBest/TheSingularityWorkshop.GUI"
# Product libraries only. Tests, samples, docs and tooling are deliberately excluded.
MAINTAINED_PROJECTS=("src/Core/TheSingularityWorkshop.GUI.Core.csproj","src/Blazor/TheSingularityWorkshop.GUI.Blazor.csproj","src/WPF/TheSingularityWorkshop.GUI.WPF.csproj","src/SVG/TheSingularityWorkshop.GUI.SVG.csproj")
def main()->int:
 revision=os.environ.get("SOURCE_REVISION","").strip()
 if not revision:
  print("SOURCE_REVISION must contain the exact source commit SHA.",file=sys.stderr); return 2
 try: root=ET.parse(SOLUTION).getroot()
 except (ET.ParseError,OSError) as exc:
  print(f"Cannot read solution manifest: {exc}",file=sys.stderr); return 2
 solution={Path(n.attrib["Path"].replace("\\","/")).as_posix() for n in root.iter("Project") if "Path" in n.attrib}
 discovered={p.relative_to(ROOT).as_posix() for p in (ROOT/"src").rglob("*.csproj") if not any(x in {"bin","obj"} for x in p.parts)}
 expected=set(MAINTAINED_PROJECTS)
 errors=([f"Maintained product project is absent from solution: {p}" for p in sorted(expected-solution)]
 +[f"Maintained product project is missing from checkout: {p}" for p in sorted(expected-discovered)]
 +[f"New src project needs explicit UML scope classification: {p}" for p in sorted(discovered-expected)])
 if errors:
  print("Solution UML scope validation failed:\n"+"\n".join("  - "+e for e in errors),file=sys.stderr); return 1
 OUTPUT.mkdir(parents=True,exist_ok=True); rows=[]
 for project in MAINTAINED_PROJECTS:
  path=ROOT/project; slug=path.parent.name.lower(); dest=OUTPUT/slug; dest.mkdir(parents=True,exist_ok=True)
  model_path,inventory_path,pages_path=dest/"model.json",dest/"inventory.md",dest/"types"
  command=["dotnet","run","--project",str(GENERATOR),"--configuration","Release","--","--project-model-json",str(model_path),"--project",str(path),"--repository",REPOSITORY,"--revision",revision,"--configuration","Release","--inventory-markdown",str(inventory_path),"--type-pages-dir",str(pages_path)]
  print("Analyzing maintained project: "+project,flush=True)
  result=subprocess.run(command,cwd=ROOT,check=False)
  if result.returncode:
   print(f"UML generation failed for {project} (exit {result.returncode}).",file=sys.stderr); return result.returncode or 1
  try: model=json.loads(model_path.read_text(encoding="utf-8"))
  except (OSError,json.JSONDecodeError) as exc:
   print(f"Invalid generated model for {project}: {exc}",file=sys.stderr); return 1
  types=model.get("types"); pages=sorted(p for p in pages_path.glob("*.md") if p.name!="index.md")
  if not isinstance(types,list) or not types or len(pages)!=len(types) or not (pages_path/"index.md").is_file():
   print(f"Type-page coverage mismatch for {project}: {len(types or [])} types, {len(pages)} pages",file=sys.stderr); return 1
  provenance=model.get("provenance",{})
  if provenance.get("sourceRevision")!=revision or provenance.get("projectPath")!=project:
   print(f"Model provenance mismatch for {project}: {provenance}",file=sys.stderr); return 1
  rows.append((project,slug,len(types),len(model.get("relationships",[]))))
 lines=["# Solution-level source-derived UML inventory","",f"- Repository: `{REPOSITORY}`",f"- Source revision: `{revision}`","- Scope: maintained product projects under `src/`; tests, samples, tooling, and documentation projects are deliberately excluded.","- Evidence: each project is loaded with MSBuild/Roslyn and fails generation on compiler errors; per-type pages are checked against model type counts.","- Limitation: this is project coverage, not member-level reflection parity or full-solution graph rendering.","","| Project | Types | Compiler-symbol relationships | Inventory | Type pages |","|---|---:|---:|---|---|"]
 for project,slug,count,edges in rows: lines.append(f"| `{project}` | {count} | {edges} | [{slug}/inventory.md]({slug}/inventory.md) | [{slug}/types/index.md]({slug}/types/index.md) |")
 lines.extend(["",f"**Coverage:** {len(rows)}/{len(MAINTAINED_PROJECTS)} maintained product projects analyzed successfully.",""])
 (OUTPUT/"index.md").write_text("\n".join(lines),encoding="utf-8")
 print(f"Verified source-derived UML coverage for {len(rows)} maintained product projects -> {OUTPUT/'index.md'}"); return 0
if __name__=="__main__": raise SystemExit(main())
