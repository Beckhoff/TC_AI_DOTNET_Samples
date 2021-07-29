<#	This powershell script demonstrates the CodeGenerationDemo with Powershell
	Cmdlets. Precondition to run the script is an valid installation of 
	TwinCAT 3.1 >= Build 4019.56a and the registration of the
	'TcXaeVs' poershell module (>= Version 0.3)

	The 'ProcessOrder.ps1' script executes the 'Orders.xml' and produces the specified configurations.
	
	.Example
	Process order with ID 1

	PS> Process-Order -orderId 1 

	.Example
	Process all arders
	
	PS> Process-Order
#>

param(
    # Optional parameter defining the orders to process
	[int[]] $orderId
)

Set-StrictMode -Version Latest # Setting Strict mode to prevent uninitialized variables
$ordersFileName = 'Orders.xml' # File name of the Orders table

function main
{
	<# .SYNOPSIS
		Script main routine
	#>

    ###########################
	# Register Message Filter #
	###########################
	Register-XaeMessageFilter

	#############################
	# Load Order table from XML #
	#############################

    $script:scriptFolder = Get-ScriptDirectory
	$script:ordersFile = Join-Path -Path $scriptFolder -ChildPath $ordersFileName
    $script:solutionsFolder = join-path -path $scriptFolder -ChildPath 'Solutions'

    $progressId = 0
    $progressActivity = "Processing orders"
    $i = 0

	[xml]$orderDocument = Get-Content -Path $ordersFile

	$configurations = $orderDocument.Root.AvailableConfigurations.Configuration
	$orders = $orderDocument.Root.MachineOrders.Order
	
	if ($orderId -eq $null)
	{
        $orderCount = $orders.Count

        $iStep = 100 / $orderCount

		######################
		# Process all orders #
		######################
		foreach($order in $orders)
		{
            $operation = "Processing Order $($order.Id)"
            Write-Progress -id $progressId -Activity $progressActivity  -CurrentOperation $operation -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i

			$configuration = $configurations | where { $_.id -eq $order.Configuration }
		    Process-Order -order:$order -configuration:$configuration
            $i+=$iStep
		}
        Write-Progress -id $progressId -Activity $progressActivity  -Completed -Status "$($i.ToString('N0')) % Complete"
	}
	else
	{
		############################
		# Process specified orders #
		############################
		foreach($o in $orderId)
		{


			$order = $orders | where { $_.id -eq $o }
			$configuration = $configurations | where { $_.id -eq $order.Configuration }

			Process-Order -order:$order -configuration:$configuration
		}
        
        $i = 100
        Write-Progress -id $progressId -Activity $progressActivity  -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i -Completed

	}

	#########################
	# Revoke Message filter #
	#########################
    Unregister-XaeMessageFilter
}

