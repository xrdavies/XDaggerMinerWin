#pragma once

#include <vector>
#include "cl_device_utils.h"
#include "Core/CL/cl2.hpp"


using namespace std;
using namespace XDaggerMinerRuntime;

std::vector<cl::Device> DeviceUtils::GetDevices(std::vector<cl::Platform> const& platforms, unsigned platformId, bool useAllOpenCLCompatibleDevices)
{
	std::vector<cl::Device> devices;
	size_t platform_num = std::min<size_t>(platformId, platforms.size() - 1);
	try
	{
		cl_device_type type = useAllOpenCLCompatibleDevices
			? CL_DEVICE_TYPE_ALL
			: CL_DEVICE_TYPE_GPU | CL_DEVICE_TYPE_ACCELERATOR;
		platforms[platform_num].getDevices(type, &devices);
	}
	catch (cl::Error const& err)
	{
		// if simply no devices found return empty vector
		if (err.err() != CL_DEVICE_NOT_FOUND)
		{
			throw err;
		}
	}
	return devices;
}


std::vector<cl::Platform> DeviceUtils::GetPlatforms()
{
	std::vector<cl::Platform> platforms;
	try
	{
		cl::Platform::get(&platforms);
	}
	catch (cl::Error const& err)
	{
#if defined(CL_PLATFORM_NOT_FOUND_KHR)
		if (err.err() == CL_PLATFORM_NOT_FOUND_KHR)
		{
			//// cwarn << "No OpenCL platforms found";
		}
		else
#endif
			throw err;
	}
	return platforms;
}