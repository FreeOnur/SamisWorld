# Samis World – Art Style Guide (v1 · 27.10.2026)

Gefunden in einer Stilfindung mit Bildvergleichen (Runden 1–11). Die Bilder im Ordner Referenzbilder dienen nur zur Orientierung, sie sind nicht final und die Figuren darin sind Platzhalter.

## Der Stil in einem Satz

2D-Pixel-Art mit Kino-Licht: eine scharfe, schnelle Spielebene wie Katana Zero, Stimmung und Tiefe wie REPLACED. Sami ist grau-schwarz und gesichtslos in einer farbigen Welt.

- **Wichtigste Regel (Tie-Breaker):** Lesbarkeit im Kampf schlägt Schönheit. Wenn ein Effekt, eine Farbe oder ein Detail den Kampf schwerer lesbar macht, fliegt er raus oder wird zurückgenommen.
- **Kern-Gefühl:** Düster & melancholisch, mystisch & episch, brutal & intensiv, ruhig & poetisch. Die Spiel-Momente wechseln zwischen stiller Schönheit und harten, schnellen Kämpfen.
- **Vorbilder:** Katana Zero (schneller 2D-Kampf, Effekte geben Tiefe), REPLACED (Kino-Licht, Atmosphäre, Gegner aus dem Hintergrund), Ghost of Tsushima / Sekiro (Samurai-Stimmung, Wind, Blätter), Demon Slayer (schnelle Schwertkämpfe, farbige Technik-Hiebe), Undertale (ikonische, simple Figuren mit Charme).

## Grundsatz-Entscheidung: 2D jetzt, 2.5D in Teil 2

- Samis World Teil 1 ist ein 2D-Seiten-Scroller. Die Spielebene (Sami, Gegner, Boden, Plattformen, Gefahren) ist immer flach, scharf und sofort lesbar.
- Die Tiefe kommt aus allem drumherum: Parallax-Ebenen, HD-Licht, Nebel, Lichtstrahlen, Unschärfe im Vorder- und Hintergrund, Spiegelungen, Partikel.
- Gegner dürfen aus dem Hintergrund auf die Spielebene springen. Sami wechselt nur an festen Stellen die Ebene (Türen, Brücken, Gänge), so wie in REPLACED.
- Hintergründe werden von Anfang an in sauberen Ebenen gebaut, damit Teil 2 später auf 2.5D umsteigen kann (Diorama mit Perspektiv-Kamera oder HD-2D mit 3D-Welt und Pixel-Figuren), ohne die Art neu zu machen.
- **Teil 2 (Zukunft):** 2.5D im REPLACED- / HD-2D-Stil mit schnellem Kampf. Kurze 3D-Kampfszenen sind dort eine Option.

## Technik & Auflösung


| Punkt | Festlegung |
|---|---|
| Engine / Pipeline | Unity mit URP und 2D Renderer (nötig für 2D-Lights und Normal Maps). Das alte Projekt nutzt noch Built-in. |
| Spielauflösung | 480×270 (×4 = 1920×1080) bei Sami 48 px. Falls Sami 32 px wird: 320×180 (×6). Wird vor der ersten Asset-Produktion fix entschieden. |
| Tiles | 16×16 px, Pixels Per Unit (PPU) = 16 für ALLE Assets (1 Tile = 1 Unity-Einheit). |
| Sami-Größe | Offen: 32 oder 48 px hoch. Entscheidung über einen Animationstest (eine Angriffs-Combo in beiden Größen). |
| Sprite-Leinwand | Figuren auf 2er-Potenz-Leinwand (32×32, 64×64, 128×128 für Angriffe mit Hieb). Die Figur selbst muss keine 2er-Potenz sein. |
| Import | Filter Mode Point, Compression None, keine Mip Maps, Pivot in Pixeln (unten Mitte, gerade Werte). |
| Kamera | Pixel Perfect Camera (URP-Version), Grid Snapping = Pixel Snapping, KEIN Upscale Render Texture, damit Licht und Post-Processing in voller Auflösung laufen. Crop Frame = Windowbox. Anti-Aliasing aus. |
| Kamerafahrten | Cinemachine mit Pixel-Perfect-Extension. Zooms in ganzzahligen Stufen. Kurze Übergänge dürfen kurz nicht pixel-perfekt sein. |
| Pixel vs. HD | Alle Sprites, Tiles und Partikel liegen auf dem Pixel-Raster. Weich und in HD dürfen nur sein: Licht, Glow, Bloom, Nebel, Lichtstrahlen, Unschärfe, Vignette, Farbkorrektur. |


