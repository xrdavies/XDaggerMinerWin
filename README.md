# XDaggerMinerWin


## Structure

Daemon.exe -->  RuntimeCLI.dll   <--  Service.exe
                    |                
                    V
                  Runtime
                    |
                    V
                  OpenCL
    
Runtime:
    This is the core library that manages Works and communicates with pool.
    
Runtime CLI:
    This is a C++/CLR project, that expose the necessary functions to dotNET framework to call.
    
Daemon:
    This is a utility project that helps communicates with XDaggerMinerManager for the management work:
    1) Provide API to install/deploy/start/stop the Miner Service;
    2) Provide API to write/read config.json file;
    3) Provide API to retrieve Machine Information (e.g. GPU info);
    4) Provide API to report statistics;
    5) Provide API to upgrade the Miner Service;
            
Service:
    This is a Windows Service project, that could:
    1) Read the config.json file for the miner configurations;
    2) Load the RuntimeCLI to start the miner work;
    3) Write information/error to EventLog;
    
    

## Build Instruction


	1. Windows 10 and Visual Studio 2015.
	
	Note: If you Have Visual Studio 2017, you will need to install the component: Visual Studio Installer -> VC++ Components -> Visual Studio 2015 Build Tools (v140)
	

	2. Install Nvidia GPU Computing Tookit (CUDA) (https://developer.nvidia.com/cuda-downloads)

	3. Open the solution in VS, "Project" -> "Properties" -> "Configuration Properties" -> "C/C++" -> "General" -> "AdditionalIncludeDirectories", input the following:
		C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v9.1\include;
		
	Note: The v9.1 is the version number from CUDA that you downloaded from step 2.
		
	4. In "Project" -> "Properties" -> "Configuration Properties" -> "Linker" -> "General" -> "Additional Library Directories":
		C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v9.1\lib\x64
		
		
	5. Build.
	6. Run:

		cd x64/Debug
		
		XDaggerMinerDaemon.exe -l
		XDaggerMinerDaemon.exe --List
		
		XDaggerMinerDaemon.exe -c '{ "Wallet":"gKNRtSL1pUaTpzMuPMznKw49ILtP6qX3", "Pool":"pool.xdag.us:13654" }'
		XDaggerMinerDaemon.exe --Configure '{ "Wallet":"gKNRtSL1pUaTpzMuPMznKw49ILtP6qX3", "Pool":"pool.xdag.us:13654" DeviceId:"1" }'
		
		
		(Hard coded Pool:"pool.xdag.us:13654" )
		
