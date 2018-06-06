[CmdletBinding()]
Param(
  [Parameter(Mandatory=$True)]
   [string]$Operation
)


Process {

	Function CleanupMinerService {

		Write-Host 'Checking Miner Service.'
		
		$service = Get-WmiObject -Class Win32_Service -Filter "Name = 'XDaggerMinerService'"
		if ($service -ne $null) 
		{
			Write-Host 'Miner Service Found, Stopping the service...'
	
		    $service | Stop-Service -Force
		    Start-sleep -s 10
			
			Write-Host 'Miner Service Stopped.'
			
			# Uninstall the service
			Write-Host 'Deleting the service...'
			Start-Process -FilePath 'C:\Windows\Microsoft.NET/Framework64\v4.0.30319\InstallUtil.exe' -ArgumentList '/u D:\Temp\xdagger\XDaggerMinerWin\XDaggerMinerService.exe' -NoNewWindow
			Write-Host 'Miner Service Deleted.'	
		}	
	}

	Function InstallMinerService {
		
		Write-Host 'Installing the Miner Service.'
		Start-Process -FilePath 'C:\Windows\Microsoft.NET/Framework64\v4.0.30319\InstallUtil.exe' -ArgumentList 'D:\Temp\xdagger\XDaggerMinerWin\XDaggerMinerService.exe' -NoNewWindow
		Start-Sleep -s 3
		
		Write-Host 'Miner Service Installed.'
		
		$service = Get-WmiObject -Class Win32_Service -Filter "Name = 'XDaggerMinerService'"
		if ($service -ne $null) 
		{
			Write-Host 'Starting Miner Service...'
			$service | Start-Service
		    Write-Host 'Miner Service Started.'
			
		}
		else
		{
			Write-Host 'Did not find Miner Service.'
		}
	}
	
	if ($Operation.Equals("InstallService", [System.StringComparison]::InvariantCultureIgnoreCase))
	{
		InstallMinerService
		return '{ "result":"0"}'
	}

	
}

