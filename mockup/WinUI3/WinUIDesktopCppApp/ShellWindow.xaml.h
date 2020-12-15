#pragma once

#include "ShellWindow.g.h"

namespace winrt::WinUIDesktopCppApp::implementation
{
    struct ShellWindow : ShellWindowT<ShellWindow>
    {
        ShellWindow();        
    };
}

namespace winrt::WinUIDesktopCppApp::factory_implementation
{
    struct ShellWindow : ShellWindowT<ShellWindow, implementation::ShellWindow>
    {
    };
}
