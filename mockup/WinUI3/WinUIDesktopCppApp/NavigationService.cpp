#include "pch.h"
#include "NavigationService.h"

using namespace winrt::Microsoft::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Interop;

namespace winrt::WinUIDesktopCppApp::implementation
{
	Frame NavigationService::navigationFrame { nullptr };

	void NavigationService::Initialize(Frame frame)
	{
		navigationFrame = frame;
	}

	void NavigationService::Navigate(TypeName pageType)
	{
		navigationFrame.Navigate(pageType);
	}
}