function Process-Order
{
	<#
	.SYNOPSIS
	Process a single Order
	.DESCRIPTION
	#>

    param (
        # Order Xml node object
		$order,
		# Order-Assigned Configuration Xml node object
        $configuration
    )

    $userControl = $true
	$orderName = "'Order $($order.id)'"    
    $progressId = 1
    $progressActivity = "Processing $orderName"

    $i = 0
    Write-Progress -id $progressId -Activity $progressActivity  -CurrentOperation "Open Project" -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i

    $configurationName = $configuration.Name
    $scriptName = $configuration.Script

    $orderDescription = $order.Description
    $configurationDescription = $configuration.Description

	$projectTemplate = $configuration['ProjectTemplate']

	##########################
	# Delete Solution Folder #
	##########################
    $solutionFolder = join-path -path $script:solutionsFolder -ChildPath $scriptName
    $exist = Test-Path -path $solutionFolder

    if ($exist)
    {
        remove-item -path $solutionFolder -recurse -force
    }

	############################################
	# Create Solution + System Manager Project #
	############################################
	$i = 5
    Write-Progress -id $progressId -Activity $progressActivity  -CurrentOperation "Create Solution" -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i
    $sysMan = New-XaeTcProject -name:$scriptName -path:$script:solutionsFolder -template:$projectTemplate -userControl:$userControl -visible -force

	###########################
	# Create IO-Configuration #
	###########################
    $i = 10
    Write-Progress -id $progressId -Activity $progressActivity  -CurrentOperation "Create hardware" -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i
    $device = Create-Hardware $sysMan $order $configuration

	########################
	# Generate Plc Project #
	########################
    $i = 50
    Write-Progress -id $progressId -Activity $progressActivity  -CurrentOperation "Create Plc" -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i
    $plcProject = Create-PlcProject $sysMan $order $configuration

	###############################
	# Create Motion configuration #
	###############################
    $i = 70
    Write-Progress -id $progressId -Activity $progressActivity  -CurrentOperation "Create Motion" -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i
    $motion = Create-Motion $sysMan $order $configuration

	####################
	# Compile Solution #
	####################    
    $i = 80
    Write-Progress -id $progressId -Activity $progressActivity -CurrentOperation "Building project" -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i
    Start-xaeBuild -dte $sysMan.DTE

	######################
	# Create IO-Mappings #
	######################
    $i = 90
    Write-Progress -id $progressId -Activity $progressActivity -CurrentOperation "Create mappings" -Status "$($i.ToString('N0')) % Complete" -PercentComplete $i
    Create-Mappings $sysMan $order $configuration
    
	#TODO: Scope part not working yet!
    #Create-Scope -path $solutionFolder -order $order -configuration $configuration -dte $sysMan.DTE
   
    Write-Progress -id $progressId -Activity $progressActivity  -Status "$($i.ToString('N0')) % Complete" -Completed
}


function Get-ScriptDirectory
{
	<#
	.SYNOPSIS
	Get the Script directory
	.DESCRIPTION
	#>

    $ret = ""

    $Invocation = (Get-Variable MyInvocation -Scope 2).Value
    $path = $Invocation.MyCommand.Path
    $ret = Split-Path $path
    return $ret
}


function Create-Hardware
{
	<#
	.SYNOPSIS
	Generate the Hardware configuration
	.DESCRIPTION
	#>

	param (
        $sysMan,
        $order,
        $configuration
	)

    $progressId = 2
    $progressActivity = "Create Hardware"

    $i = 0
    Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Scanning fieldbus ...' -PercentComplete $i
    
    $options = $configuration.SelectNodes('Options/Option')
    $hardware = $configuration.SelectSingleNode('Hardware')
    $device = $null

    if ($hardware)
    {

        [bool]$scanHardware = $options -contains 'ScanHardware'
        [bool]$simulateHardware = $options -Contains 'SimulateHardware'

        $etherCATDeviceTypes = @(
            $global:TcXaeVsConfiguration.SystemManager.DeviceTypes.EtherCAT_Master,
            $global:TcXaeVsConfiguration.SystemManager.DeviceTypes.EtherCAT_AutomationProtocol
        )

        $deviceName = "EtherCAT Master"

		################
		# Scan Devices #
		################
        $scannedDevices = $sysMan | Get-XaeEcDeviceInfos
        $deviceInfo = $null

        $i = 20
        Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Scanning fieldbus' -PercentComplete $i

        if ($scannedDevices)
        {
            $deviceInfos = $scannedDevices | Where { $etherCATDeviceTypes -contains $_.SubType }
            if ($deviceInfos.Count -gt 0)
            {
                $deviceInfo = $deviceInfos[0]
            }
        }

		#######################
		# Add EtherCAT Master #
		#######################
        $i = 30
        Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Creating fieldbus master ...' -PercentComplete $i

        $device = $sysMan | New-XaeEcDevice -name:$deviceName -type:EtherCAT_Master -deviceInfo:$deviceInfo
        $i = 50

        Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Creating topology ...' -PercentComplete $i

		##########################
		# Add EtherCAT Terminals #
		##########################
        $parent = $device
        $rootBoxes = $configuration.SelectNodes('Hardware/Box')
        $boxCount = Count-boxes -boxNodes $rootBoxes

        if ($boxCount -gt 0)
        {
            [int]$iStep = (100 - $i) / $boxCount

            foreach($box in $rootBoxes)
            {
                $i = $i + $istep
                $childBox = Create-Box -parent:$parent -boxInfo:$box -progressId $progressId -progressStep $iStep -progressValue $i
            }
        }
    
    }
    $i = 100
    Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -Completed
    $device
}