## Ebenen & Tiefe


| Ebene | Was liegt dort | Regeln |
|---|---|---|
| Vordergrund (FG) | Gräser, Äste, Pfosten, Blüten vor der Kamera | Sehr dunkel, stark unscharf, nur am Bildrand, nie über der Spielebene in Kämpfen |
| Spielebene | Sami, Gegner, Boden, Plattformen, Gefahren, Items | Immer scharf, voller Kontrast, kein Nebel, keine Unschärfe |
| Nah-Hintergrund | Bäume, Laternen, Ruinen direkt dahinter | Leicht unscharf, leichter Nebel, dunkler als die Spielebene |
| Mitte | Torii, Tempel, Hügel | Mehr Nebel, weniger Kontrast, Farbe geht Richtung Himmelfarbe |
| Fern | Berge, Silhouetten | Stark aufgehellt bzw. in Himmelfarbe getaucht, kaum Details |
| Himmel | Verlauf in Bändern, Sonne/Mond, Wolken | Bänder mit Dither-Übergang, Lichtquelle bestimmt die Szene |

- Atmosphärische Perspektive: Je weiter weg, desto weniger Sättigung, weniger Kontrast und desto näher an der Himmel-/Nebelfarbe.
- Gegner im Hintergrund werden mit Nebelfarbe eingefärbt und leicht verkleinert, bevor sie auf die Spielebene springen.
- Nasser Boden mit schwacher Spiegelung ist ein Standard-Werkzeug für Stimmung.

## Licht

- Jede Szene hat eine klare Hauptlichtquelle (Sonne, Mond, Laterne, Glut, Blitz). Ihre Richtung ist in der Szene überall gleich.
- Figuren werden vom Licht angestrahlt (wie in REPLACED): 2D-Lights in URP plus Normal Map pro Sprite. Die beleuchtete Seite zeigt zur Lichtquelle, der Rest bleibt dunkel.
- Randlicht (Rim Light) in der Lichtfarbe der Szene trennt Figuren vom Hintergrund. Pflicht für Sami, weil er sonst im Dunkeln verschwindet.
- Warm/kalt-Kontrast ist erwünscht: kaltes Mondlicht plus warme Laternen, warmes Gegenlicht plus kühle Schatten.
- Lichtstrahlen, Glow und Bloom sparsam, nie so stark, dass Gegner-Angriffe darin verschwinden.

| Licht-Stimmung | Wirkung | Typisch für |
|---|---|---|
| Dämmerung | Warmes Gegenlicht, Lichtstrahlen, Blüten | Biom 1, Rückblenden |
| Mondnacht + Laternen | Kaltes Mondlicht, warme Lichtinseln | Biom 3, Dörfer, ruhige Momente |
| Dichter Nebel | Fast farblos, Silhouetten, unheimlich | Biom 2, Geister, Überraschungen |
| Gewitter / Regen | Dunkel, Regen, Blitz-Randlicht, nasser Boden | Bosskämpfe, Wendepunkte der Story |
| Glut | Rotes Licht von unten, Asche, Funken | Biom 4 |
| Leere | Fast schwarz-weiß, harte Lichtkante | Biom 5, Finale |


## Farbe

- Mittlere Sättigung insgesamt: farbig, aber nicht knallig.
- Jedes Biom hat eine eigene Farbwelt, gern sehr unterschiedlich (bewusst mehr Unterschied statt weniger). Zusammengehalten wird das Spiel durch Sami, die Licht-Technik und die Ebenen-Regeln.
- Sami ist die einzige Figur ohne Farbe. Er muss in jeder Farbwelt sofort zu erkennen sein.
- Jeder Boss hat eine Signaturfarbe, die nur er und seine Angriffe tragen. Diese Farbe kommt im eigenen Biom sonst kaum vor.
- Rot für Blut und Gefahr ist im ganzen Spiel reserviert. Weiß ist reserviert für Sami (Hutrand, Hieb) und Treffer-Blitze.
- Schatten werden dunkler und kühler, nicht grauer. Lichter werden wärmer (Hue Shifting).
Die genauen Paletten (Hex-Werte) stehen weiter unten und liegen als Aseprite-Paletten im Ordner Art-Style/Paletten.


