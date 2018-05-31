#pragma once

#include "../Stdafx.h"

#include <stdio.h>
#include <string>
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;


typedef void(__stdcall * LoggerCallbackFunc)(int, int, std::string);


namespace XDaggerMinerRuntimeCLI {


	[UnmanagedFunctionPointer(CallingConvention::StdCall)]
	public delegate void LoggerCallback(int, int, std::string);

	public ref class LoggerBase
	{
	public:
		LoggerBase();

		virtual void WriteLog(int level, int eventId, String ^ message);

	private:

	};


}

