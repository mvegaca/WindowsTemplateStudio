#pragma once

#include "ShellWindow.g.h"

namespace winrt::WinUIDesktopCppApp::implementation
{
    struct ShellWindow : ShellWindowT<ShellWindow>
    {
        ShellWindow();

        int32_t MyProperty();
        void MyProperty(int32_t value);
    };
}

namespace winrt::WinUIDesktopCppApp::factory_implementation
{
    struct ShellWindow : ShellWindowT<ShellWindow, implementation::ShellWindow>
    {
    };
}
