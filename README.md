# PDF Manager

A Windows desktop app, written in VB.NET (Windows Forms), for working with PDFs.
All the PDF work is done by the free QPDF command-line tool, which comes bundled.

## Open it in Visual Studio

Double-click `src\PdfManager.sln` (Visual Studio 2026 or later, with .NET 10), then press F5. The app targets `net10.0-windows10.0.19041.0` so it can use Windows' PDF renderer for page thumbnails; it runs on Windows 10 version 1809 or later.
The app is `src\PdfManager` (VB.NET, Windows Forms; the main window opens in the form designer).
Its tests are `src\PdfManager.Tests` (xUnit, run from Test Explorer); they run every operation through QPDF and check which pages come out.

The build keeps its temporary files (the "obj" folders) on this computer's local disk, under `%LOCALAPPDATA%\PdfManagerBuild`, set in `Directory.Build.props`. That lets the project live in Google Drive, OneDrive or Dropbox. Without it, the sync program opens new files mid-build and Visual Studio stops with "The CreateAppHost task failed unexpectedly … user-mapped section open" and a long call stack. Old `obj` folders still inside the project are ignored and can be deleted.

`CheckLauncher.csproj` at the top is not part of the app: it only lets Craft Anything's automatic check build and start it.

## Share it with other computers

Right-click `Make-Release.ps1` and choose **Run with PowerShell**
(or run `powershell -ExecutionPolicy Bypass -File Make-Release.ps1 -Version 1.0.1`).
It makes `dist\PdfManager-<version>-win-x64.zip` (about 47 MB), checks that everything the app needs is inside, and checks that the bundled QPDF runs.

The zip holds `PdfManager.exe` (with .NET built in), the `qpdf` folder and `Read me first.txt`.
On the other PC: unzip it anywhere and double-click `PdfManager.exe`. It needs 64-bit Windows 10 or 11 and nothing installed: no .NET, no QPDF, no Visual C++ runtime.

From Visual Studio instead: right-click the PdfManager project → **Publish** → choose the **Portable-win-x64** profile → **Publish**. The files land in `dist\PdfManager`; zip that folder yourself.

Windows SmartScreen may warn on first launch because the app isn't code-signed ("More info" → "Run anyway"). Signing it with a code-signing certificate removes that warning.

## Using it

1. **Add your PDFs**: use Add PDF or Add Folder, or drag files and folders onto the list. Move Up, Move Down and Sort A–Z set the order.
2. **Choose what to do**:
   - **Combine** joins all the pages of every PDF in the list.
   - **Merge Pages**: tick the PDFs you need and type the pages for each one in the Pages to Use column, for example `1-10` for the first and `5` for the second.
   - **Extract Pages** copies chosen pages into one PDF, one PDF per page, or a new PDF every few pages.
   - **Encrypt** makes a password-protected copy of each PDF (AES 256-bit), with optional limits on printing, copying and editing.
   - **Encrypt / Decrypt** has a "What to do" choice. **Remove the password (decrypt)** makes an unlocked copy of every locked PDF in the list (named like `report-unlocked.pdf`): no password to open it and no limits on printing, copying or editing. It asks for any password not typed yet. A PDF that only has limits is unlocked without its permissions password. PDFs without any password are skipped, and the result says how many.
   - **Rotate** turns all or chosen pages 90° clockwise, 90° anticlockwise or 180°.
   - **Stamp & Background** adds a stamp only (on top: logo, watermark, "Approved"), a background only (behind: letterhead), or both at once. The stamp PDF and the background PDF are chosen separately, each with its own "use page" number, and "Put them on pages" picks which pages get them. Backgrounds only show where pages are see-through.
   - **Arrange Pages** shows every page of every PDF in the list as a small picture (Show Page Thumbnails). Drag pages into a new order, click a page's red ✕ or press Delete to leave pages out, and use Ctrl+click / Shift+click to pick several. Move to Start, Move to End, Start Over and a picture-size slider (or Ctrl+mouse wheel) help with long documents. Use These Pages keeps the choice; Save Pages then writes the kept pages as one new PDF through QPDF. PDFs added to the list later join the end; removed ones leave. The pictures come from Windows' own PDF renderer (built into Windows 10 and 11); locked PDFs, and any Windows can't open as they are, are first unlocked by QPDF into memory only, never onto disk.
   - **Reduce Size** makes a smaller copy at one of four levels:
     - Normal: lossless repacking, no quality change.
     - Medium (recommended): photos re-saved as JPEG at quality 75.
     - High: photos at quality 50.
     - Extreme: photos at quality 30; also removes document details, page labels and accessibility tags.

     QPDF only keeps a re-saved photo when it is smaller, and the app reports the before and after sizes. QPDF can't lower a photo's resolution, so PDFs that are mostly text shrink less than scans and photos.
