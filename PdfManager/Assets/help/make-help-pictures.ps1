# Makes the pictures shown in the Help window, from the app itself.
#
#   powershell -ExecutionPolicy Bypass -File make-help-pictures.ps1
#
# Build the app first (Debug). The script makes three small sample PDFs, opens the app on them
# with --screenshot for each part of the window (light theme), crops each picture to the part the
# Help topic talks about, covers the folder paths with a neutral one, and saves the results here
# as PNG files. They are built into the program (see PdfManager.vbproj).
param(
    [string]$Exe = (Join-Path $PSScriptRoot '..\..\bin\Debug\net10.0-windows10.0.19041.0\PdfManager.exe')
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$appRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$work = Join-Path $appRoot 'shots\help-work'
$samples = Join-Path $work 'samples'
$raw = Join-Path $work 'raw'
New-Item -ItemType Directory -Force $samples, $raw | Out-Null
$Exe = (Resolve-Path $Exe).Path

# --- Sample PDFs: a coloured title bar, grey text lines and two boxes on each page -------------
function New-SamplePdf([string]$name, [string]$label, [int]$pages, [string]$rgb) {
    $objs = New-Object System.Collections.Generic.List[string]
    $kids = (1..$pages | ForEach-Object { "$(2 + $_ * 2) 0 R" }) -join ' '
    $objs.Add('<< /Type /Catalog /Pages 2 0 R >>')
    $objs.Add("<< /Type /Pages /Kids [$kids] /Count $pages >>")
    $objs.Add('<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>')
    foreach ($p in 1..$pages) {
        $t = "$rgb rg 40 640 532 110 re f 1 1 1 rg BT /F1 64 Tf 70 670 Td ($label $p) Tj ET 0.75 0.75 0.75 rg 60 560 492 14 re f 60 530 420 14 re f 60 500 470 14 re f 60 470 380 14 re f $rgb rg 60 160 230 250 re f 0.8 0.8 0.8 rg 320 160 232 250 re f"
        $objs.Add("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R >> >> /Contents $(3 + $p * 2) 0 R >>")
        $objs.Add("<< /Length $($t.Length) >>`nstream`n$t`nendstream")
    }
    $sb = New-Object Text.StringBuilder
    [void]$sb.Append("%PDF-1.4`n")
    $offsets = @()
    for ($i = 0; $i -lt $objs.Count; $i++) { $offsets += $sb.Length; [void]$sb.Append("$($i + 1) 0 obj`n$($objs[$i])`nendobj`n") }
    $xref = $sb.Length
    [void]$sb.Append("xref`n0 $($objs.Count + 1)`r`n0000000000 65535 f`r`n")
    foreach ($o in $offsets) { [void]$sb.Append(('{0:D10} 00000 n' -f $o) + "`r`n") }
    [void]$sb.Append("trailer`n<< /Size $($objs.Count + 1) /Root 1 0 R >>`nstartxref`n$xref`n%%EOF`n")
    [IO.File]::WriteAllText((Join-Path $samples $name), $sb.ToString(), [Text.Encoding]::ASCII)
}
New-SamplePdf 'Invoice.pdf' 'Invoice' 3 '0.85 0.5 0.17'
New-SamplePdf 'Photos.pdf' 'Photo' 4 '0.18 0.62 0.36'
New-SamplePdf 'Report.pdf' 'Report' 6 '0.18 0.43 0.87'
$files = @('Invoice.pdf', 'Photos.pdf', 'Report.pdf') | ForEach-Object { '"' + (Join-Path $samples $_) + '"' }

# --- Pictures of the window ----------------------------------------------------------------
function Shoot([string]$name, [string]$extra) {
    $target = Join-Path $raw "$name.png"
    if (Test-Path $target) { Remove-Item $target }
    $argText = ($files -join ' ') + " --theme=light $extra --screenshot `"$target`""
    $p = Start-Process $Exe -ArgumentList $argText -PassThru
    if (-not $p.WaitForExit(90000)) { $p.Kill(); throw "The app didn't finish the picture $name." }
    if (-not (Test-Path $target)) { throw "No picture for $name." }
    return $target
}

# Places in the main window, measured at 125% scale (1475 pixels wide) and scaled to the picture.
$ref = 1475.0
function Scaled([System.Drawing.Bitmap]$bmp, [double[]]$box) {
    $k = $bmp.Width / $ref
    return New-Object System.Drawing.Rectangle ([int]($box[0] * $k)), ([int]($box[1] * $k)), ([int]($box[2] * $k)), ([int]($box[3] * $k))
}

# Covers the folder paths (the list's Folder column and the output folder box) with a neutral path.
function Hide-Folders([System.Drawing.Bitmap]$bmp, [bool]$outputBox, [double]$left = 968) {
    $k = $bmp.Width / $ref
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
    $font = New-Object System.Drawing.Font('Segoe UI', [single](9.0 * $k * 1.25), [System.Drawing.GraphicsUnit]::Point)
    $muted = [System.Drawing.Color]::FromArgb(110, 110, 120)
    $column = Scaled $bmp @($left, 238, (1446 - $left), 278)
    for ($y = $column.Top; $y -lt $column.Bottom; $y++) {
        $c = $bmp.GetPixel([int](($left - 13) * $k), $y)
        $pen = New-Object System.Drawing.Pen($c)
        $g.DrawLine($pen, $column.Left, $y, $column.Right, $y)
        $pen.Dispose()
    }
    for ($row = 0; $row -lt 3; $row++) {
        $g.DrawString('C:\Users\You\Documents\PDFs', $font, (New-Object System.Drawing.SolidBrush($muted)), [single](($left + 6) * $k), [single]((244 + 38 * $row) * $k))
    }
    if ($outputBox) {
        $box = Scaled $bmp @(160, 882, 1144, 21)
        $g.FillRectangle([System.Drawing.Brushes]::White, $box)
        $g.DrawString('C:\Users\You\Documents\PDFs', $font, [System.Drawing.Brushes]::Black, [single](159 * $k), [single](881 * $k))
    }
    $g.Dispose()
}

function Save-Picture([System.Drawing.Bitmap]$bmp, [double[]]$crop, [string]$name, [int]$width = 960) {
    $area = if ($crop) { Scaled $bmp $crop } else { New-Object System.Drawing.Rectangle 0, 0, $bmp.Width, $bmp.Height }
    $height = [int]($area.Height * $width / $area.Width)
    $out = New-Object System.Drawing.Bitmap $width, $height
    $g = [System.Drawing.Graphics]::FromImage($out)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.DrawImage($bmp, (New-Object System.Drawing.Rectangle 0, 0, $width, $height), $area, [System.Drawing.GraphicsUnit]::Pixel)
    $g.Dispose()
    $out.Save((Join-Path $PSScriptRoot "$name.png"), [System.Drawing.Imaging.ImageFormat]::Png)
    $out.Dispose()
    Write-Host "  $name.png ($width x $height)"
}

# Step 2 with its options: from the "2. Choose what to do" heading to the bottom of the options.
$options = @(15, 482, 1445, 352)
# The list and step 2 together (Merge Pages shows the Use and Pages to Use columns).
$listAndOptions = @(15, 115, 1445, 719)

Write-Host 'Taking pictures of PDF Manager...'
$shots = [ordered]@{
    'overview' = '--tab=combine'
    'merge'    = '--tab=merge'
    'extract'  = '--tab=extract'
    'arrange'  = '--tab=arrange'
    'encrypt'  = '--tab=encrypt'
    'decrypt'  = '--tab=decrypt'
    'rotate'   = '--tab=rotate'
    'stamp'    = '--tab=stamp'
    'compress' = '--tab=compress'
    'pages'    = '--show-pages'
}
foreach ($name in $shots.Keys) {
    $file = Shoot $name $shots[$name]
    $bmp = New-Object System.Drawing.Bitmap $file
    $copy = New-Object System.Drawing.Bitmap $bmp
    $bmp.Dispose()
    switch ($name) {
        'overview' { Hide-Folders $copy $true; Save-Picture $copy $null 'help-overview'; Save-Picture $copy @(15, 838, 1445, 185) 'help-extras' }
        'merge'    { Hide-Folders $copy $false 1066; Save-Picture $copy $listAndOptions 'help-merge' }
        'pages'    { Save-Picture $copy $null 'help-pages' }
        default    { Save-Picture $copy $options "help-$name" }
    }
    $copy.Dispose()
}
Write-Host 'Done. Rebuild the app to include the new pictures.'
