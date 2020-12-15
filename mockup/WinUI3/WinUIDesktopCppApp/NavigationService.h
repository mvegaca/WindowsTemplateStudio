#pragma once

using namespace winrt::Microsoft::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Interop;

namespace winrt::WinUIDesktopCppApp::implementation
{
	class NavigationService
	{
	private:
		static Frame navigationFrame;

	public:
		static void Initialize(Frame frame);
		static void Navigate(TypeName pageType);
	};
}

