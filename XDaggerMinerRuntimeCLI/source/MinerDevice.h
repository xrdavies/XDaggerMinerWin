#include "../Stdafx.h"


using namespace System;
using namespace System::Collections::Generic;

namespace XDaggerMinerRuntimeCLI {

	///
	/// Miner Device, genetic class for all of the devices that could retreive from OpenCL
	/// 
	public ref class MinerDevice
	{
	public: 
		MinerDevice();
		MinerDevice(String ^ deviceId, String ^ displayName, String ^ deviceVersion, String ^ driverVersion);

		String ^ GetDeviceId();
		String ^ GetDisplayName();
		String ^ GetDeviceVersion();
		String ^ GetDriverVersion();

		bool IsMatchId(String ^ deviceId);

	private:
		String ^ deviceId;
		String ^ displayName;
		String ^ deviceVersion;
		String ^ driverVersion;

	};


};
