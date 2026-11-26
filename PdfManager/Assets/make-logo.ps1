# Draws the PDF Manager logo with simple shapes and saves logo.png (256 px) and app.ico (16-256 px).
# Run from this folder:  powershell -ExecutionPolicy Bypass -File make-logo.ps1
param([string]$OutDir = $PSScriptRoot)

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'

function New-RoundedRect([float]$x, [float]$y, [float]$w, [float]$h, [float]$r) {
    $p = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $r * 2
    $p.AddArc($x, $y, $d, $d, 180, 90)
    $p.AddArc($x + $w - $d, $y, $d, $d, 270, 90)
    $p.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
    $p.AddArc($x, $y + $h - $d, $d, $d, 90, 90)
    $p.CloseFigure()
    return $p
}

function New-Page([float]$x, [float]$y, [float]$w, [float]$h, [float]$fold) {
    $p = New-Object System.Drawing.Drawing2D.GraphicsPath
    $p.AddPolygon([System.Drawing.PointF[]]@(
        (New-Object System.Drawing.PointF($x, $y)),
        (New-Object System.Drawing.PointF(($x + $w - $fold), $y)),
        (New-Object System.Drawing.PointF(($x + $w), ($y + $fold))),
        (New-Object System.Drawing.PointF(($x + $w), ($y + $h))),
        (New-Object System.Drawing.PointF($x, ($y + $h)))))
    return $p
}

function Color([string]$hex, [int]$alpha = 255) {
    $c = [System.Drawing.ColorTranslator]::FromHtml($hex)
    return [System.Drawing.Color]::FromArgb($alpha, $c.R, $c.G, $c.B)
}

# Everything is drawn on a 256-unit grid, scaled up 4x for smooth edges.
function Draw-Logo([int]$size) {
    $bmp = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = 'AntiAlias'
    $g.TextRenderingHint = 'AntiAliasGridFit'
    $g.InterpolationMode = 'HighQualityBicubic'
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.ScaleTransform($size / 256.0, $size / 256.0)

    # Tile
    $tile = New-RoundedRect 8 8 240 240 54
    $tileBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        (New-Object System.Drawing.PointF(8, 8)), (New-Object System.Drawing.PointF(248, 248)),
        (Color '#3A8EF0'), (Color '#2B3A8F'))
    $g.FillPath($tileBrush, $tile)

    # Back page
    $back = New-Page 52 34 112 150 30
    $g.FillPath((New-Object System.Drawing.SolidBrush((Color '#BFD6F6'))), $back)

    # Shadow and front page
    $shadow = New-Page 82 56 114 156 34
    $g.FillPath((New-Object System.Drawing.SolidBrush((Color '#0A1A4A' 70))), $shadow)
    $front = New-Page 76 50 114 156 34
    $g.FillPath([System.Drawing.Brushes]::White, $front)
    $fold = New-Object System.Drawing.Drawing2D.GraphicsPath
    $fold.AddPolygon([System.Drawing.PointF[]]@(
        (New-Object System.Drawing.PointF(156, 50)),
        (New-Object System.Drawing.PointF(156, 84)),
        (New-Object System.Drawing.PointF(190, 84))))
    $g.FillPath((New-Object System.Drawing.SolidBrush((Color '#C3D2E8'))), $fold)

    # Text lines
    $linePen = New-Object System.Drawing.Pen((Color '#AFC0D8'), 7)
    $linePen.StartCap = 'Round'; $linePen.EndCap = 'Round'
    $g.DrawLine($linePen, 94, 96, 140, 96)
    $g.DrawLine($linePen, 94, 112, 172, 112)

    # Ribbon with PDF
    $band = New-RoundedRect 60 130 144 50 8
    $g.FillPath((New-Object System.Drawing.SolidBrush((Color '#E5483C'))), $band)
    $font = New-Object System.Drawing.Font('Segoe UI', 30, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $format = New-Object System.Drawing.StringFormat
    $format.Alignment = 'Center'; $format.LineAlignment = 'Center'
    $g.DrawString('PDF', $font, [System.Drawing.Brushes]::White, (New-Object System.Drawing.RectangleF(60, 131, 144, 50)), $format)

    $g.Dispose()
    return $bmp
}

function Resize([System.Drawing.Bitmap]$source, [int]$size) {
    $bmp = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = 'HighQualityBicubic'
    $g.PixelOffsetMode = 'HighQuality'
    $g.SmoothingMode = 'AntiAlias'
    $g.DrawImage($source, 0, 0, $size, $size)
    $g.Dispose()
    return $bmp
}

$master = Draw-Logo 1024
$logo = Resize $master 256
$logo.Save((Join-Path $OutDir 'logo.png'), [System.Drawing.Imaging.ImageFormat]::Png)

# An .ico file holding PNG images at each size Windows asks for.
$sizes = 16, 20, 24, 32, 40, 48, 64, 128, 256
$images = foreach ($s in $sizes) {
    $bmp = Resize $master $s
    $ms = New-Object System.IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    , $ms.ToArray()
}
$out = New-Object System.IO.MemoryStream
$w = New-Object System.IO.BinaryWriter($out)
$w.Write([UInt16]0); $w.Write([UInt16]1); $w.Write([UInt16]$sizes.Count)
$offset = 6 + 16 * $sizes.Count
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $s = $sizes[$i]; $data = $images[$i]
    $w.Write([byte]($s % 256)); $w.Write([byte]($s % 256)); $w.Write([byte]0); $w.Write([byte]0)
    $w.Write([UInt16]1); $w.Write([UInt16]32); $w.Write([UInt32]$data.Length); $w.Write([UInt32]$offset)
    $offset += $data.Length
}
foreach ($data in $images) { $w.Write($data) }
$w.Flush()
[System.IO.File]::WriteAllBytes((Join-Path $OutDir 'app.ico'), $out.ToArray())
"Saved logo.png and app.ico in $OutDir"
