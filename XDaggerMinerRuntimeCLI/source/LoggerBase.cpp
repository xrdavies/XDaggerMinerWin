#pragma once

#include "../Stdafx.h"

#include "LoggerBase.h"

using namespace XDaggerMinerRuntimeCLI;

LoggerBase::LoggerBase()
{


}

void LoggerBase::WriteLog(int level, int eventId, String ^ message)
{
	// Write the logs
}

void LoggerBase::WriteTrace(String ^ message)
{
	// Write the trace
}