3. **Choose where to save**: pick the output folder and file name, then press the big blue button.
   The **Also** row can give every new PDF, whatever the operation, a password (**Protect with a password**, same choices as Encrypt) and/or a stamp and background (**Add a stamp / background**: stamp PDF, background PDF, the page of each to use, and which pages of the new PDF get them). QPDF does these in the same run that makes the PDF, including each piece of a split result. They're greyed out where the operation already does the same thing (Encrypt / Decrypt, Stamp & Background).

**Help** (top right, or press F1) explains every feature, with pictures of the window in the topics about each part, and a "What's new" topic. The pictures are made by `src\PdfManager\Assets\help\make-help-pictures.ps1` (build the app first, then run it; it covers folder paths with a neutral one) and built into the program. F1 opens the topic for the operation in use. The Page ranges topic lists examples, each with the pages it picks in a 10-page PDF, and has a "Try it" box. "Page range help" links sit next to every box where you type pages.

The big buttons in step 2 choose the operation. The Theme buttons at the top right switch between Light, Dark and Same as Windows; the choice is remembered.

Pages can be written as `1-3, 5, 8-end`, `odd`, `even` or `All`.
Locked PDFs show "Needs password": double-click one to unlock it.
Nothing is written until QPDF has finished and the result has been read back, so a failure never damages an existing file.

Batches run several QPDF copies at once (one fewer than the PC's processor cores, at most 8), and the status line counts the files done ("Working… 5 of 16 done"). Results still come out in list order, and if one file fails the rest are stopped and nothing is saved. On the build PC this made encrypting 16 PDFs about 5× faster (3.8 s → 0.8 s) and splitting a 60-page PDF into single pages about 4× faster (3.5 s → 0.8 s). `SpeedTests` in the tests prints these times.

## QPDF

QPDF 12.4.2 (official Windows 64-bit release, checksum verified) is in `src\PdfManager\tools\qpdf` and is copied next to the program when it builds.
To use a different copy, click "Use a different QPDF…" at the bottom of the window.
QPDF is © Jay Berkenbilt and Manfred Holger, under the Apache License 2.0; see `src\PdfManager\tools\qpdf\COPYRIGHT.txt`.

## Command line

`PdfManager.exe [files or folders…] [--tab=combine|merge|extract|encrypt|rotate|stamp|compress|arrange|decrypt] [--show-pages] [--theme=light|dark|auto] [--show-help[=topic]] [--screenshot picture.png]`

`--show-help` opens the Help window at start, on a topic such as `"--show-help=Page ranges"`; with `--screenshot`, the picture is of the Help window.

`--theme` changes the look for that run only, without changing the saved choice.

`--show-pages` opens the page thumbnail view once the PDFs given are read; with `--screenshot`, the picture is of that view.

## Logo

The logo (a blue tile with stacked pages and a red PDF ribbon) is an original drawing made from simple shapes by `src\PdfManager\Assets\make-logo.ps1`, which writes `logo.png` and the program icon `app.ico`.

`--screenshot` opens the window quietly behind other windows, saves a picture of it and exits.
