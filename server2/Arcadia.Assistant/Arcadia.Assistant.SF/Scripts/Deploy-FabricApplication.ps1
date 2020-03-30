<#
.SYNOPSIS 
Deploys a Service Fabric application type to a cluster.

.DESCRIPTION
This script deploys a Service Fabric application type to a cluster.  It is invoked by Visual Studio when deploying a Service Fabric Application project.

.NOTES
WARNING: This script file is invoked by Visual Studio.  Its parameters must not be altered but its logic can be customized as necessary.

.PARAMETER PublishProfileFile
Path to the file containing the publish profile.

.PARAMETER ApplicationPackagePath
Path to the folder of the packaged Service Fabric application.

.PARAMETER DeployOnly
Indicates that the Service Fabric application should not be created or upgraded after registering the application type.

.PARAMETER ApplicationParameter
Hashtable of the Service Fabric application parameters to be used for the application.

.PARAMETER UnregisterUnusedApplicationVersionsAfterUpgrade
Indicates whether to unregister any unused application versions that exist after an upgrade is finished.

.PARAMETER OverrideUpgradeBehavior
Indicates the behavior used to override the upgrade settings specified by the publish profile.
'None' indicates that the upgrade settings will not be overridden.
'ForceUpgrade' indicates that an upgrade will occur with default settings, regardless of what is specified in the publish profile.
'VetoUpgrade' indicates that an upgrade will not occur, regardless of what is specified in the publish profile.

.PARAMETER UseExistingClusterConnection
Indicates that the script should make use of an existing cluster connection that has already been established in the PowerShell session.  The cluster connection parameters configured in the publish profile are ignored.

.PARAMETER OverwriteBehavior
Overwrite Behavior if an application exists in the cluster with the same name. Available Options are Never, Always, SameAppTypeAndVersion. This setting is not applicable when upgrading an application.
'Never' will not remove the existing application. This is the default behavior.
'Always' will remove the existing application even if its Application type and Version is different from the application being created. 
'SameAppTypeAndVersion' will remove the existing application only if its Application type and Version is same as the application being created.

.PARAMETER SkipPackageValidation
Switch signaling whether the package should be validated or not before deployment.

.PARAMETER SecurityToken
A security token for authentication to cluster management endpoints. Used for silent authentication to clusters that are protected by Azure Active Directory.

.PARAMETER CopyPackageTimeoutSec
Timeout in seconds for copying application package to image store.

.PARAMETER RemoteVMDeploy

.EXAMPLE
. Scripts\Deploy-FabricApplication.ps1 -ApplicationPackagePath 'pkg\Debug'

Deploy the application using the default package location for a Debug build.

.EXAMPLE
. Scripts\Deploy-FabricApplication.ps1 -ApplicationPackagePath 'pkg\Debug' -DoNotCreateApplication

Deploy the application but do not create the application instance.

.EXAMPLE
. Scripts\Deploy-FabricApplication.ps1 -ApplicationPackagePath 'pkg\Debug' -ApplicationParameter @{CustomParameter1='MyValue'; CustomParameter2='MyValue'}

Deploy the application by providing values for parameters that are defined in the application manifest.
#>

Param
(
    [String]
    $PublishProfileFile,

    [String]
    $ApplicationPackagePath,

    [Switch]
    $DeployOnly,

    [Hashtable]
    $ApplicationParameter,

    [Boolean]
    $UnregisterUnusedApplicationVersionsAfterUpgrade,

    [String]
    [ValidateSet('None', 'ForceUpgrade', 'VetoUpgrade')]
    $OverrideUpgradeBehavior = 'None',

    [Switch]
    $UseExistingClusterConnection,

    [String]
    [ValidateSet('Never','Always','SameAppTypeAndVersion')]
    $OverwriteBehavior = 'Never',

    [Switch]
    $SkipPackageValidation,

    [String]
    $SecurityToken,

    [int]
    $CopyPackageTimeoutSec,

    [int]
    $RegisterApplicationTypeTimeoutSec,

    [Switch]
    $RemoteVMDeploy
)

function Read-XmlElementAsHashtable
{
    Param (
        [System.Xml.XmlElement]
        $Element
    )

    $hashtable = @{}
    if ($Element.Attributes)
    {
        $Element.Attributes | 
            ForEach-Object {
                $boolVal = $null
                if ([bool]::TryParse($_.Value, [ref]$boolVal)) {
                    $hashtable[$_.Name] = $boolVal
                }
                else {
                    $hashtable[$_.Name] = $_.Value
                }
            }
    }

    return $hashtable
}

