#pragma once

namespace MobileCenterNative
{
	public ref class WatsonComponent sealed
	{
	public:
		WatsonComponent();
		void Start(Platform::String^ appSecret);
	};
}