## Figuren

- **Sami:** Grau-schwarz, gesichtslos (nur Dunkelheit unter dem Kasa), weißer Hutrand als Erkennungszeichen. Keine Outline. Wird komplett neu designt.
- **Proportion:** Realistisch bis leicht stilisiert (etwa 5–7 Köpfe), kein Chibi.
- **Outline:** Keine Outlines bei Figuren. Form und Lesbarkeit kommen aus dunklen Flächen, Randlicht und Silhouette.
- **Andere Figuren:** Farbig. NPCs und Bosse sind ikonisch und einfach zu lesen (Undertale-Charme), mit einer klaren Signaturfarbe.
- **Silhouetten-Test:** Jede Figur muss als reine schwarze Silhouette erkennbar sein. Gegner-Angriffe müssen schon in der Silhouette ablesbar sein (Ausholbewegung).
- **Shading:** Licht kommt aus der Szene (2D-Lights). In den Sprites selbst wenige Töne: etwa 4–5 Töne pro Material bei 48 px, 3 bei 32 px. Kein Pillow-Shading.

## Kampf-Effekte

- Basis-Hieb: Weiß mit hellgrauer Kante, als Smear-Frame über 1–2 Frames. Höchstens 2–3 Farben pro Hieb.
- Treffer: Gegner blitzt 1–2 Frames weiß auf, dazu Hitstop (leichte Treffer etwa 2–4 Frames bei 60 FPS, schwere 6–10, Boss-Finisher 12–18).
- Blut ist pixelig und brutal: rote Pixel spritzen und schweben vom Gegner weg, ein paar bleiben am Boden. Bei Kills mehr, bei normalen Treffern 3–8 Pixel.
- Skills verändern den Hieb (wie Atemtechniken in Demon Slayer): Schatten-Skill = schwarze Tusche mit Spritzern, Frost-Skill = Eisblau mit Eissplittern. Jede neue Technik bekommt ihre eigene Farbe und Form.
- Bildschirm-Wackeln kurz und knackig (Nahkampf etwa 4–8 Frames). Nicht bei jedem Treffer jedes Gegners.
- Gegner-Angriffe in der Signaturfarbe des Gegners, Sami-Effekte in Weiß/Skillfarbe. So weiß man immer, wessen Angriff es ist.
- Vollbild-Effekte (Farbfilter, Zeitlupe, Letterbox) nur für Finisher, Boss-Phasen und Story-Momente.

## Dialog & Schrift

- Keine Textboxen. Text steht frei in der Welt, mit weichem dunklem Schatten für die Lesbarkeit.
- Schrift: Yuji Syuku (Pinsel-Serif, Google Fonts, frei nutzbar unter der OFL). Textfarbe Weiß F2F0EA.
- Namen stehen in der Signaturfarbe der Figur. Sami spricht in Weiß/Grau.
- Offen: genaue Platzierung (Untertitel unten, über dem Sprecher, oder groß im Raum für Story-Momente).
- Harajuku (bisherige Schrift) höchstens für Titel und Logos.

## HUD

- HUD ist klassisch immer sichtbar: Leben, Ki/Ausdauer, Skills, Tränke, Geld.
- Stil noch offen. Getestet: Tusche-Pinsel (HD), Pixel-Rahmen (klassisch), Kasa-Hüte als Lebenspunkte (ikonisch).
- UI wird über einem Screen-Space-Overlay-Canvas gerendert, damit Text scharf bleibt.

## Kamera

- Cineastisch und je nach Szene verschieden: rauszoomen bei großen Orten, näher ran bei Kämpfen und Story, Blickwinkel wechselt.
- Im Kampf hat die Lesbarkeit Vorrang: kein Zoom, der Gegner aus dem Bild schneidet.

## Häufige Fehler (immer prüfen)

- Unschärfe oder Nebel auf der Spielebene.
- Sami geht im dunklen Hintergrund unter (fehlendes Randlicht).
- Gegner-Farbe zu nah an der Biom-Farbe.
- Sprites skaliert, gedreht oder mit Bilinear-Filter (verwaschene Pixel).
- Verschiedene PPU-Werte oder Pixelgrößen im selben Bild.
- Zu viel Bloom, Lichtstrahlen und Partikel im Kampf.

