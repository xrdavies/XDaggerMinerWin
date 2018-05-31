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

/// #include "Core/Exceptions.h"
/// #include "Core/Workers/CLMiner.h"

extern "C"
{
	typedef void(__stdcall * LoggerCallback)(int, int, std::string);

}

namespace XDaggerMinerRuntime
{

	
	// TODO: Seems not useful anymore
	class NATIVE_LIB_EXPORT Logger {
	public:
		Logger();

	protected:

		void write();
	};


	//
	// The main methods for CLI to call
	//
	class NATIVE_LIB_EXPORT MinerManager {
	public:
		MinerManager();
		MinerManager(bool isFakeRun);

		void setLogCallback(LoggerCallback loggerFunction);
		
		// Main Functions

		std::vector< MinerDevice* > getAllMinerDevices();

		//// void doMining(std::string& remote, unsigned recheckPeriod);	// DEPRECATED
		void doMining(std::string poolAddress, std::string walletAddress);

	private:

		
		// void doBenchmark(MinerType type, unsigned warmupDuration = 15, unsigned trialDuration = 3, unsigned trials = 5);
		// void doMining(MinerType type, std::string& remote, unsigned recheckPeriod);
		void configureGpu();
		void configureCpu();
		///void fillRandomTask(XTaskWrapper *taskWrapper);
		
		
		// Loggers
		LoggerCallback _logCallback;
		void logInformation(int eventId, std::string message);
		void logWarning(int eventId, std::string message);
		void logError(int eventId, std::string message);

		void doRealMiningWork();
		void MinerManager::doFakeMiningWork();



		// Mining options
		bool _running = true;
		unsigned _openclPlatform = 0;
		unsigned _cpuMiningThreads = 0;
		///unsigned _openclMiningDevices = MAX_CL_DEVICES;
		bool _shouldListDevices = false;
		unsigned _openclSelectedKernel = 0;  ///< A numeric value for the selected OpenCL kernel
		unsigned _openclDeviceCount = 0;
		/// unsigned _openclDevices[MAX_CL_DEVICES];
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

