#!/usr/bin/env python3
"""Collect the latest test report from each suite into one Pages site.

Run as: collect_reports.py <output-dir>

Reads GH_TOKEN and GH_REPO from the environment. For every suite in SUITES it
finds the most recent artifact of the given name, downloads it, and either
unpacks it as a ready-made HTML report or renders Allure results into one.

A suite that has never uploaded an artifact is reported as absent and skipped.
The point of the site is to show what the suites currently say, so one missing
suite must not take the whole publish down with it.
"""
import json
import os
import subprocess
import sys
import urllib.error
import urllib.request
import zipfile
from pathlib import Path

API = 'https://api.github.com'

# kind: 'playwright' unpacks a directory that already contains index.html.
#       'allure' unpacks raw results and renders them with the Allure CLI.
SUITES = [
    dict(slug='the-internet', title='the-internet UI',
         stack='TypeScript', artifact='playwright-report-the-internet',
         kind='playwright'),
    dict(slug='uitestingplayground', title='UI Test Automation Playground',
         stack='TypeScript', artifact='playwright-report-uitestingplayground',
         kind='playwright'),
    dict(slug='restful-booker-api', title='Restful Booker Platform - API',
         stack='TypeScript, Zod', artifact='playwright-report-restful-booker-api',
         kind='playwright'),
    dict(slug='restful-booker-hybrid', title='Restful Booker Platform - Hybrid',
         stack='TypeScript, Zod', artifact='playwright-report-restful-booker-hybrid',
         kind='playwright'),
    dict(slug='typescript-playwright', title='TypeScript POM Reference',
         stack='TypeScript', artifact='playwright-report-ts',
         kind='playwright'),
    dict(slug='automationexercise-ui', title='Automation Exercise - UI',
         stack='C#, NUnit', artifact='allure-results-automationexercise-ui',
         kind='allure'),
    dict(slug='automationexercise-api', title='Automation Exercise - API',
         stack='C#, NUnit', artifact='allure-results-automationexercise-api',
         kind='allure'),
    dict(slug='expandtesting-api', title='Expand Testing Notes API',
         stack='C#, NUnit', artifact='allure-results-expandtesting',
         kind='allure'),
    dict(slug='petstore-rest-assured', title='Petstore - RestAssured',
         stack='Java 17, JUnit 5', artifact='allure-results-petstore-rest-assured',
         kind='allure'),
]


def api_get(path):
    req = urllib.request.Request(
        API + path,
        headers={'Authorization': 'Bearer ' + os.environ['GH_TOKEN'],
                 'Accept': 'application/vnd.github+json',
                 'User-Agent': 'collect-reports'})
    with urllib.request.urlopen(req, timeout=60) as r:
        return json.load(r)


def latest_artifacts():
    """Newest artifact per name, across all workflows."""
    found = {}
    page = 1
    repo = os.environ['GH_REPO']
    while page <= 10:
        data = api_get('/repos/%s/actions/artifacts?per_page=100&page=%d'
                       % (repo, page))
        items = data.get('artifacts', [])
        if not items:
            break
        for a in items:
            # The listing is newest-first, so the first sighting of a name wins.
            if a['name'] not in found and not a.get('expired'):
                found[a['name']] = a
        if len(items) < 100:
            break
        page += 1
    return found


def download(artifact, dest_zip):
    req = urllib.request.Request(
        artifact['archive_download_url'],
        headers={'Authorization': 'Bearer ' + os.environ['GH_TOKEN'],
                 'User-Agent': 'collect-reports'})
    with urllib.request.urlopen(req, timeout=300) as r:
        with open(dest_zip, 'wb') as f:
            while True:
                chunk = r.read(1 << 20)
                if not chunk:
                    break
                f.write(chunk)


def render_allure(results_dir, out_dir):
    subprocess.run(['allure', 'generate', str(results_dir),
                    '-o', str(out_dir), '--clean'],
                   check=True, stdout=subprocess.DEVNULL)


def collect(out, available):
    tmp = out / '_tmp'
    tmp.mkdir(parents=True, exist_ok=True)
    published = []

    for suite in SUITES:
        art = available.get(suite['artifact'])
        if art is None:
            print('  SKIP %-28s no artifact named %r'
                  % (suite['slug'], suite['artifact']))
            published.append(dict(suite, status='absent'))
            continue

        zip_path = tmp / (suite['slug'] + '.zip')
        raw = tmp / suite['slug']
        target = out / suite['slug']
        try:
            download(art, zip_path)
            with zipfile.ZipFile(zip_path) as z:
                z.extractall(raw)

            if suite['kind'] == 'allure':
                if not any(raw.iterdir()):
                    raise RuntimeError('allure results were empty')
                render_allure(raw, target)
            else:
                if not (raw / 'index.html').exists():
                    raise RuntimeError('no index.html in the report')
                raw.rename(target)

            published.append(dict(suite, status='ok',
                                  updated=art.get('updated_at', '')))
            print('  OK   %-28s %s' % (suite['slug'], art.get('updated_at', '')))
        except (urllib.error.URLError, zipfile.BadZipFile, RuntimeError,
                subprocess.CalledProcessError, OSError) as exc:
            # One unusable report must not sink the whole site.
            print('  FAIL %-28s %s' % (suite['slug'], exc))
            published.append(dict(suite, status='error', detail=str(exc)))

    return published


