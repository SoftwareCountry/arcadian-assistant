# ------------------------------------------------------------
# Copyright (c) Microsoft Corporation.  All rights reserved.
# Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
# ------------------------------------------------------------

function Publish-NewServiceFabricApplicationByUrl
{
    <#
    .SYNOPSIS 
    Publishes a new Service Fabric application type to Service Fabric cluster.

    .DESCRIPTION
    This script registers & creates a Service Fabric application.

    .NOTES
    Connection to service fabric cluster should be established by using 'Connect-ServiceFabricCluster' before invoking this cmdlet.
    WARNING: This script creates a new Service Fabric application in the cluster. If OverwriteExistingApplication switch is provided, it deletes any existing application in the cluster with the same name.

    .PARAMETER ApplicationPathAsUrl

    .PARAMETER ApplicationManifestPath

    .PARAMETER ApplicationParameterFilePath
    Path to the application parameter file which contains Application Name and application parameters to be used for the application.    

    .PARAMETER ApplicationName
    Name of Service Fabric application to be created. If value for this parameter is provided alongwith ApplicationParameterFilePath it will override the Application name specified in ApplicationParameter  file.

    .PARAMETER Action
    Action which this script performs. Available Options are Register, Create, RegisterAndCreate. Default Action is RegisterAndCreate.

    .PARAMETER ApplicationParameter
    Hashtable of the Service Fabric application parameters to be used for the application. If value for this parameter is provided, it will be merged with application parameters
    specified in ApplicationParameter file. In case a parameter is found in application parameter file and on commandline, commandline parameter will override the one specified in application parameter file.

    .PARAMETER OverwriteBehavior
    Overwrite Behavior if an application exists in the cluster with the same name. Available Options are Never, Always, SameAppTypeAndVersion. 
    Never will not remove the existing application. This is the default behavior.
    Always will remove the existing application even if its Application type and Version is different from the application being created. 
    SameAppTypeAndVersion will remove the existing application only if its Application type and Version is same as the application being created.

    .PARAMETER SkipPackageValidation
    Switch signaling whether the package should be validated or not before deployment.

    .PARAMETER CopyPackageTimeoutSec
    Timeout in seconds for copying application package to image store. Default is 600 seconds.

    .PARAMETER CompressPackage
    Switch signaling whether the package should be compressed or not before deployment.

    .PARAMETER RegisterApplicationTypeTimeoutSec
    Timeout in seconds for registering application type. Default is 600 seconds.

    .PARAMETER UnregisterApplicationTypeTimeoutSec
    Timeout in seconds for unregistering application type. Default is 600 seconds.

    .EXAMPLE
    Publish-NewServiceFabricApplication -ApplicationPackagePath 'pkg\Debug' -ApplicationParameterFilePath 'Local.xml'

    Registers & Creates an application with AppParameter file containing name of application and values for parameters that are defined in the application manifest.

    Publish-NewServiceFabricApplication -ApplicationPackagePath 'pkg\Debug' -ApplicationName 'fabric:/Application1'

    Registers & Creates an application with the specified application name.

    #>

    [CmdletBinding(DefaultParameterSetName="ApplicationName")]  
    Param
    (
        [Parameter(Mandatory=$true,ParameterSetName="ApplicationName")]
        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
	[String]$ApplicationPathAsUrl,

        [Parameter(Mandatory=$true,ParameterSetName="ApplicationName")]
        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
	[String]$ApplicationManifestPath,

        [Parameter(Mandatory=$true,ParameterSetName="ApplicationParameterFilePath")]
        [String]$ApplicationParameterFilePath,    

        [Parameter(Mandatory=$true,ParameterSetName="ApplicationName")]
        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [String]$ApplicationName,

        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [Parameter(ParameterSetName="ApplicationName")]
        [ValidateSet('Register','Create','RegisterAndCreate')]
        [String]$Action = 'RegisterAndCreate',

        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [Parameter(ParameterSetName="ApplicationName")]
        [Hashtable]$ApplicationParameter,

        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [Parameter(ParameterSetName="ApplicationName")]
        [ValidateSet('Never','Always','SameAppTypeAndVersion')]
        [String]$OverwriteBehavior = 'Never',

        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [Parameter(ParameterSetName="ApplicationName")]
        [Switch]$SkipPackageValidation,

        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [Parameter(ParameterSetName="ApplicationName")]
        [int]$CopyPackageTimeoutSec = 600,

        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [Parameter(ParameterSetName="ApplicationName")]
        [Switch]$CompressPackage,

        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [Parameter(ParameterSetName="ApplicationName")]
        [int]$RegisterApplicationTypeTimeoutSec = 600,

        [Parameter(ParameterSetName="ApplicationParameterFilePath")]
        [Parameter(ParameterSetName="ApplicationName")]
        [int]$UnregisterApplicationTypeTimeoutSec = 600
    )

    Write-Host ("Application URL: $ApplicationPathAsUrl")

    $AppPkgPathToUse = $ApplicationPathAsUrl

    Write-Host ("Application parameter file path: $ApplicationParameterFilePath")

    if ($PSBoundParameters.ContainsKey('ApplicationParameterFilePath') -and !(Test-Path $ApplicationParameterFilePath -PathType Leaf))
    {
        $errMsg = "$ApplicationParameterFilePath is not found."
        throw $errMsg
    }

    try
    {
        [void](Test-ServiceFabricClusterConnection)
    }
    catch
    {
        Write-Warning "Unable to verify connection to Service Fabric cluster."
        throw
    }

    Write-Host "Connection to remote cluster"

    # Get image store connection string
    $clusterManifestText = Get-ServiceFabricClusterManifest
    $imageStoreConnectionString = Get-ImageStoreConnectionStringFromClusterManifest ([xml] $clusterManifestText)

    Write-Host "Application manifest file: $ApplicationManifestPath"

    # If ApplicationName is not specified on command line get application name from Application Parameter file.
    if(!$ApplicationName)
    {
       $ApplicationName = Get-ApplicationNameFromApplicationParameterFile $ApplicationParameterFilePath
    }

    Write-Host "Application name: $ApplicationName"

    $names = Get-NamesFromApplicationManifest -ApplicationManifestPath $ApplicationManifestPath
    if (!$names)
    {
        Write-Warning "Unable to read Application Type and Version from application manifest file."
        return
    }

    Write-Host 'Register Service Fabric'

    if($Action.Equals("Register") -or $Action.Equals("RegisterAndCreate"))
    {
        # Apply OverwriteBehavior if an applciation with same name already exists.
        $app = Get-ServiceFabricApplication -ApplicationName $ApplicationName
        if ($app)
        {
            $removeApp = $false
            if($OverwriteBehavior.Equals("Never"))
            {
                $errMsg = "An application with name '$ApplicationName' already exists, its Type is '$($app.ApplicationTypeName)' and Version is '$($app.ApplicationTypeVersion)'.
                You must first remove the existing application before a new application can be deployed or provide a new name for the application."
                throw $errMsg
            }

            if($OverwriteBehavior.Equals("SameAppTypeAndVersion")) 
            {
                if($app.ApplicationTypeVersion -eq $names.ApplicationTypeVersion -and $app.ApplicationTypeName -eq $names.ApplicationTypeName)
                {
                    $removeApp = $true
                }
                else
                {
                    $errMsg = "An application with name '$ApplicationName' already exists, its Type is '$($app.ApplicationTypeName)' and Version is '$($app.ApplicationTypeVersion)'.
                    You must first remove the existing application before a new application can be deployed or provide a new name for the application."
                    throw $errMsg
                }             
            }

            if($OverwriteBehavior.Equals("Always"))
            {
                $removeApp = $true
            }            

            if($removeApp)
            {
                Write-Host "An application with name '$ApplicationName' already exists in the cluster with Application Type '$($app.ApplicationTypeName)' and Version '$($app.ApplicationTypeVersion)'. Removing it."

                try
                {
                    $app | Remove-ServiceFabricApplication -Force -ForceRemove
                }
                catch [System.TimeoutException]
                {
                    # Catch operation timeout and continue with force remove replica.
                }
                catch [System.Fabric.FabricElementNotFoundException]
                {
                    if ($_.Exception.ErrorCode -eq [System.Fabric.FabricErrorCode]::ApplicationNotFound)
                    {
                        # Ignore
                    }
                    else
                    {
                        throw
                    }
                }

                foreach ($node in Get-ServiceFabricNode)
                {
                    [void](Get-ServiceFabricDeployedReplica -NodeName $node.NodeName -ApplicationName $ApplicationName | Remove-ServiceFabricReplica -NodeName $node.NodeName -ForceRemove)
                }

                if($OverwriteBehavior.Equals("Always"))
                {                    
                    # Unregsiter AppType and Version if there are no other applciations for the Type and Version. 
                    # It will unregister the existing application's type and version even if its different from the application being created,
                    if((Get-ServiceFabricApplication | Where-Object {$_.ApplicationTypeVersion -eq $($app.ApplicationTypeVersion) -and $_.ApplicationTypeName -eq $($app.ApplicationTypeName)}).Count -eq 0)
                    {
                        Unregister-ServiceFabricApplicationType -ApplicationTypeName $($app.ApplicationTypeName) -ApplicationTypeVersion $($app.ApplicationTypeVersion) -Force -TimeoutSec $UnregisterApplicationTypeTimeoutSec
                    }
                }
            }
        }        

        $reg = Get-ServiceFabricApplicationType -ApplicationTypeName $names.ApplicationTypeName | Where-Object  { $_.ApplicationTypeVersion -eq $names.ApplicationTypeVersion }
        if ($reg)
        {
            Write-Host 'Application Type '$names.ApplicationTypeName' and Version '$names.ApplicationTypeVersion' was already registered with Cluster, unregistering it...'
            $reg | Unregister-ServiceFabricApplicationType -Force -TimeoutSec $UnregisterApplicationTypeTimeoutSec
            if(!$?)
            {
                throw "Unregistering of existing Application Type was unsuccessful."
            }
        }

        $applicationPackagePathInImageStore = $names.ApplicationTypeName

	Write-Host 'Application URL: ' + $ApplicationPathAsUrl + ' App name: ' + $names.ApplicationTypeName + ' Version: ' + $names.ApplicationTypeVersion
        Write-Host 'Registering application type from url...'
	Register-ServiceFabricApplicationType -ApplicationPackageDownloadUri $ApplicationPathAsUrl -ApplicationTypeName $names.ApplicationTypeName -ApplicationTypeVersion $names.ApplicationTypeVersion -TimeoutSec $RegisterApplicationTypeTimeoutSec
        if(!$?)
        {
            throw "Registration of application type failed."
        }

        Write-Host 'Application type registered!'

        # Wait for app registration to finish.
        $ready = $false
        $retryTimeInterval = 2
        $retryCount = $RegisterApplicationTypeTimeoutSec / $retryTimeInterval
        $prevStatusDetail = ""

        do
        {
            $appType = Get-ServiceFabricApplicationType -ApplicationTypeName $names.ApplicationTypeName -ApplicationTypeVersion $names.ApplicationTypeVersion

            if($appType.Status -eq "Available")
            {
                $ready = $true
            }
            elseif($appType.Status -eq "Failed")
            {
                if($appType.StatusDetails -ne "")
                {
                    Write-Host $appType.StatusDetails
                }

                throw "Registration of application type failed."
            }
            else
            {
                if($appType.StatusDetails -ne "")
                {
                    if($prevStatusDetail -ne $appType.StatusDetails)
                    {
                        Write-Host $appType.StatusDetails
                    }

                    $prevStatusDetail = $appType.StatusDetails
                }

                Start-Sleep -Seconds $retryTimeInterval
                $retryCount--
            }
        } while (!$ready -and $retryCount -gt 0)

        if(!$ready)
        {
            throw "Registration of application package is not completed in specified timeout of $RegisterApplicationTypeTimeoutSec seconds. Please consider increasing this timout by passing a value for RegisterApplicationTypeTimeoutSec parameter."
        }
        else
        {
            Write-Host "Application package is registered."
        }

        Write-Host 'Removing application package from image store...'
        Remove-ServiceFabricApplicationPackage -ApplicationPackagePathInImageStore $applicationPackagePathInImageStore -ImageStoreConnectionString $imageStoreConnectionString
    }

    if($Action.Equals("Create") -or $Action.Equals("RegisterAndCreate"))
    {
        Write-Host 'Creating application...'

        # If application parameters file is specified read values from and merge it with parameters passed on Commandline
        if ($PSBoundParameters.ContainsKey('ApplicationParameterFilePath'))
        {
           $appParamsFromFile = Get-ApplicationParametersFromApplicationParameterFile $ApplicationParameterFilePath        
           if(!$ApplicationParameter)
            {
                $ApplicationParameter = $appParamsFromFile
            }
            else
            {
                $ApplicationParameter = Merge-Hashtables -HashTableOld $appParamsFromFile -HashTableNew $ApplicationParameter
            }    
        }
    
        New-ServiceFabricApplication -ApplicationName $ApplicationName -ApplicationTypeName $names.ApplicationTypeName -ApplicationTypeVersion $names.ApplicationTypeVersion -ApplicationParameter $ApplicationParameter
        if(!$?)
        {
            throw "Creation of application failed."
        }

        Write-Host 'Create application succeeded.'
    }
}