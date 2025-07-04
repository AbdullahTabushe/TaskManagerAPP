using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Frontend.Models;

namespace TaskManager.Frontend.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private const string TOKEN_KEY = "authToken";
        private const string ROLE_KEY = "userRole";

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/v1/Auth/login", loginDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (authResponse != null)
                    {
                        await _localStorage.SetItemAsync(TOKEN_KEY, authResponse.Token);
                        await _localStorage.SetItemAsync(ROLE_KEY, authResponse.Role);
                        
                        // Set the authorization header for future requests
                        _httpClient.DefaultRequestHeaders.Authorization = 
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.Token);
                    }
                    return authResponse;
                }
                else
                {
                    // Log the status code and response content for debugging
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed: {response.StatusCode} - {errorContent}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex.Message}");
                return null;
            }
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserCreateDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/v1/Auth/register", createDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (authResponse != null)
                    {
                        await _localStorage.SetItemAsync(TOKEN_KEY, authResponse.Token);
                        await _localStorage.SetItemAsync(ROLE_KEY, authResponse.Role);
                        
                        // Set the authorization header for future requests
                        _httpClient.DefaultRequestHeaders.Authorization = 
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.Token);
                    }
                    return authResponse;
                }
                else
                {
                    // Log the status code and response content for debugging
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Registration failed: {response.StatusCode} - {errorContent}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration exception: {ex.Message}");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TOKEN_KEY);
            await _localStorage.RemoveItemAsync(ROLE_KEY);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            if (string.IsNullOrEmpty(token))
                return false;

            // Check if token is expired
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                return jsonToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(TOKEN_KEY);
        }

        public async Task<string?> GetUserRoleAsync()
        {
            return await _localStorage.GetItemAsync<string>(ROLE_KEY);
        }
        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            if (string.IsNullOrEmpty(token)) return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                return jsonToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetUsernameAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                // Look for the username in the standard Name claim type
                var username = jsonToken.Claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" ||
                    c.Type == "name" ||
                    c.Type == "username" ||
                    c.Type == "sub" ||
                    c.Type == "unique_name")?.Value;

                return username ?? "User";
            }
            catch
            {
                return "User";
            }
        }


    }
} 