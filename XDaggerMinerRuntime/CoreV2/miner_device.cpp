#pragma once

#include "miner_device.h"

using namespace XDaggerMinerRuntime;

MinerDevice::MinerDevice()
{
	this->_displayName = "";
	this->_deviceId = "";
	this->_deviceVersion = "";
	this->_driverVersion = "";
}

MinerDevice::MinerDevice(std::string deviceId, std::string displayName, std::string deviceVersion, std::string driverVersion)
{
	this->_deviceId = deviceId;
	this->_displayName = displayName;
	this->_deviceVersion = deviceVersion;
	this->_driverVersion = driverVersion;
}

std::string MinerDevice::getDisplayName()
{
	return _displayName;
}

std::string MinerDevice::getDeviceId()
{
	return _deviceId;
}

std::string MinerDevice::getDeviceVersion()
{
	return _deviceVersion;
}

std::string MinerDevice::getDriverVersion()
{
	return _driverVersion;
}
