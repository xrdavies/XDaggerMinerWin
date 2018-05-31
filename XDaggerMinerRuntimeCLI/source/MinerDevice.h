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
		MinerDevice(long deviceId, String ^ displayName);

		long GetDeviceId();
		String ^ GetDisplayName();

	private:
		long deviceId;
		String ^ displayName;

	};


};
