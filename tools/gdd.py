#!/usr/bin/env python3
"""GDD 문서 보조 도구.

사용법 (저장소 루트에서):
  python tools/gdd.py status
      99번 문서의 남은 항목(D·B·C)을 한 줄씩 출력합니다.
  python tools/gdd.py decide D9 --title "모병소 갱신" --before "정의 없음" --after "결정 내용"
      99번 문서에서 D9 행을 지우고, 98번 문서 §1 표 맨 아래에 결정 행을 추가합니다(오늘 날짜 자동).
  python tools/gdd.py links
      docs/GDD, CLAUDE.md, progress.md의 상대 링크와 #앵커가 실제로 있는지 검사합니다.

파일의 줄바꿈(CRLF/LF)은 원래대로 유지합니다.
"""
import argparse
import datetime
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
GDD = ROOT / "docs" / "GDD"
TODO = GDD / "99_review_notes.md"
LOG = GDD / "98_decision_log.md"
LOG_TABLE_HEADING = "## 1. 바뀐 내용 (결정 기록)"

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")


def read(path):
    raw = path.read_text(encoding="utf-8")
    eol = "\r\n" if "\r\n" in raw else "\n"
    return raw.replace("\r\n", "\n"), eol


def write(path, text, eol):
    path.write_text(text.replace("\n", eol), encoding="utf-8", newline="")


def cells(line):
    return [c.strip() for c in line.strip().strip("|").split("|")]


# ---------- status ----------
def cmd_status(_):
    text, _ = read(TODO)
    section = ""
    for line in text.split("\n"):
        if line.startswith("#"):
            section = line.lstrip("#").strip()
            continue
        m = re.match(r"\|\s*([DBC]\d+)\s*\|", line)
        if not m:
            continue
        c = cells(line)
        # D: | # | 챕터 | 항목 | 내용 |  B: | # | 챕터 | 항목 |  C: | # | 항목 | 할 일 |  D16~: | # | 항목 | 내용 |
        if c[0].startswith("D") and len(c) >= 4 and c[1].startswith(("[", "—")):
            title = c[2]
        elif c[0].startswith("B"):
            title = c[2]
        else:
            title = c[1]
        if len(title) > 40:
            title = title[:40] + "…"
        print(f"{c[0]:4} {title}   ({section})")


# ---------- decide ----------
def cmd_decide(a):
    item = a.id.upper()
    todo, eol_t = read(TODO)
    lines = todo.split("\n")
    hits = [i for i, l in enumerate(lines) if re.match(rf"\|\s*{re.escape(item)}\s*\|", l)]
    if len(hits) != 1:
        sys.exit(f"99번 문서에서 {item} 행을 {len(hits)}개 찾았습니다. 1개여야 합니다.")
    removed = lines.pop(hits[0])

    log, eol_l = read(LOG)
    loglines = log.split("\n")
    try:
        start = loglines.index(LOG_TABLE_HEADING)
    except ValueError:
        sys.exit(f"98번 문서에서 '{LOG_TABLE_HEADING}' 제목을 찾지 못했습니다.")
    i = start + 1
    while i < len(loglines) and not loglines[i].startswith("|"):
        i += 1
    while i < len(loglines) and loglines[i].startswith("|"):
        i += 1
    date = a.date or datetime.date.today().isoformat()
    row = f"| {a.title} ({item}) | {a.before} | {a.after} ({date}) |"
    loglines.insert(i, row)

    if a.dry_run:
        print("[dry-run] 99에서 지울 행:\n  " + removed + "\n[dry-run] 98에 추가할 행:\n  " + row)
        return
    write(TODO, "\n".join(lines), eol_t)
    write(LOG, "\n".join(loglines), eol_l)
    print(f"99에서 {item} 삭제, 98 §1에 기록 추가 ({date})")


# ---------- links ----------
def slug(heading):
    s = heading.strip().lower()
    s = re.sub(r"[^\w\- ]", "", s)  # 유니코드 글자·숫자·_·-·공백만 남김 (GitHub 방식)
    return s.replace(" ", "-")


def anchors(path):
    text, _ = read(path)
    seen, result = {}, set()
    in_code = False
    for line in text.split("\n"):
        if line.startswith("```"):
            in_code = not in_code
        if in_code:
            continue
        m = re.match(r"^#{1,6}\s+(.*)$", line)
        if m:
            base = slug(m.group(1))
            n = seen.get(base, 0)
            result.add(base if n == 0 else f"{base}-{n}")
            seen[base] = n + 1
    return result


def cmd_links(_):
    files = sorted(GDD.glob("*.md")) + [ROOT / "CLAUDE.md", ROOT / "progress.md"]
    cache, bad = {}, 0
    for f in files:
        if not f.exists():
            continue
        text, _ = read(f)
        for n, line in enumerate(text.split("\n"), 1):
            for target in re.findall(r"\]\(([^)\s]+)\)", line):
                if re.match(r"^[a-z]+:", target):
                    continue
                path_part, _, frag = target.partition("#")
                dest = (f.parent / path_part).resolve() if path_part else f
                if not dest.exists():
                    print(f"{f.relative_to(ROOT)}:{n}  파일 없음 → {target}")
                    bad += 1
                    continue
                if frag and dest.suffix == ".md":
                    if dest not in cache:
                        cache[dest] = anchors(dest)
                    if frag.lower() not in cache[dest]:
                        print(f"{f.relative_to(ROOT)}:{n}  앵커 없음 → {target}")
                        bad += 1
    print(f"문제 {bad}개" if bad else "링크 문제 없음")
    sys.exit(1 if bad else 0)


def main():
    p = argparse.ArgumentParser(description="GDD 문서 보조 도구")
    sub = p.add_subparsers(dest="cmd", required=True)
    sub.add_parser("status").set_defaults(fn=cmd_status)
    d = sub.add_parser("decide")
    d.add_argument("id")
    d.add_argument("--title", required=True, help="98번 기록의 항목 이름")
    d.add_argument("--before", required=True, help="이전 상태")
    d.add_argument("--after", required=True, help="결정 내용")
    d.add_argument("--date", help="기본값: 오늘")
    d.add_argument("--dry-run", action="store_true")
    d.set_defaults(fn=cmd_decide)
    sub.add_parser("links").set_defaults(fn=cmd_links)
    a = p.parse_args()
    a.fn(a)


if __name__ == "__main__":
    main()
