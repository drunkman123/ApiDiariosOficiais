using Microsoft.Playwright;

namespace ApiDiariosOficiais.Infrastructure.BrowserManager;

public class BrowserManager
{
    private readonly object _lock = new();
    private IBrowser? _browser;
    private IPlaywright? _playwright;

    public async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser == null || !IsBrowserActive())
        {
            lock (_lock)
            {
                if (_browser == null || !IsBrowserActive())
                {
                    RestartBrowser();
                }
            }
        }

        return _browser;
    }

    public async Task ReopenBrowserAsync() //sem uso
    {
        lock (_lock)
        {
            if (_browser != null && !!IsBrowserActive())
            {
                _browser.CloseAsync().Wait();
            }

            _browser = _playwright!.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }).Result;
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _browser?.CloseAsync().Wait();
            _playwright?.Dispose();
        }
    }
    private bool IsBrowserActive()
    {
        try
        {
            // Attempt to create a page to check if the browser is active
            var page = _browser.NewPageAsync().Result;
            page.CloseAsync().Wait();
            return true;
        }
        catch
        {
            return false;
        }
    }
    private void RestartBrowser()
    {
        var playwright = Playwright.CreateAsync().Result;
        _browser = playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }).Result;
    }
}
