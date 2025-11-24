using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;

public class AuthService
{
    private readonly IJSRuntime _js;

    public AuthService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SetUserAsync(object user)
    {
        var json = JsonSerializer.Serialize(user);
        await _js.InvokeVoidAsync("localStorage.setItem", "user", json);
        await _js.InvokeVoidAsync("window.dispatchEvent", "userChanged");
    }

    public async Task<string?> GetUserJsonAsync()
    {
        return await _js.InvokeAsync<string>("localStorage.getItem", "user");
    }

    public async Task ClearUserAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "user");
        await _js.InvokeVoidAsync("window.dispatchEvent", "userChanged");
    }
}
