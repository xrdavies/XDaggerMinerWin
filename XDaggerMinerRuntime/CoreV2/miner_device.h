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
		MinerDevice(std::string displayName, int deviceId);

		std::string getDisplayName();
		int getDeviceId();

	private:

		std::string _displayName;
		int _deviceId;
	};
}
