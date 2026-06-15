param(
    [string]$RepositoryName = "unity-2d-survivor-prototype",
    [ValidateSet("public", "private", "internal")]
    [string]$Visibility = "public",
    [string]$Owner = "GaoWY-design"
)

$ErrorActionPreference = "Stop"

$gh = Get-Command gh -ErrorAction SilentlyContinue
if (-not $gh) {
    $fallback = "C:\Program Files\GitHub CLI\gh.exe"
    if (Test-Path -LiteralPath $fallback) {
        $ghPath = $fallback
    }
    else {
        throw "GitHub CLI is not available. Install it with: winget install --id GitHub.cli -e"
    }
}
else {
    $ghPath = $gh.Source
}

& $ghPath auth status *> $null
if ($LASTEXITCODE -ne 0) {
    throw "GitHub CLI is not logged in. Run: gh auth login"
}

$status = git status --porcelain
if ($status) {
    throw "Working tree has uncommitted changes. Commit or stash them before publishing."
}

$repoFullName = "$Owner/$RepositoryName"
$repoUrl = "https://github.com/$repoFullName"

& $ghPath repo view $repoFullName *> $null
if ($LASTEXITCODE -ne 0) {
    $visibilityFlag = "--$Visibility"
    & $ghPath repo create $repoFullName $visibilityFlag --source . --remote origin --push
}
else {
    $remote = git remote get-url origin 2>$null
    if ($LASTEXITCODE -ne 0) {
        git remote add origin "$repoUrl.git"
    }
    elseif ($remote -ne "$repoUrl.git" -and $remote -ne "git@github.com:$repoFullName.git") {
        git remote set-url origin "$repoUrl.git"
    }

    git push -u origin main
}

Write-Output $repoUrl
