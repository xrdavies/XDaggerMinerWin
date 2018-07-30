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


#pragma unmanaged

typedef void(__stdcall * LoggerCallbackC)(int, int, const char*);

#pragma managed

namespace XDaggerMinerRuntimeCLI {

	
	public ref class MinerManager
	{
	public:

		[UnmanagedFunctionPointer(CallingConvention::StdCall)]
		delegate void LoggerCallbackFunc(int, int, const char*);

		MinerManager();
		MinerManager(bool isFakeRun);

		~MinerManager();

		List<MinerDevice^>^ GetAllMinerDevices();

		
		void SetLogger(LoggerBase ^ logger);

		void DoMining(String ^ poolAddress, String ^ walletAddress);

		// Query Status
		// queryId:		1: status	2. hashrate
		//
		String ^ QueryStatistics(int queryId);

		void ConfigureMiningDevice(int platformId, int deviceId);

		void WriteTestMessage();

		void WriteLog(int level, int eventId, const char* message);
	private:
		// List<String^>^ dinosaurs = gcnew List<String^>();

		XDaggerMinerRuntime::MinerManager * _impl;
		LoggerBase ^ _logger;

		//// LoggerCallbackFunc^ writLogFunc;
		//// LoggerCallbackC necb;
		GCHandle gch;

	};






}