function Read-PublishProfile
{
    Param (
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $PublishProfileFile
    )

    $publishProfileXml = [Xml] (Get-Content $PublishProfileFile)
    $publishProfile = @{}

    $publishProfile.ClusterConnectionParameters = Read-XmlElementAsHashtable $publishProfileXml.PublishProfile.Item("ClusterConnectionParameters")
    $publishProfile.UpgradeDeployment = Read-XmlElementAsHashtable $publishProfileXml.PublishProfile.Item("UpgradeDeployment")
    $publishProfile.CopyPackageParameters = Read-XmlElementAsHashtable $publishProfileXml.PublishProfile.Item("CopyPackageParameters")
    $publishProfile.RegisterApplicationParameters = Read-XmlElementAsHashtable $publishProfileXml.PublishProfile.Item("RegisterApplicationParameters")

    if ($publishProfileXml.PublishProfile.Item("UpgradeDeployment"))
    {
        $publishProfile.UpgradeDeployment.Parameters = Read-XmlElementAsHashtable $publishProfileXml.PublishProfile.Item("UpgradeDeployment").Item("Parameters")
        if ($publishProfile.UpgradeDeployment["Mode"])
        {
            $publishProfile.UpgradeDeployment.Parameters[$publishProfile.UpgradeDeployment["Mode"]] = $true
        }
    }

    $publishProfileFolder = (Split-Path $PublishProfileFile)
    $publishProfile.ApplicationParameterFile = [System.IO.Path]::Combine($PublishProfileFolder, $publishProfileXml.PublishProfile.ApplicationParameterFile.Path)

    return $publishProfile
}

$LocalFolder = (Split-Path $MyInvocation.MyCommand.Path)
$deployFile = "$LocalFolder\deploy.json"
$remoteLocationConfig = {}
if (Test-Path $deployFile)
{
    $remoteLocationConfig = Get-Content -Raw -Path $deployFile | ConvertFrom-Json
}

if (!$PublishProfileFile)
{
    $PublishProfileFile = "$LocalFolder\..\PublishProfiles\Local.xml"
}

if (!$ApplicationPackagePath)
{
    if ($RemoteVMDeploy)
    {
	$package_cfg = $remoteLocationConfig.package_configuration
	$ApplicationPackagePath = "$LocalFolder\..\pkg\$package_cfg"
    }
    else
    {    
	$ApplicationPackagePath = "$LocalFolder\..\pkg\Release"
    }
}
else
{
    if ($RemoteVMDeploy)
    {
	$package_cfg = $remoteLocationConfig.package_configuration
	$ApplicationPackagePath = "$ApplicationPackagePath\$package_cfg"
    }
}

$ApplicationPackagePath = Resolve-Path $ApplicationPackagePath

if (!(Test-Path $ApplicationPackagePath))
{
    $errMsg = "$ApplicationPackagePath is not found."
    throw $errMsg
}

$publishProfile = Read-PublishProfile $PublishProfileFile

if (-not $UseExistingClusterConnection)
{
    $ClusterConnectionParameters = $publishProfile.ClusterConnectionParameters
    if ($SecurityToken)
    {
        $ClusterConnectionParameters["SecurityToken"] = $SecurityToken
    }

    try
    {
        [void](Connect-ServiceFabricCluster @ClusterConnectionParameters)
    }
    catch [System.Fabric.FabricObjectClosedException]
    {
        Write-Warning "Service Fabric cluster may not be connected."
        throw
    }
}

$RegKey = "HKLM:\SOFTWARE\Microsoft\Service Fabric SDK"
$ModuleFolderPath = (Get-ItemProperty -Path $RegKey -Name FabricSDKPSModulePath).FabricSDKPSModulePath
Import-Module "$ModuleFolderPath\ServiceFabricSDK.psm1"

$IsUpgrade = ($publishProfile.UpgradeDeployment -and $publishProfile.UpgradeDeployment.Enabled -and $OverrideUpgradeBehavior -ne 'VetoUpgrade') -or $OverrideUpgradeBehavior -eq 'ForceUpgrade'

$PublishParameters = @{
    'ApplicationParameterFilePath' = $publishProfile.ApplicationParameterFile
    'ApplicationParameter' = $ApplicationParameter
    'CopyPackageTimeoutSec' = 7200
    'ErrorAction' = 'Stop'
}

