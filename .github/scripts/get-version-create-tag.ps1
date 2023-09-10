$majorMinorVersion = $env:MAJOR_MINOR_VERSION
$tagPrefix = $env:TAG_PREFIX

Write-Host "Major/minor version from TwitPoster.csproj: '$majorMinorVersion'"
Write-Host "Looking for old tags with prefix '$tagPrefix'"
$versionTags = git tag --list "$tagPrefix*"
Write-Host "Tags found: $versionTags"

$commitCount = 0

if ($versionTags) {
    $mostRecentTag = $versionTags | Sort-Object -Descending | Select-Object -First 1
    Write-Host "mostRecentTag tag: $mostRecentTag"

    $lastTagVersion = $mostRecentTag -replace $tagPrefix,''
    $commitCount = git rev-list --count $HEAD..$mostRecentTag

    $commitCount = git rev-list HEAD ^$mostRecentTag --pretty=oneline --count
    Write-Host "Commit count between current commit and last tag : $commitCount"
} else {
    $lastTagVersion = "0.0"
}

Write-Host "Most recent version: $lastTagVersion"

if ($majorMinorVersion -ne $lastTagVersion) {
    Write-Host "New version detected, creating a new tag..."
    git tag "$tagPrefix$majorMinorVersion"
    git push origin "$tagPrefix$majorMinorVersion"
    $commitCount = 0;
}

Write-Host "Commit count since last tag: $commitCount"

$version = "$majorMinorVersion.$commitCount"
Write-Host "Package version: $version"

echo "VERSION=${version}" | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append