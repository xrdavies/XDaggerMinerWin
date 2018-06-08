[CmdletBinding()]
Param(
  [Parameter(Mandatory=$True)]
   [string]$Operation
)


Process {

	Function UninstallMinerService {

		Write-Host 'Checking Miner Service.'
		
		$service = Get-WmiObject -Class Win32_Service -Filter "Name = 'XDaggerMinerService'"
		if ($service -ne $null) 
		{
			Write-Host 'Miner Service Found, Stopping the service...'
	
		    $service | Stop-Service -Force
		    Start-Sleep -s 3
			
			Write-Host 'Miner Service Stopped.'
			
			# Uninstall the service
			Write-Host 'Deleting the service...'
			Start-Process -FilePath 'C:\Windows\Microsoft.NET/Framework64\v4.0.30319\InstallUtil.exe' -ArgumentList '/u .\XDaggerMinerService.exe' -NoNewWindow
			Write-Host 'Miner Service Deleted.'	
		}
		else
		{
			Write-Host 'Cannot find Miner Service.'
		}
	}

	Function StartMinerService {

		Write-Host 'Checking Miner Service.'
		
		$service = Get-WmiObject -Class Win32_Service -Filter "Name = 'XDaggerMinerService'"
		if ($service -ne $null) 
		{
			Write-Host 'Miner Service Found, Starting the service...'
	
		    $service | Start-Service
		    Start-Sleep -s 3
			
			Write-Host 'Miner Service Started.'
		}
		else
		{
			Write-Host 'Cannot find Miner Service.'
		}
	}
	
	Function StopMinerService {

		Write-Host 'Checking Miner Service.'
		
		$service = Get-WmiObject -Class Win32_Service -Filter "Name = 'XDaggerMinerService'"
		if ($service -ne $null) 
		{
			Write-Host 'Miner Service Found, Stopping the service...'
	
		    $service | Stop-Service -Force
		    Start-Sleep -s 3
			
			Write-Host 'Miner Service Stopped.'
		}
		else
		{
			Write-Host 'Cannot find Miner Service.'
		}
	}

	Function InstallMinerService {
		
		Write-Host 'Installing the Miner Service.'

		# Clean up first to make sure it does not exist
		UninstallMinerService

		Start-Process -FilePath 'C:\Windows\Microsoft.NET/Framework64\v4.0.30319\InstallUtil.exe' -ArgumentList '.\XDaggerMinerService.exe' -NoNewWindow
		Start-Sleep -s 3
		
		Write-Host 'Miner Service Installed.'
	}
	
	if ($Operation.Equals("InstallService", [System.StringComparison]::InvariantCultureIgnoreCase))
	{
		InstallMinerService
		return '{ "result":"0"}'
	}

	if ($Operation.Equals("UninstallService", [System.StringComparison]::InvariantCultureIgnoreCase))
	{
		InstallMinerService
		return '{ "result":"0"}'
	}

	if ($Operation.Equals("StartService", [System.StringComparison]::InvariantCultureIgnoreCase))
	{
		StartMinerService
		return '{ "result":"0"}'
	}

	if ($Operation.Equals("StopService", [System.StringComparison]::InvariantCultureIgnoreCase))
	{
		StopMinerService
		return '{ "result":"0"}'
	}
}

