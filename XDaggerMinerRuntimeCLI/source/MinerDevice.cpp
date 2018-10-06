
#include "../Stdafx.h"
#include "MinerDevice.h"

using namespace XDaggerMinerRuntimeCLI;

MinerDevice::MinerDevice()
{

}

MinerDevice::MinerDevice(String ^ deviceId, String ^ displayName, String ^ deviceVersion, String ^ driverVersion)
{
	this->deviceId = deviceId;
	this->displayName = displayName;
	this->deviceVersion = deviceVersion;
	this->driverVersion = driverVersion;
}

String ^ MinerDevice::GetDeviceId()
{
	return this->deviceId;
}

String ^ MinerDevice::GetDisplayName()
{
	return this->displayName;
}

String ^ MinerDevice::GetDeviceVersion()
{
	return this->deviceVersion;
}

String ^ MinerDevice::GetDriverVersion()
{
	return this->driverVersion;
}

bool MinerDevice::IsMatchId(String ^ deviceId)
{
	if (deviceId == nullptr)
	{
		return false;
	}

	return this->deviceId->Equals(deviceId, System::StringComparison::InvariantCultureIgnoreCase);
}