## Offene Entscheidungen

- Sami 32 oder 48 px (Animationstest) und damit 320×180 oder 480×270.
- Platzierung der Dialoge.
- HUD-Stil.
- Signaturfarbe des Endbosses.
- Biome bekommen jeweils eigene Strukturen und Formen. Bisher sind nur ihre Farbwelten und Licht-Stimmungen festgelegt.

## Referenzbilder & Dateien

Im Ordner Art-Style neben diesem Dokument liegen die Referenzbilder aus der Stilfindung (nur zur Orientierung, NICHT final, Figuren dort sind Platzhalter) und die Aseprite-Paletten (.gpl) für Sami, Kampf und jedes Biom.


## Paletten (Hex)

**Sami:** Kasa / Schatten #0B0B0B, Kleidung dunkel #232323, Hakama #3C3C3C, Hakama hell #5A5A5A, Obi / Details #8A8884, Hutrand / Hieb (Weiß) #F2F0EA

**Kampf & Skills:** Hieb Weiß #F2F0EA, Hieb Kante #B9B7B0, Blut #D0313A, Blut dunkel #7A141B, Schatten-Hieb #0B0B0B, Schatten-Kante #2A1630, Frost-Hieb #E6F6FF, Frost-Kante #7CC4E8, Treffer-Blitz #FFF6EE

**Biom 1 Kirschblüten-Ruinen:** Himmel 1 #4A2A46, Himmel 2 #6E3A58, Himmel 3 #985068, Himmel 4 #C06E78, Horizont #E09A8E, Berge fern #A8607A, Berge mitte #7A4662, Ruinen #5A3150, Nah #3D2238, Blüte dunkel #B84C72, Blüte #DE86A2, Blüte hell #F7C3D1, Boden #2A1A24, Boden dunkel #1C1018, Licht #FFD9BD, Randlicht #FFD2B0, Boss Blutrot #B3232B, Boss dunkel #7A141B, Boss hell #E2474B

**Biom 2 Bambus-Sumpf / Nebelwald:** Himmel 1 #4A5A50, Himmel 2 #5C6E60, Himmel 3 #708272, Himmel 4 #869684, Horizont #9AA896, Fern #6E8270, Mitte #4E6452, Nah #3A4E40, Sehr nah #2A3A2E, Laub dunkel #2E4A32, Laub #46663E, Laub hell #62824E, Bambus #3A5A34, Boden #18221A, Glühwürmchen #D8F2A0, Randlicht #E4F4C8, Boss Jade #2F9A6A, Boss dunkel #1C5E40, Boss hell #5CC890

**Biom 3 Eisgipfel / Frost-Tempel:** Himmel 1 #0B0F1C, Himmel 2 #121A2E, Himmel 3 #1A2640, Himmel 4 #223452, Horizont #2C4262, Fern #3A4C66, Mitte #2A3A54, Nah #101A2C, Schnee dunkel #9AB0C8, Schnee #C4D4E4, Schnee hell #EEF4FA, Boden #0D111B, Laterne #FFB35A, Mond-Randlicht #BCD0F4, Boss Eisblau #4FA6D2, Boss dunkel #2A6A92, Boss hell #9AD8F2

**Biom 4 Vulkan / Asche-Schmiede:** Himmel 1 #140A08, Himmel 2 #24100A, Himmel 3 #3A160C, Himmel 4 #58200E, Horizont #7A2C10, Fern #3A1A12, Mitte #2A120C, Nah #140806, Glut #FF7A1A, Glut hell #FF9A3A, Randlicht #FFB070, Asche #4A4440, Asche hell #6A625C, Boden #140A08, Boss Glut-Orange #E0661F, Boss dunkel #9A3A0C, Boss hell #FFA04A

**Biom 5 Leere / Thronsaal:** Schwarz 1 #050507, Schwarz 2 #0A0A0E, Dunkel 3 #121218, Dunkel 4 #1C1C24, Horizont #2A2A34, Licht #E8E4F0, Licht dunkel #CFCAD8, Randlicht #F0ECFF, Akzent (Platzhalter) #B89CF0
