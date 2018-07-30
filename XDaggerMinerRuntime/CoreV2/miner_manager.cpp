
#pragma once
#include <stdio.h>  

#include <stdint.h>
#include "miner_manager.h"
#include "cl_device_utils.h"

#include "Core/Workers/CLMiner.h"
#include "Core/Farm.h"
#include "Core/Workers/XCpuMiner.h"
#include "XDagCore/XTaskProcessor.h"
#include "XDagCore/XPool.h"
#include "Utils/CpuInfo.h"
#include "Utils/Random.h"

using namespace std;
using namespace XDag;
using namespace XDaggerMinerRuntime;


MinerManager::MinerManager()
{
	this->_loggerCallback = nullptr;
}

MinerManager::MinerManager(bool isFakeRun)
{
	this->_loggerCallback = nullptr;
	this->_isFakeRun = isFakeRun;
}

void MinerManager::setLoggerCallback(LoggerCallbackC loggerFunction)
{
	this->_loggerCallback = loggerFunction;
}


std::vector< MinerDevice* > MinerManager::getAllMinerDevices()
{
	std::vector< MinerDevice* > resultList;

	std::vector<cl::Platform> platformList = DeviceUtils::GetPlatforms();

	if (platformList.empty())
	{
		logError(0, "No Platform founded on this device.");
		return resultList;
	}

	bool useAllOpenCLCompatibleDevices = true;
	cl_device_type type = useAllOpenCLCompatibleDevices
		? CL_DEVICE_TYPE_ALL
		: CL_DEVICE_TYPE_GPU | CL_DEVICE_TYPE_ACCELERATOR;

	int platformId = 0;
	for (auto const &platform : platformList) // access by reference to avoid copying
	{
		std::vector<cl::Device> devices;

		try
		{
			platform.getDevices(type, &devices);
		}
		catch (cl::Error const& err)
		{
			// if simply no devices found return empty vector
			if (err.err() != CL_DEVICE_NOT_FOUND)
			{
				throw err;
			}
		}

		if (devices.empty())
		{
			logInformation(0, "No Device founded on this Platform.");
			continue;
		}

		int deviceId = 0;
		for (auto const &device : devices)
		{
			std::string deviceName = device.getInfo<CL_DEVICE_NAME>();

			std::string deviceIdString = "_d_" + std::to_string(deviceId);
			deviceIdString = "p_" + std::to_string(platformId) + deviceIdString;

			std::string deviceVersion = device.getInfo<CL_DEVICE_VERSION>();
			std::string driverVersion = device.getInfo<CL_DRIVER_VERSION>();

			MinerDevice * deviceObject = new MinerDevice(deviceIdString, deviceName, deviceVersion, driverVersion);
			resultList.push_back(deviceObject);
			
			deviceId ++;
		}

		platformId ++;
	}

	return resultList;
}

void MinerManager::doMining(std::string poolAddress, std::string walletAddress)
{
	if (_isFakeRun)
	{
		doFakeMiningWork();
	}
	else
	{
		doRealMiningWork(poolAddress, walletAddress);
	}
}

void MinerManager::doRealMiningWork(std::string& poolAddress, std::string& walletAddress)
{
	if (_isRunning)
	{
		// Just return if it's already running
		logInformation(0, "Skipp this round since the manager is still running.");
		return;
	}

	//// configureGpu();

	XTaskProcessor taskProcessor;

	logTrace(0, "Initializing XPool.");
	_xPool = new XPool(walletAddress, poolAddress, &taskProcessor);
	
	logTrace(0, "Initializing Farm.");
	_farm = new XDag::Farm(&taskProcessor);
	//// Farm farm(&taskProcessor);

	if (!_xPool->Initialize())
	{
		logError(0, "Pool initialization error");
		return;
	}
	if (!_xPool->Connect())
	{
		logError(0, "Cannot connect to pool: " + poolAddress);
		return;
	}
	//wait a bit
	logTrace(0, "Wait for a bit...");
	this_thread::sleep_for(chrono::milliseconds(200));

	logTrace(0, "Create Farm and Add Seeker...");
	_farm->AddSeeker(XDag::Farm::SeekerDescriptor{ &CLMiner::Instances, [](unsigned index, XTaskProcessor* taskProcessor) { return new CLMiner(index, taskProcessor); } });

	if (!_farm->Start())
	{
		logError(0, "Failed to start mining");
		return;
	}

	logTrace(0, "Farm Started.");

	uint32_t iteration = 0;
	bool isConnected = false;
	_isRunning = true;

	while (_running)
	{
		if (!isConnected)
		{
			isConnected = _xPool->Connect();
			if (isConnected)
			{
				logInformation(0, "Pool Connected. Starting Farm now.");

				if (!_farm->Start())
				{
					logInformation(0, "Failed to restart mining");
					_isRunning = false;
					return;
				}

				logInformation(0, "Farm Started.");
			}
			else
			{
				logInformation(0, "Cannot connect to pool. Reconnection...");
				this_thread::sleep_for(chrono::milliseconds(5000));
				continue;
			}
		}
		else
		{
			logTrace(0, "Pool Connected.");
		}

		if (!_xPool->Interract())
		{
			logInformation(0, "Failed to get data from pool. Stopping this iteration...");

			_xPool->Disconnect();
			_farm->Stop();
			isConnected = false;
			_isRunning = false;

			logInformation(0, "Stoped the iteration.");

			return;
		}

		if (iteration > 0 && (iteration & 1) == 0)
		{
			auto mp = _farm->MiningProgress();
			//// minelog << mp;
		}

		logTrace(0, "Continue for next running iteration...");

		this_thread::sleep_for(chrono::milliseconds(_poolRecheckPeriod));
		++iteration;
	}

	_isRunning = false;
	_farm->Stop();

	logInformation(0, "Farm Stopped.");
}


