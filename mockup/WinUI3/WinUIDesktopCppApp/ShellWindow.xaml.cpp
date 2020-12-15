#include "pch.h"
#include "ShellWindow.xaml.h"
#if __has_include("ShellWindow.g.cpp")
#include "ShellWindow.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::WinUIDesktopCppApp::implementation
{
    ShellWindow::ShellWindow()
    {
        InitializeComponent();
    }

    int32_t ShellWindow::MyProperty()
    {
        throw hresult_not_implemented();
    }

    void ShellWindow::MyProperty(int32_t /* value */)
    {
        throw hresult_not_implemented();
    }
}
