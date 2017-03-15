#include "pch.h"
#include "WatsonComponent.h"

using namespace MobileCenterNative;
using namespace Platform;

WatsonComponent::WatsonComponent()
{
}

void WatsonComponent::Start(Platform::String^ appSecret)
{
	WerRegisterCustomMetadata(TEXT("VSMCAppSecret"), const_cast<LPWSTR>(appSecret->Data()));
}