function Count-Boxes
{
	<#
	.Synopsis
	Count the number of boxes in XML (recursively)
	#>
    param (
		$boxNodes
	)

    $ret = 0

    foreach ($boxNode in $boxNodes)
    {
        $ret++
        $childNodes = $boxNode.SelectNodes("Box")
        $ret += Count-Boxes -boxNodes $childNodes
    }
    $ret
}

function Create-Box
{
	<#
	.SYNOPSIS
	Creates an EtherCAT Terminal
	#>

    param (
		# Parent Terminal
        $parent,
		# Xml Node of the terminal box to be createed
        $boxInfo,
		# ProgressId
        [int]$progressId,
		# Progress Step
        [int]$progressStep,
		# Current progress value
        [int]$progressValue
        )

	###################
	# Create Terminal #
	###################

    $box = New-XaeEcTerminal -name $boxInfo.Name -product:$boxInfo.type -parent:$parent
    $operation = "Creating box $($boxInfo.Name) ($($boxInfo.type))"

    Write-Progress -id $progressId -Activity $progressActivity -Status "%$progressValue Complete" -CurrentOperation $operation  -PercentComplete $progressValue

    $parent = $box

    if ($boxInfo['Box'])
    {
		######################################
		# Create Child Terminals Recursively #
		######################################
        foreach($childBoxInfo in $boxInfo.Box)
        {
            $i = $progressValue + $progressStep
            $childbox = Create-Box -parent $parent -boxInfo $childBoxInfo -progressId $progressId -progressStep $progressStep -progressValue $i
        }
    }
    $box
}

function Create-PlcProject
{
	<#
	.SYNOPSIS
	Generate the PLC Project
	#>

	param (
        # System manager
		$sysMan,
        # Order Xml Node
		$order,
		# Configuration Xml Node
        $configuration
	)

    $plcConfiguration = $configuration.SelectSingleNode('Plc')
    $project = $null

    if ($plcConfiguration)
    {
        $plcProjectName = $plcConfiguration.PlcProjectName

        $progressId = 2
        $progressActivity = "Generating plc project '$plcProjectName'"

		####################################
		# Create PLC Project from Template #
		####################################
        $i = 0
        Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Creating PLC project ...' -PercentComplete $i

        $emptyTemplate = "Empty PLC Template.plcproj"
        $project = $sysMan | New-XaeNestedProject -name:$plcProjectName -type:Plc -template:$emptyTemplate
	    $i = 5
        Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Setting project autostart ...' -PercentComplete $i

		##############################
		# Set Project Boot Autostart #
		##############################
        $project.BootProjectAutostart = $true

		########################
		# Generate Bootproject #
		########################
	    $i = 10
        Write-Progress -id $progressId -Activity $progressActivity  -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Generating boot project ...' -PercentComplete $i
        $project.GenerateBootProject($true)

	    $i = 15
        Write-Progress -id $progressId -Activity $progressActivity  -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Creating PLC project' -PercentComplete $i

        $plcProject = $project.LookupChild("$plcProjectName Project")

		############################
		# Generate Project content #
		############################
        $plcInfoObjects = $plcConfiguration.PlcObjects.PlcObject
	    $count = $plcInfoObjects.Count
	    [int]$step = (90 - $i) / $count

        foreach($plcInfoObject in $plcInfoObjects)
        {
		    $i += $step

            switch -regEx ($plcInfoObject.type)
            {
                'Placeholder' {
			        $name = $plcInfoObject.'#text'
				    Write-Progress -id $progressId -Activity $progressActivity  -Status "$($i.ToString('N0')) % Complete" -CurrentOperation "Adding placeholder '$name' ..." -PercentComplete $i
                    $ph = New-XaePlcPlaceholder -project:$plcProject -name:$name
                }

                'Library' {
                    $name = $plcInfoObject.'#text'
				    Write-Progress -id $progressId -Activity $progressActivity  -Status "$($i.ToString('N0')) % Complete" -CurrentOperation "Adding library '$name' ..." -PercentComplete $i
				    $lb = New-XaePlcLibrary -project:$plcProject -name $name
                }
                '^Datatype$|^POU$|^Gvl$>|^Itf$' {
				    $plcPath = $plcInfoObject.path
				    $templatePath = $plcInfoObject.type
				    $name = [IO.Path]::GetFileNameWithoutExtension($templatePath)

				    Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation "Adding worksheet '$plcPath $name' ..." -PercentComplete $i
                    $ws = Create-Worksheet -plcProject $plcProject -infoObject $plcInfoObject
                }
            }
        }

		############################
		# Generate additional task #
		############################
	    $i = 90
        Write-Progress -id $progressId -Activity $progressActivity  -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Adding tasks ...' -PercentComplete $i

        $rtTasks = $sysMan | Get-XaeTreeItem -standardFolder RTAdditionalTask
        #$rtTask = $rtTasks.CreateChild("PlcTask",$global:TcXaeVsConfiguration.SystemManager.TreeItemTypes.Task)

        $subType = $global:TcXaeVsConfiguration.SystemManager.TreeItemTypes.Task
        $rtTask = New-XaeChildItem -parent $rtTasks -name 'PlcTask' -typeId $subType

		####################
		# Connect PLC Task #
		####################
	    $i = 95
        Write-Progress -id $progressId -Activity $progressActivity  -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Adding tasks ...' -PercentComplete $i

        $plcTask = Get-XaeTreeItem -parent $plcProject -path 'PlcTask'

        if (!$plcTask)
        {
            #$plcTask = $plcProject.Createchild("PlcTask",$global:TcXaeVsConfiguration.SystemManager.TreeItemTypes.PlcTask,"","MAIN")
            $subType = $global:TcXaeVsConfiguration.SystemManager.TreeItemTypes.PlcTask
            $plcTask = New-XaeChildItem -parent $plcProject -name "PlcTask" -typeId $subType  -vInfo 'Main'
        }

        Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -Completed
    }
    $project
}

