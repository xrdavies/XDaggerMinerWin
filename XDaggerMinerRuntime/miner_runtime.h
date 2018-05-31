#pragma once

#include <ctime>
#include <sstream>
#include <vector>

#include "stdafx.h"

#include "CoreV2/miner_manager.h"
#include "CoreV2/miner_device.h"

namespace XDaggerMinerRuntimeTest
{
	//
	// This is a test class for C++/CLR calling
	//
	class NATIVE_LIB_EXPORT Runtime {
	public:
		Runtime();

		int getDevices();

		int getValue();

	private:
		int m_value;
	};

	class NATIVE_LIB_EXPORT DeviceGPU {

	public:
		std::string displayName;
		int deviceId;
	};

	//
	// The main methods for CLI to call
	//
	class NATIVE_LIB_EXPORT MinerManagerTest {
	public:
		MinerManagerTest();

		void execute();

		std::vector< DeviceGPU* > getAllGpuDevices();

	private:

		void doMining(std::string& remote, unsigned recheckPeriod);
		void configureGpu();

	};


}