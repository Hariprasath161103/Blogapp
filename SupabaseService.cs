using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO; // Required for Stream and MemoryStream
using System;    // Required for Guid/Exception
using Supabase;
using Supabase.Gotrue;
using Supabase.Interfaces;

public class SupabaseService
{
    private Supabase.Client? _client;
    private bool _isInitialized = false;

    public User? CurrentUser => _client?.Auth.CurrentUser;

    public async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        _client = new Supabase.Client(
            "https://jlsaqfstepekeszlugfv.supabase.co",
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Impsc2FxZnN0ZXBla2Vzemx1Z2Z2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTY2MTk0NzAsImV4cCI6MjA3MjE5NTQ3MH0.RYgCcWYU_L-ZlYelpiv4cf-pwjowcBLhPXunowiVPLQ",
            new SupabaseOptions { AutoConnectRealtime = false }
        );

        await _client.InitializeAsync();
        _isInitialized = true;
    }

    public async Task<Session?> SignInAsync(string email, string password)
    {
        await EnsureInitializedAsync();
        var session = await _client!.Auth.SignIn(email, password);
        return session;
    }

    public async Task<User?> SignUpAsync(string email, string password, string name)
    {
        await EnsureInitializedAsync();
        var options = new SignUpOptions
        {
            Data = new Dictionary<string, object>
            {
                { "name", name }
            }
        };

        var response = await _client!.Auth.SignUp(email, password, options);
        return response.User;
    }

    public string GetUserMetadataField(string key)
    {
        if (CurrentUser?.UserMetadata != null && CurrentUser.UserMetadata.TryGetValue(key, out var value) && value is string stringValue)
        {
            return stringValue;
        }
        return string.Empty;
    }

    public string GetCurrentUserName() => GetUserMetadataField("name");

    public async Task SignOutAsync()
    {
        if (_client == null) return;
        await _client.Auth.SignOut();
    }

    public Dictionary<string, object>? GetRawUserMetadata() => CurrentUser?.UserMetadata;

    // --- FIXED IMAGE UPLOAD METHOD ---
    public async Task<string?> UploadImageAsync(Stream fileStream, string fileName)
    {
        try
        {
            // 1. Ensure client is connected
            await EnsureInitializedAsync();

            // 2. Create a unique filename
            var uniqueName = $"{Guid.NewGuid()}_{fileName}";

            // 3. Convert Stream to Byte Array
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // 4. Upload using the Byte Array
            await _client!.Storage
                .From("blog-images")
                .Upload(fileBytes, uniqueName);

            // 5. Get the Public URL
            var publicUrl = _client.Storage
                .From("blog-images")
                .GetPublicUrl(uniqueName);

            return publicUrl;
        }
        catch (Exception ex)
        {
            // CHANGED: Return the actual error message so we can see it in the notification
            Console.WriteLine($"Upload failed: {ex.Message}");
            // Return a distinct error string starting with "Error:"
            return $"Error: {ex.Message}";
        }
    }
}