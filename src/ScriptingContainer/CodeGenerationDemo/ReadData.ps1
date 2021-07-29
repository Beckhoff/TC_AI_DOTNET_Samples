$script:targetAddress = $null #'TC3TestA1-CP67x' # Target to address
$script:userName = 'Administrator' 
#$script:password = '1'

Set-StrictMode -Version Latest # Setting Strict mode to prevent uninitialized variables

function main
{
    ###########################
	# Register Message Filter #
	###########################
    Register-XaeMessageFilter

    $localRoute = get-AdsRoute -Local # Get local route

    if (!$targetAddress)
    {
        $targetAddress = $localRoute.NetId
    }
	
	################
	# Select-Route #
	################
	$registeredRoutes = Get-AdsRoute # Get the registered routes

	$allRoutes = Get-AdsRoute -all # Perform Broadcast search
	$broadcastRoute = $allRoutes | Where { $_.Name -eq $script:targetAddress }
    #$targetRoute = $broadcastRoute # Route filtered via Broadcast search

	#Getting TargetRoute directly
    $targetRoute = Get-AdsRoute -All -Address $targetAddress # Search by HostName

    $isLocal = $targetRoute.IsLocal

	##########################
	# Add route if necessary #
	##########################
    if (!$targetRoute.IsLocal)
    {
	    if ($registeredRoutes -notcontains $targetRoute)
	    {
            $credential = Get-Credential -UserName $script:userName -Message "Password for '$($targetRoute)'"

            if ($credential)
            {
                Add-AdsRoute -InputObject $targetRoute -Credential $credential > $null
            }
	    }
    }

	######################
	# Test Target Access #
	######################
    $reachable = Test-AdsRoute -inputObject $targetRoute -Quiet -Port 10000 -ErrorAction SilentlyContinue # Port 10000 
    $plcReachable = Test-AdsRoute -inputObject $targetRoute -Quiet -Port 851 -ErrorAction SilentlyContinue

    if (!$reachable)
    {
        throw "Target '$targetRoute' is not reachable!"
    }

    if (!$plcReachable)
    {
        throw "PLC (851) not started on target '$targetRoute.Name'!"
    }

	#########################
	# Read data from Target #
	#########################
    $session = New-AdsSession -InputObject $targetRoute -Port 851
    $dataTypes = Get-AdsDataTypes -Session $session

    $symbols = Get-AdsSymbols -Session $session

    $systemInfo = $symbols | where InstancePath -eq 'TwinCAT_SystemInfoVarList'

    $appInfo = $systemInfo._AppInfo
    $taskInfo = $systemInfo._TaskInfo
    #$licInfo = $systemInfo._DummyLicenseInfo
    
    $projectName = $appInfo.ProjectName.ReadValue() # Type save Read!
    $projectTimeStamp = $appInfo.AppTimestamp.ReadValue()
    $taskCnt = $appInfo.TaskCnt.ReadValue()

    $targetInfo = Get-TcTargetInfo -InputObject $targetRoute

	#########################
	# Write Data to Console #
	#########################
    Write-Host ""
    Write-Host "Running Project '$projectName' (Timestamp: $projectTimeStamp, TaskCnt: $taskCnt)"
    Write-Host ""
    Write-Host "Name   : $($targetInfo.Target)"
    Write-Host "Type   : $($targetInfo.TargetType)"
    Write-Host "Version: $($targetInfo.TargetVersion)"
    
    Write-Host ""
    Write-Host "Symbols:"
    $symbols | format-table -AutoSize

    for($i = 1; $i -le $taskCnt; $i++)
    {
        Write-Host ""
        Write-Host "Task $i"
        Write-Host "======="

        $cycleTime = [TimeSpan]::FromTicks($taskInfo[$i].CycleTime.ReadValue())
        $cycleCount = $taskInfo[$i].CycleCount.ReadValue()
        $taskName = $taskInfo[$i].TaskName.ReadValue()
        $lastExecTime = [TimeSpan]::FromTicks($taskInfo[$i].LastExecTime.ReadValue())

        Write-Host "CycleTime   : $($cycleTime.TotalMilliseconds) ms"
        Write-Host "CycleCount  : $cycleCount"
        Write-Host "TaskName    : $taskName"
        Write-Host "LastExecTime: $($lastExecTime.TotalMilliseconds) ms"
    }
   
	################
	# Remove Route #
	################
    if (!$targetRoute.IsLocal)
    {
        $targetRoute | Remove-AdsRoute -Confirm:$false
    }

	#############################
	# Unregister Message Filter #
	#############################   
    Unregister-XaeMessageFilter
}
 
main