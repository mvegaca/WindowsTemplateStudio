#include "pch.h"
#include "ShellWindow.xaml.h"
#include "NavigationService.h"
#include "MainPage.xaml.h"
#if __has_include("ShellWindow.g.cpp")
#include "ShellWindow.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml;
using namespace Microsoft::UI::Xaml::Controls;
using namespace Microsoft::UI::Xaml::Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::WinUIDesktopCppApp::implementation
{
    ShellWindow::ShellWindow()
    {
        InitializeComponent();
        NavigationService::Initialize(ShellFrame());
    }
}
