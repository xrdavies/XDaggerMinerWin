#pragma once

//
// NOTE: This header file will be included in C++/CLR project, so DO NOT include boost/thread/mutex and Core headers here, use them in cpp file.
// 

#include <chrono>
#include <fstream>
#include <iostream>
#include <signal.h>
#include <random>

////#include <boost/algorithm/string.hpp>
////#include <boost/algorithm/string/trim_all.hpp>
////#include <boost/optional.hpp>

#include <stdio.h>
#include "stdafx.h"
#include "miner_device.h"
/// #include "../Core/Farm.h"
/// #include "Core/Exceptions.h"
/// #include "Core/Workers/CLMiner.h"

// using namespace XDag;

#ifdef __cplusplus
extern "C"
{
#endif

	typedef void(__stdcall * LoggerCallbackC)(int, int, const char*);

#ifdef __cplusplus
}
#endif

class XPool;

namespace XDag
{
	class Farm;
}

namespace XDaggerMinerRuntime
{
	//
	// The main methods for CLI to call
	//
	class NATIVE_LIB_EXPORT MinerManager {
	public:

		typedef enum LoggerLevel
		{
			LoggerLevel_Trace = 0,
			LoggerLevel_Error = 1,
			LoggerLevel_Warning = 2,
			LoggerLevel_Information = 3
		} LoggerLevel;



		MinerManager();
		MinerManager(bool isFakeRun);

		void setLoggerCallback(LoggerCallbackC loggerFunction);
		
		// Main Functions

		std::vector< MinerDevice* > getAllMinerDevices();

		// Query Status
		// queryId:		1: status	2. hashrate
		//
		std::string queryStatistics(int queryId);

		// Do Mining
		void doMining(std::string poolAddress, std::string walletAddress);

		void configureMiningDevice(int platformId, int deviceId);
		
	private:

		// void doBenchmark(MinerType type, unsigned warmupDuration = 15, unsigned trialDuration = 3, unsigned trials = 5);
		// void doMining(MinerType type, std::string& remote, unsigned recheckPeriod);
		void configureGpu();
		void configureCpu();
		///void fillRandomTask(XTaskWrapper *taskWrapper);
		
		
		// Loggers
		LoggerCallbackC _loggerCallback;
		void logTrace(int eventId, std::string message);
		void logError(int eventId, std::string message);
		void logWarning(int eventId, std::string message);
		void logInformation(int eventId, std::string message);

		void doRealMiningWork(std::string& poolAddress, std::string& walletAddress);
		void MinerManager::doFakeMiningWork();

		// Status: running, stopped, disconnected, connected
		std::string retrieveRunningStatus();

		// HashRate: e.g. "15.39"
		std::string retrieveHashrate();


		// Private members
		XPool * _xPool = nullptr;
		XDag::Farm * _farm = nullptr;


		bool _isRunning = false;
		bool _isConnected = false;


		// Mining options
		bool _running = true;
		
		// Device and Platform
		unsigned _openclPlatform = 0;
		unsigned _openclDeviceCount = 0;
		unsigned _openclDevices[16];
		unsigned _openclMiningDevices = 16;

		unsigned _localWorkSize = 128U;
		unsigned _globalWorkSizeMultiplier = true;
		//// unsigned _localWorkSize = CLMiner::_defaultLocalWorkSize;
		//// unsigned _globalWorkSizeMultiplier = CLMiner::_defaultGlobalWorkSizeMultiplier;

		unsigned _cpuMiningThreads = 0;
		bool _shouldListDevices = false;
		unsigned _openclSelectedKernel = 0;  ///< A numeric value for the selected OpenCL kernel
		/// 
		bool _useOpenClCpu = false;
		/// unsigned _globalWorkSizeMultiplier = XDag::CLMiner::_defaultGlobalWorkSizeMultiplier;
		/// unsigned _localWorkSize = XDag::CLMiner::_defaultLocalWorkSize;
		bool _useNvidiaFix = false;

		// Benchmarking params
		unsigned _benchmarkWarmup = 15;
		unsigned _benchmarkTrial = 3;
		unsigned _benchmarkTrials = 5;

		// Pool params
		std::string _poolUrl = "http://127.0.0.1:8545";
		unsigned _poolRetries = 0;
		unsigned _maxPoolRetries = 3;
		unsigned _poolRecheckPeriod = 2000;
		bool _poolRecheckSet = false;
		std::string _accountAddress;

		int _worktimeout = 180;
		bool _show_hwmonitors = false;

		std::string _fport = "";

		// Context
		bool _isFakeRun = true;


	};


}

