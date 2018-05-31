#pragma once

#include "miner_device.h"

using namespace XDaggerMinerRuntime;

MinerDevice::MinerDevice()
{
	this->_displayName = "";
	this->_deviceId = 0;
}

MinerDevice::MinerDevice(std::string displayName, int deviceId)
{
	this->_displayName = displayName;
	this->_deviceId = deviceId;
}

std::string MinerDevice::getDisplayName()
{
	return _displayName;
}

int MinerDevice::getDeviceId()
{
	return _deviceId;
}
