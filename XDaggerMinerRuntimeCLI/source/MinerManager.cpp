#pragma once

#include "../Stdafx.h"
#include <msclr\marshal_cppstd.h>
#include "MinerManager.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace XDaggerMinerRuntimeCLI;


MinerManager::MinerManager()
{
	this->_impl = new XDaggerMinerRuntime::MinerManager();
	
	// Set Logger Callback
	LoggerCallback^ writLogFunc = gcnew LoggerCallback(this, &MinerManager::WriteLog);
	GCHandle gc = GCHandle::Alloc(writLogFunc);
	IntPtr func = Marshal::GetFunctionPointerForDelegate(writLogFunc);
	LoggerCallbackFunc necb = static_cast<LoggerCallbackFunc>(func.ToPointer());
	this->_impl->setLogCallback(necb);

}

MinerManager::MinerManager(bool isFakeRun)
{
	this->_impl = new XDaggerMinerRuntime::MinerManager(isFakeRun);

	// Set Logger Callback
	LoggerCallback^ writLogFunc = gcnew LoggerCallback(this, &MinerManager::WriteLog);
	GCHandle gc = GCHandle::Alloc(writLogFunc);
	IntPtr func = Marshal::GetFunctionPointerForDelegate(writLogFunc);
	LoggerCallbackFunc necb = static_cast<LoggerCallbackFunc>(func.ToPointer());
	this->_impl->setLogCallback(necb);
}

List<MinerDevice^>^ MinerManager::GetAllMinerDevices()
{
	List<MinerDevice^>^ resultList = gcnew List<MinerDevice^>();

	std::vector<XDaggerMinerRuntime::MinerDevice*> rawDevices = this->_impl->getAllMinerDevices();

	for (auto const &rawDevice : rawDevices)
	{
		String ^ displayName = msclr::interop::marshal_as<System::String^>(rawDevice->getDisplayName());
		MinerDevice^ device = gcnew MinerDevice(rawDevice->getDeviceId(), displayName);
		resultList->Add(device);
	}

	return resultList;
}

void MinerManager::DoMining(String ^ poolAddress, String ^ walletAddress)
{
	std::string poolAddressStd = msclr::interop::marshal_as<std::string>(poolAddress);
	std::string walletAddressStd = msclr::interop::marshal_as<std::string>(walletAddress);

	this->_impl->doMining(poolAddressStd, walletAddressStd);
}

void MinerManager::SetLogger(LoggerBase ^ logger)
{
	this->_logger = logger;
}

void MinerManager::WriteLog(int level, int eventId, std::string message)
{
	if (this->_logger != nullptr)
	{
		this->_logger->WriteLog(level, eventId, msclr::interop::marshal_as<System::String^>(message));
	}
}
