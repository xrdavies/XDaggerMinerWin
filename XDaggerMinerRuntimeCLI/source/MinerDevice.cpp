
#include "../Stdafx.h"
#include "MinerDevice.h"

using namespace XDaggerMinerRuntimeCLI;

MinerDevice::MinerDevice()
{

}

MinerDevice::MinerDevice(long deviceId, String ^ displayName)
{
	this->deviceId = deviceId;
	this->displayName = displayName;

}

long MinerDevice::GetDeviceId()
{
	return this->deviceId;
}

String ^ MinerDevice::GetDisplayName()
{
	return this->displayName;
}