function Create-Worksheet
{
	<#
	.SYNOPSIS
	Create a single Plc Worksheet
	#>

    param(
        # Plc Project
		$plcProject,
        # Xml Info object
		$infoObject
    )

    $type = $infoObject.type
    $parent = $plcProject
    $ret = $null

	################################
	# Generate Folders recursively #
	################################
    if ($infoObject.HasAttribute('path'))
    {
        $s = $infoObject.path -split '[/\\^]'

        $actual = $null
        for($i= 0; $i -lt $s.Length; $i++)
        {
            $actual = Get-XaeTreeItem -parent $parent -path $s[$i]

            if (!$actual)
            {
                $actual = New-XaePlcFolder -parent:$parent -name $s[$i]
            }
            $parent = $actual
        }
    }

    $relPath = $infoObject.'#text'
    $docPath = join-path -path $script:scriptFolder -ChildPath $relPath

    [xml]$xml = Get-Content -Path $docPath
    $name = [Io.Path]::GetFileNameWithoutExtension($docPath)

	################################
	# Generate Worksheet in Folder #
	################################
    switch($type)
    {
            'Datatype' {
                $ret = New-XaePlcDataType -parent:$parent -Name $name -type Struct -xml $xml
            }
            'POU' {
                $ret = New-XaePlcPou -parent:$parent -Name $name -xml $xml -type FunctionBlock
            }
            'Gvl' {
                $ret = New-XaePlcGlobalVariables -parent:$parent -Name $name -xml $xml
            }
            'Itf' {
                $ret = New-XaePlcInterface -parent:$parent -Name $name -xml $xml
            }

    }
    $ret
}

