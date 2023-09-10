$tagPrefix = $Env:tagPrefix
  
Write-Host "Looking for old tags with prefix '$tagPrefix'"

$versionTags = git tag --list "$tagPrefix*"
Write-Host "Found tags: $versionTags"
  
if ($versionTags) {
    $mostRecentTag = $versionTags | Sort-Object -Descending | Select-Object -First 1
    Write-Host "Latest tag is: $mostRecentTag"

    $versionTags | Foreach-Object {
        if ($_ -ne $mostRecentTag) {
            Write-Host "Deleting old tag: $_"
            git push origin --delete $_
            git tag -d $_
            Write-Host "Deleted old tag: $_"
        }
    }
} else {
    Write-Host "No tags found with prefix '$tagPrefix'"
}