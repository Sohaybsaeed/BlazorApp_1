# generate-icons.ps1
# Run this script once to generate all PWA icon sizes from icon.svg
# Requires: Inkscape (https://inkscape.org) OR ImageMagick (https://imagemagick.org)
# 
# Usage: Right-click → Run with PowerShell
#        OR: powershell -ExecutionPolicy Bypass -File generate-icons.ps1

$sizes = @(72, 96, 128, 144, 152, 192, 384, 512)
$svgPath = "$PSScriptRoot\icon.svg"

# Try ImageMagick first
$magick = Get-Command "magick" -ErrorAction SilentlyContinue
$inkscape = Get-Command "inkscape" -ErrorAction SilentlyContinue

foreach ($size in $sizes) {
    $outFile = "$PSScriptRoot\icon-${size}x${size}.png"
    
    if ($magick) {
        Write-Host "Generating ${size}x${size} with ImageMagick..."
        & magick -background none -size "${size}x${size}" "$svgPath" "$outFile"
    }
    elseif ($inkscape) {
        Write-Host "Generating ${size}x${size} with Inkscape..."
        & inkscape --export-type=png --export-width=$size --export-height=$size --export-filename="$outFile" "$svgPath"
    }
    else {
        Write-Warning "Neither ImageMagick nor Inkscape found. Please install one and re-run."
        Write-Host "Download ImageMagick: https://imagemagick.org/script/download.php#windows"
        break
    }
}

Write-Host "Done! Icons generated in: $PSScriptRoot"
