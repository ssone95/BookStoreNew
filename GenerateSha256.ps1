# Used the following repository as a reference point
# Repo: https://gist.github.com/benrobot/67bacea1b1bbe4eb0d9529ba2c65b2a6
# Copyright: https://gist.github.com/benrobot

param (
    [string]$InputStr="secret"
)

Write-Host "Calculating Sha256 hash of a string: ${InputStr}"
$sha256Calculator = New-Object System.Security.Cryptography.SHA256Managed
$inputBytes = [System.Text.Encoding]::UTF8.GetBytes($InputStr)
$hashedInput = $sha256Calculator.ComputeHash($inputBytes)
$hashedInputStr = ($hashedInput | ForEach-Object {$_.ToString("x2")}) -join ''

Write-Host "Hash: ${hashedInputStr}"

