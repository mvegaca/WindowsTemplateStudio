using System;

namespace WinUIUWPApp.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