void MinerManager::doFakeMiningWork()
{
	logTrace(0, "Fake Farm Started.");

	int iteration = 0;
	while (_running)
	{
		logTrace(0, "Fake Running...");

		this_thread::sleep_for(chrono::milliseconds(_poolRecheckPeriod));
		++iteration;
	}
}

void MinerManager::configureGpu()
{
	logTrace(0, "Configuring GPU...");
	if (_openclDeviceCount > 0)
	{
		CLMiner::SetDevices(_openclDevices, _openclDeviceCount);
		_openclMiningDevices = _openclDeviceCount;
	}

	if (!CLMiner::ConfigureGPU(
		_localWorkSize,
		_globalWorkSizeMultiplier,
		_openclPlatform,
		_useOpenClCpu))
	{
		logError(0, "Configure GPU Error.");
		return;
	}

	CLMiner::SetNumInstances(_openclMiningDevices);
	CLMiner::SetUseNvidiaFix(true);
	logInformation(0, "Configure GPU Completed.");
}

void MinerManager::configureCpu()
{
	logTrace(0, "Configuring CPU...");
	if (_cpuMiningThreads == 0)
	{
		_cpuMiningThreads = CpuInfo::GetNumberOfCpuCores();
	}

	XCpuMiner::SetNumInstances(_cpuMiningThreads);
	logInformation(0, "Configure CPU Completed.");
}

void MinerManager::configureMiningDevice(int platformId, int deviceId)
{
	_openclPlatform = platformId;
	_openclDeviceCount = 1;
	_openclDevices[0] = deviceId;
}

string MinerManager::queryStatistics(int queryId)
{
	switch (queryId)
	{
	case 1:
		return retrieveRunningStatus();
	case 2:
		return retrieveHashrate();
	default:
		return nullptr;
	}

	return nullptr;
}

std::string MinerManager::retrieveRunningStatus()
{
	if (_xPool == nullptr)
	{
		return "stopped";
	}

	if (!_xPool->Interract())
	{
		return "disconnected";
	}

	if (!_isRunning)
	{
		return "connected";
	}

	if (_farm == nullptr || !_farm->IsMining())
	{
		return "idle";
	}

	return "mining";
}

std::string MinerManager::retrieveHashrate()
{
	if (_farm == nullptr || !_farm->IsMining())
	{
		return "0";
	}

	WorkingProgress progress = _farm->MiningProgress();
	
	return std::to_string(progress.Rate());
}

void MinerManager::logInformation(int eventId, std::string message)
{
	if (this->_loggerCallback == nullptr)
	{
		throw new exception("logInformation : logger callback is null");
	}

	this->_loggerCallback(LoggerLevel_Information, eventId, message.c_str());
}

void MinerManager::logTrace(int eventId, std::string message)
{
	if (this->_loggerCallback == nullptr)
	{
		throw new exception("logInformation : logger callback is null");
	}

	this->_loggerCallback(LoggerLevel_Trace, eventId, message.c_str());
}

void MinerManager::logWarning(int eventId, std::string message)
{
	this->_loggerCallback(LoggerLevel_Warning, eventId, message.c_str());
}

void MinerManager::logError(int eventId, std::string message)
{
	this->_loggerCallback(LoggerLevel_Error, eventId, message.c_str());
}

