#pragma once

#include <vector>

#include "Core/CL/cl2.hpp"

/*
#define OPENCL_PLATFORM_UNKNOWN 0
#define OPENCL_PLATFORM_NVIDIA  1
#define OPENCL_PLATFORM_AMD     2
#define OPENCL_PLATFORM_CLOVER  3

#define CL_USE_DEPRECATED_OPENCL_1_2_APIS true
#define CL_HPP_ENABLE_EXCEPTIONS true
#define CL_HPP_CL_1_2_DEFAULT_BUILD true
#define CL_HPP_TARGET_OPENCL_VERSION 120
#define CL_HPP_MINIMUM_OPENCL_VERSION 120

#define MAX_CL_DEVICES 16

// macOS OpenCL fix:
#ifndef CL_DEVICE_COMPUTE_CAPABILITY_MAJOR_NV
#define CL_DEVICE_COMPUTE_CAPABILITY_MAJOR_NV       0x4000
#endif

#ifndef CL_DEVICE_COMPUTE_CAPABILITY_MINOR_NV
#define CL_DEVICE_COMPUTE_CAPABILITY_MINOR_NV       0x4001
#endif
*/

namespace XDaggerMinerRuntime
{
	class DeviceUtils {
	public:
		DeviceUtils();

		static std::vector<cl::Platform> GetPlatforms();

		static std::vector<cl::Device> GetDevices(std::vector<cl::Platform> const& platforms, unsigned platformId, bool useAllOpenCLCompatibleDevices);


	private:


	};
}