function Create-Motion
{
	<#
	.SYNOPSIS
	Create the Motion configuration
	#>

	param (
        # System Manager
		$sysMan,
		# Order Xml Node
        $order,
		# Configuration Xml Node
        $configuration
	)

    $progressId = 2
    $progressActivity = 'Creating Motion configuration'

	$i = 0
    Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation 'Creating Motion tasks ...' -PercentComplete $i

    $motionConfiguration = $configuration.SelectSingleNode('Motion')
    
    if ($motionConfiguration)
    {
        $taskConfig = $configuration.SelectNodes('Motion/Task')

        $motion = $sysMan | Get-XaeTreeItem -standard Motion

		#############################
	    # Count the elements to add #
		#############################
        $count = 0

	    foreach($taskInfo in $taskConfig)
	    {
            $axisNodes = $taskInfo.SelectNodes('Axes.Axis')
        
            if ($axisNodes)
            {
		        $count += $axisNodes.Count
            }
		    $count++
	    }

	    [int]$step = (100 - $i) / $count

        foreach($taskInfo in $taskConfig)
        {
			######################
			# Create Motion Task #
			######################
            $name = $taskInfo.name
            $task = New-XaeChildItem -parent $motion -name $name -typeId 1
            $axes = Get-XaeTreeItem -parent $task -path 'Axes'

            $axisNodes = $taskInfo.SelectNodes('Axes.Axis')

            foreach($axisInfo in $axisNodes)
            {
                ###############
				# Create Axis #
				###############
				$axisName = $axisInfo.name
            
		        Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -CurrentOperation "Creating Axis '$axisName'..." -PercentComplete $i

			    $axis = New-XaeAxis -parent $axes -type Continous -name $axisName 

				#############################
				# Import Axis template data #
				#############################
                $template = $axisInfo.'#text'
                $templatePath = join-path -path $script:scriptfolder -ChildPath $template

                [xml]$xml = get-content -path $templatePath
                Import-XaeXml -node $axis -xml $xml

			    $i+=$step
            }
        }
	    Write-Progress -id $progressId -Activity $progressActivity -Status "$($i.ToString('N0')) % Complete" -Completed
    }
}

function Create-Mappings
{
	<#
	.SYNOPSIS
	Create the System Manager Mappings
	.DESCRIPTION
	#>

	param (
		# System Manager
        $sysMan,
		# Order XML Node
        $order,
		# Configuration XML Nodde
        $configuration
	)
    
    $relMappings = $configuration.SelectSingleNode('Mappings')

    if ($relMappings -and $relMappings.InnerText -ne "")
    {
		######################
		# Import IO-Mappings #
		######################
        $mappingsPath = join-path -path $script:scriptFolder -ChildPath $relMappings.InnerText 
        [xml]$xml = get-content -path $mappingsPath
        $sysMan | Set-XaeMapping -xml $xml
    }
}

function Create-Scope
{
	<#
	.SYNOPSIS
	Create the Measurement part (TODO)
	#>
    param (
        $path,
        $order,
        $configuration,
        $dte
    )

    $scopeConfig = $configuration.SelectSingleNode('Scope')

    if ($scopeConfig)
    {
        $template = $scopeConfig.Template
        $projectName = $scopeConfig.ProjectName
        
        $scopeProject = $dte.Solution.AddFromTemplate($template,$path,$projectName)
        $ytProject = $scopeProject.ProjectItems(1).Item(1).Object
        $ytProject.ShowControl()
        
        $chartsConfig = $scopeConfig.SelectNodes('Chart')

        if ($chartsConfig)
        {
            foreach($chartConfig in $chartsConfig)
            {
                Create-Chart $ytProject $chartConfig
            }

            #Process commands
            foreach($chartConfig in $chartsConfig)
            {
                $commands = $chartConfig.SelectNodes('Command')

                foreach($command in $commands)
                {
                    $commandType = $command.type

                    switch($commandType)
                    {
                        'StartRecord' { }
                        'StopRecord' { }
                        'Sleep' { }
                    }
                }
            }
        }
    }
}

function Create-Chart
{
    param (
        $ytProject,
        $chartConfig
    )

    $chartName = $chartConfig.name
    $chartProperties = $chartConfig.SelectNodes('Property')

    $chart = $null
    $l = $ytProject.CreateChild([ref] $chart)

    foreach($chartProperty in $chartProperties)
    {
        
    }
}

 
main
