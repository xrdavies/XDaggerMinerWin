
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
	this->_logCallback = nullptr;
}

MinerManager::MinerManager(bool isFakeRun)
{
	this->_logCallback = nullptr;
	this->_isFakeRun = isFakeRun;
}

void MinerManager::setLogCallback(LoggerCallback loggerFunction)
{
	this->_logCallback = loggerFunction;
}

std::vector< MinerDevice* > MinerManager::getAllMinerDevices()
{
	std::vector< MinerDevice* > resultList;

	std::vector<cl::Platform> platformList = DeviceUtils::GetPlatforms();

	if (platformList.empty())
	{
		logError(0, "No Platform founded on this device.");
		//// XCL_LOG("No OpenCL platforms found.");
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

	XTaskProcessor taskProcessor;
	XPool pool(walletAddress, poolAddress, &taskProcessor);

	Farm farm(&taskProcessor);

	if (!pool.Initialize())
	{
		logError(0, "Pool initialization error");
		return;
	}
	if (!pool.Connect())
	{
		logError(0, "Cannot connect to pool: " + poolAddress);
		return;
	}
	//wait a bit
	logInformation(0, "Wait for a bit...");
	this_thread::sleep_for(chrono::milliseconds(200));

	logInformation(0, "Create Farm and Add Seeker...");
	farm.AddSeeker(Farm::SeekerDescriptor{ &CLMiner::Instances, [](unsigned index, XTaskProcessor* taskProcessor) { return new CLMiner(index, taskProcessor); } });

	if (!farm.Start())
	{
		logError(0, "Failed to start mining");
		return;
	}

	logInformation(0, "Farm Started.");

	uint32_t iteration = 0;
	bool isConnected = true;
	_isRunning = true;

	while (_running)
	{
		if (!isConnected)
		{
			isConnected = pool.Connect();
			if (isConnected)
			{
				if (!farm.Start())
				{
					logError(0, "Failed to restart mining");
					_isRunning = false;
					return;
				}
			}
			else
			{
				logError(0, "Cannot connect to pool. Reconnection...");
				this_thread::sleep_for(chrono::milliseconds(5000));
				continue;
			}
		}

		if (!pool.Interract())
		{
			pool.Disconnect();
			farm.Stop();
			isConnected = false;
			_isRunning = false;
			logError(0, "Failed to get data from pool. Stop this iteration...");
			return;
		}

		if (iteration > 0 && (iteration & 1) == 0)
		{
			auto mp = farm.MiningProgress();
			//// minelog << mp;
		}

		this_thread::sleep_for(chrono::milliseconds(_poolRecheckPeriod));
		++iteration;
	}

	_isRunning = false;
	farm.Stop();
}


void MinerManager::doFakeMiningWork()
{
	logInformation(0, "Fake Farm Started.");

	int iteration = 0;
	while (_running)
	{
		logInformation(0, "Fake Running...");

		this_thread::sleep_for(chrono::milliseconds(_poolRecheckPeriod));
		++iteration;
	}
}

void MinerManager::logInformation(int eventId, std::string message)
{
	this->_logCallback(0, eventId, message);
}

void MinerManager::logWarning(int eventId, std::string message)
{
	this->_logCallback(1, eventId, message);
}

void MinerManager::logError(int eventId, std::string message)
{
	this->_logCallback(2, eventId, message);
}