def write_index(out, suites):
    rows = []
    for s in suites:
        if s['status'] == 'ok':
            when = (s.get('updated') or '').replace('T', ' ').replace('Z', ' UTC')
            rows.append(
                '<tr><td><a href="{slug}/index.html">{title}</a></td>'
                '<td>{stack}</td><td><span class="ok">published</span></td>'
                '<td class="when">{when}</td></tr>'.format(when=when, **s))
        else:
            label = 'no run yet' if s['status'] == 'absent' else 'unavailable'
            rows.append(
                '<tr><td>{title}</td><td>{stack}</td>'
                '<td><span class="off">{label}</span></td>'
                '<td class="when">-</td></tr>'.format(label=label, **s))

    ok = sum(1 for s in suites if s['status'] == 'ok')
    html = (TEMPLATE.replace('{{ROWS}}', '\n'.join(rows))
                    .replace('{{OK}}', str(ok))
                    .replace('{{TOTAL}}', str(len(suites))))
    (out / 'index.html').write_text(html, encoding='utf-8')
    # Pages runs Jekyll by default, which strips files and folders whose names
    # begin with an underscore - Playwright and Allure both emit those.
    (out / '.nojekyll').write_text('', encoding='utf-8')


TEMPLATE = """<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Test Reports - AutomationTestingProject</title>
<style>
  :root {
    --bg: #ffffff; --fg: #1c1e21; --muted: #656d76;
    --line: #d8dee4; --accent: #0969da;
    --ok-bg: #dafbe1; --ok-fg: #116329;
    --off-bg: #f6f8fa; --off-fg: #656d76;
  }
  @media (prefers-color-scheme: dark) {
    :root {
      --bg: #0d1117; --fg: #e6edf3; --muted: #9198a1;
      --line: #30363d; --accent: #4493f8;
      --ok-bg: #12261e; --ok-fg: #3fb950;
      --off-bg: #161b22; --off-fg: #9198a1;
    }
  }
  * { box-sizing: border-box; }
  body {
    margin: 0; padding: 2.5rem 1.25rem; background: var(--bg); color: var(--fg);
    font: 16px/1.55 -apple-system, BlinkMacSystemFont, "Segoe UI", Helvetica, Arial, sans-serif;
  }
  main { max-width: 60rem; margin: 0 auto; }
  h1 { font-size: 1.6rem; margin: 0 0 .35rem; letter-spacing: -.02em; }
  .sub { color: var(--muted); margin: 0 0 2rem; font-size: .95rem; }
  .wrap { overflow-x: auto; border: 1px solid var(--line); border-radius: 8px; }
  table { border-collapse: collapse; width: 100%; min-width: 34rem; }
  th, td { text-align: left; padding: .7rem .9rem; border-bottom: 1px solid var(--line); }
  th { font-size: .74rem; text-transform: uppercase; letter-spacing: .05em;
       color: var(--muted); font-weight: 600; background: var(--off-bg); }
  tr:last-child td { border-bottom: 0; }
  a { color: var(--accent); text-decoration: none; font-weight: 500; }
  a:hover { text-decoration: underline; }
  td.when { color: var(--muted); font-size: .85rem; white-space: nowrap; }
  .ok, .off { font-size: .78rem; padding: .12rem .5rem; border-radius: 2rem;
              display: inline-block; }
  .ok { background: var(--ok-bg); color: var(--ok-fg); }
  .off { background: var(--off-bg); color: var(--off-fg); }
  footer { margin-top: 2rem; color: var(--muted); font-size: .85rem; }
  footer a { font-weight: 400; }
</style>
</head>
<body>
<main>
  <h1>Test Reports</h1>
  <p class="sub">Latest report from each suite in
    <a href="https://github.com/ValBo71/AutomationTestingProject">AutomationTestingProject</a>
    - {{OK}} of {{TOTAL}} published.</p>
  <div class="wrap">
    <table>
      <thead><tr><th>Suite</th><th>Stack</th><th>Report</th><th>Run</th></tr></thead>
      <tbody>
{{ROWS}}
      </tbody>
    </table>
  </div>
  <footer>
    Rebuilt after each suite finishes. A suite marked <em>no run yet</em> has not
    uploaded a report since artifacts were last retained.
  </footer>
</main>
</body>
</html>
"""


def main():
    out = Path(sys.argv[1] if len(sys.argv) > 1 else 'site')
    out.mkdir(parents=True, exist_ok=True)

    available = latest_artifacts()
    print('%d artifact names visible' % len(available))

    published = collect(out, available)
    write_index(out, published)

    tmp = out / '_tmp'
    for p in sorted(tmp.rglob('*'), reverse=True):
        p.unlink() if p.is_file() else p.rmdir()
    if tmp.exists():
        tmp.rmdir()

    ok = sum(1 for s in published if s['status'] == 'ok')
    print('%d of %d suites published' % (ok, len(SUITES)))


if __name__ == '__main__':
    main()
