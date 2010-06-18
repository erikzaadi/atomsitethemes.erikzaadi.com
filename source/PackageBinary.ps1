function RecurseDelete ([string]$path){
  Write-Host "RecurseDelete " + $path
  if (!(Test-Path $path)) {exit}
  dir $path | ? {$_.psIsContainer -eq $true} | % {
      RecurseDelete $_.FullName
  }
  rd $path -force -rec
}

# This script will copy all the files needed to create a release on CodePlex.  
# It also zips the files using 7zip.  You must have 7zip installed.
# The zip file is suitable for use with WebPI

$ErrorActionPreference = "Stop"

#Read version number
$version = get-content -path Version.txt

if ($version.Length -lt 7) {
  Write-Error "ERROR: Version is not found or incorrect." 
  exit 1 
}

#Create destination folder (but first remove if exists)
$dest = '..\AtomSiteTags\AtomSite ' + $version + ' Binary'
if (!(Test-Path $dest)) {
    md $dest
} else {
    write-host "Path exists, deleting..."
    RecurseDelete $dest    
    md $dest    
}

$dest = (Resolve-Path $dest).Path

#Get zip file name based on destination
$zipFile = $dest+".zip"
if (Test-Path $zipFile) { ri $zipFile }

#Get source path, and files to copy and file types to exclude
$src = (Resolve-Path '.\').Path
$files = @('*.xml'
,'Readme.htm'
,'WebCore\*.aspx'
,'WebCore\*.asax'
,'WebCore\*.config'
,'WebCore\*.ico'
,'WebCore\App_Data\*'
,'WebCore\bin\*.dll'
,'WebCore\css\*'
,'WebCore\img\*'
,'WebCore\js\*'
,'WebCore\plugins\*.zip'
,'WebCore\themes\*')
$fTypes = '*.log', '*.cs', '_svn\*'

#For each file, copy it to destination (creating directories when they don't exist)
ls $files -exclude $fTypes -r | % {
  $file = $_.fullname
  $new = $file.Replace($src, $dest)
  trap [System.IO.DirectoryNotFoundException] {
    [void](md $(split-path $new -par))
    cpi $file $new
    continue
  }
  cpi $file $new
}

#Rename WebCore folder to AtomSite
$wc = $dest + "\WebCore"
$as = $dest + "\AtomSite"
mv $wc $as

#Zip the folder
$7ZipPath = "C:\Program Files\7-Zip\7z.exe"

&$7ZipPath a -tzip $zipFile $dest\* -mx9 -r -mmt | out-host #Call 7z.exe to create archive file 
if ($LASTEXITCODE -ne 0) {
  Write-Error "ERROR: 7Zip terminated with exit code $LASTEXITCODE. Script halted." 
  exit $LASTEXITCODE 
}