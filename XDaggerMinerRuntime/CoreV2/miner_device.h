#pragma once

#include "stdafx.h"
#include <string>

namespace XDaggerMinerRuntime
{
	//
	// The main methods for CLI to call
	//
	class NATIVE_LIB_EXPORT MinerDevice {
	public:
		MinerDevice();
		MinerDevice(std::string deviceId, std::string deviceName, std::string deviceVersion, std::string driverVersion);

		std::string getDisplayName();
		std::string getDeviceId();
		std::string getDeviceVersion();
		std::string getDriverVersion();

	private:

		std::string _displayName;
		std::string _deviceId;
		std::string _deviceVersion;
		std::string _driverVersion;
	};
}
