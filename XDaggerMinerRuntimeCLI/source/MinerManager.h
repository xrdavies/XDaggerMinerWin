#pragma once

#include <vector>
#include "../Stdafx.h"

#include "MinerDevice.h"
#include "..\..\XDaggerMinerRuntime\miner_runtime.h";

#include <stdio.h>
#include "LoggerBase.h"


using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;




namespace XDaggerMinerRuntimeCLI {

	
	public ref class MinerManager
	{
	public:
		MinerManager();
		MinerManager(bool isFakeRun);

		List<MinerDevice^>^ GetAllMinerDevices();

		
		void SetLogger(LoggerBase ^ logger);

		void DoMining(String ^ poolAddress, String ^ walletAddress);

	private:
		// List<String^>^ dinosaurs = gcnew List<String^>();

		XDaggerMinerRuntime::MinerManager * _impl;
		LoggerBase ^ _logger;

		void WriteLog(int level, int eventId, std::string message);

	};






}
