#pragma once

#include "../Stdafx.h"
#include <msclr\marshal_cppstd.h>
#include "MinerManager.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace XDaggerMinerRuntimeCLI;


MinerManager::MinerManager() : MinerManager(false)
{

}

MinerManager::~MinerManager()
{
	gch.Free();
}

MinerManager::MinerManager(bool isFakeRun)
{
	this->_impl = new XDaggerMinerRuntime::MinerManager(isFakeRun);

	// Set Logger Callback
	LoggerCallbackFunc^ writLogFunc = gcnew LoggerCallbackFunc(this, &MinerManager::WriteLog);
	gch = GCHandle::Alloc(writLogFunc);
	IntPtr func = Marshal::GetFunctionPointerForDelegate(writLogFunc);
	LoggerCallbackC necb = static_cast<LoggerCallbackC>(func.ToPointer());
	this->_impl->setLoggerCallback(necb);

	//// GC::Collect();

	/*
	writLogFunc = gcnew LoggerCallback(this, &MinerManager::WriteLog);
	IntPtr func = Marshal::GetFunctionPointerForDelegate(writLogFunc);
	necb = static_cast<LoggerCallbackC>(func.ToPointer());
	*/
}

List<MinerDevice^>^ MinerManager::GetAllMinerDevices()
{
	List<MinerDevice^>^ resultList = gcnew List<MinerDevice^>();

	std::vector<XDaggerMinerRuntime::MinerDevice*> rawDevices = this->_impl->getAllMinerDevices();

	for (auto const &rawDevice : rawDevices)
	{
		String ^ deviceId = msclr::interop::marshal_as<System::String^>(rawDevice->getDeviceId());
		String ^ displayName = msclr::interop::marshal_as<System::String^>(rawDevice->getDisplayName());
		String ^ deviceVersion = msclr::interop::marshal_as<System::String^>(rawDevice->getDeviceVersion());
		String ^ driverVersion = msclr::interop::marshal_as<System::String^>(rawDevice->getDriverVersion());

		MinerDevice^ device = gcnew MinerDevice(deviceId, displayName, deviceVersion, driverVersion);
		resultList->Add(device);
	}

	return resultList;
}

String ^ MinerManager::QueryStatistics(int queryId)
{
	std::string result = this->_impl->queryStatistics(queryId);
	return msclr::interop::marshal_as<System::String^>(result);
}

void MinerManager::DoMining(String ^ poolAddress, String ^ walletAddress)
{
	std::string poolAddressStd = msclr::interop::marshal_as<std::string>(poolAddress);
	std::string walletAddressStd = msclr::interop::marshal_as<std::string>(walletAddress);
	
	this->_impl->doMining(poolAddressStd, walletAddressStd);
}

void MinerManager::WriteTestMessage()
{
	std::string msg = "WriteTestMessage from MinerManager.";
	WriteLog(0, 99, msg.c_str());
}

void MinerManager::SetLogger(LoggerBase ^ logger)
{
	this->_logger = logger;
}

void MinerManager::WriteLog(int level, int eventId, const char* message)
{
	if (this->_logger != nullptr)
	{
		this->_logger->WriteLog(level, eventId, msclr::interop::marshal_as<System::String^>(message));
	}
}
