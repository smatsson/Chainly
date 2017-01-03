
function setVersionInfo() {
	$assemblyVersion = git describe --abbrev=0 --tags
	$assemblyInformationalVersion = git describe --tags --long	
	(Get-Content .\Chainly\project.json) -replace "1.0.0-*", "$assemblyVersion-" | Out-File .\Chainly\project.json
}

function makeBuildFolder() {
	if(Test-Path -Path ".\Chainly\" ){
		Remove-Item -Recurse -Force .\Chainly\
	}
	
	New-Item -ItemType directory .\Chainly\
	robocopy /E ..\Source\chainly\ .\Chainly\ /MIR
	copy ..\Source\global.json .\Chainly\
}

function verifyNuget() {
	if(![bool](Get-Command dotnet -errorAction SilentlyContinue)) {
		Throw "Could not find dotnet command"
	}
}

function createPackage() {
	cd Chainly\
	& dotnet pack -c Release
	cd..
}

function movePackage() {
	move Chainly\bin\Release\*.nupkg .\
}

function cleanup() {
	Remove-Item -Recurse -Force .\Chainly\
}

verifyNuget
Write-Host Copying files for build...
makeBuildFolder
Write-Host Setting version info...
setVersionInfo
Write-Host Version info set
Write-Host Creating package...
createPackage
movePackage
Write-Host Package created
Write-Host Cleaning up...
cleanup
Write-Host Done!