if ($RemoteVMDeploy)
{
	$sfpkgFile = "sf_package.sfpkg"

	Write-Host "Use remote location"

	if (Test-Path Y:)
	{
	    Get-PSDrive Y | Remove-PSDrive
	}

	$RemoteLocationShare = $remoteLocationConfig.remote_location_share
        $pass=$remoteLocationConfig.remote_location_pass|ConvertTo-SecureString -AsPlainText -Force
        $cred = New-Object System.Management.Automation.PsCredential($remoteLocationConfig.remote_location_user,$pass)
	New-PSDrive -name Y -Root $RemoteLocationShare -Credential $cred -PSProvider filesystem


	$zipContentPath = "$ApplicationPackagePath\*"
	$sourcePath = (Get-Item $ApplicationPackagePath).parent.FullName

	$archive = "$sourcePath\$sfpkgFile.zip"
	$manifest = "$ApplicationPackagePath\ApplicationManifest.xml"

	Write-Host ("Compress package to file '" + $archive + "'")
        Compress-Archive -Path $zipContentPath -DestinationPath $archive -Force

	Remove-Item Y:\*.sfpkg -Force
	Remove-Item Y:\*.xml -Force

	$destination = "Y:\$sfpkgFile"
	Write-Host ("Copy package '" + $archive + "' to '" + $destination + "'")
	Copy-Item $archive -destination $destination -Force

	$destination = "Y:\ApplicationManifest.xml"
	Write-Host ("Copy manifest '" + $manifest + "' to '" + $destination + "'")
	Copy-Item $manifest -destination $destination -Force

	$PackageLocationURL = $remoteLocationConfig.package_location_url
	$PublishParameters['ApplicationPathAsUrl'] = "$PackageLocationURL/$sfpkgFile"
	$PublishParameters['ApplicationManifestPath'] = $manifest
	Write-Host ("Apply application path URL as '" + $PublishParameters['ApplicationPathAsUrl'] + "'")

	Get-PSDrive Y | Remove-PSDrive
}
else
{
    Write-Host 'Set ApplicationPackagePath as ' + $ApplicationPackagePath
    $PublishParameters['ApplicationPackagePath'] = $ApplicationPackagePath
}

if ($publishProfile.CopyPackageParameters.CopyPackageTimeoutSec)
{
    $PublishParameters['CopyPackageTimeoutSec'] = $publishProfile.CopyPackageParameters.CopyPackageTimeoutSec
}

if ($publishProfile.CopyPackageParameters.CompressPackage)
{
    $PublishParameters['CompressPackage'] = $publishProfile.CopyPackageParameters.CompressPackage
}

if ($publishProfile.RegisterApplicationParameters.RegisterApplicationTypeTimeoutSec)
{
    $PublishParameters['RegisterApplicationTypeTimeoutSec'] = $publishProfile.RegisterApplicationParameters.RegisterApplicationTypeTimeoutSec
}

# CopyPackageTimeoutSec parameter overrides the value from the publish profile
if ($CopyPackageTimeoutSec)
{
    $PublishParameters['CopyPackageTimeoutSec'] = $CopyPackageTimeoutSec
}

# RegisterApplicationTypeTimeoutSec parameter overrides the value from the publish profile
if ($RegisterApplicationTypeTimeoutSec)
{
    $PublishParameters['RegisterApplicationTypeTimeoutSec'] = $RegisterApplicationTypeTimeoutSec
}

if ($IsUpgrade)
{
    $Action = "RegisterAndUpgrade"
    if ($DeployOnly)
    {
        $Action = "Register"
    }
    
    $UpgradeParameters = $publishProfile.UpgradeDeployment.Parameters

    if ($OverrideUpgradeBehavior -eq 'ForceUpgrade')
    {
        # Warning: Do not alter these upgrade parameters. It will create an inconsistency with Visual Studio's behavior.
        $UpgradeParameters = @{ UnmonitoredAuto = $true; Force = $true }
    }

    $PublishParameters['Action'] = $Action
    $PublishParameters['UpgradeParameters'] = $UpgradeParameters
    $PublishParameters['UnregisterUnusedVersions'] = $UnregisterUnusedApplicationVersionsAfterUpgrade

    Publish-UpgradedServiceFabricApplication @PublishParameters
}
else
{
    $Action = "RegisterAndCreate"
    if ($DeployOnly)
    {
        $Action = "Register"
    }

    $PublishParameters['Action'] = $Action
    $PublishParameters['OverwriteBehavior'] = $OverwriteBehavior
    $PublishParameters['SkipPackageValidation'] = $SkipPackageValidation

    if ($RemoteVMDeploy)
    {
    	$script = "$PSScriptRoot\PublishCloud-NewServiceFabricApplication.ps1"
	Write-Host ("Script: " + $script)

	. "$script"
	Publish-NewServiceFabricApplicationByUrl @PublishParameters
    }
    else
    {
    	Publish-NewServiceFabricApplication @PublishParameters
    }
}