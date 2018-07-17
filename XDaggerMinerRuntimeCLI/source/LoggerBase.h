#pragma once

#include "../Stdafx.h"

#include <stdio.h>
#include <string>
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;


namespace XDaggerMinerRuntimeCLI {

	public ref class LoggerBase
	{
	public:
		LoggerBase();

		virtual void WriteLog(int level, int eventId, String ^ message);
		virtual void WriteTrace(String ^ message);

	protected:

		const int Level_Trace = 0;
		const int Level_Error = 1;
		const int Level_Warning = 2;
		const int Level_Information = 3;
	};


}

