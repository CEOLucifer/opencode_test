using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FaceRecognitionWeb.Models;

namespace FaceRecognitionWeb.Controllers;

public class FaceController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FaceController> _logger;
    private readonly HttpClient _httpClient;
    private const string PythonApiBase = "http://localhost:5000/api";

    public FaceController(ILogger<FaceController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public IActionResult Index()
    {
        return RedirectToAction("Login");
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Users()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var username = HttpContext.Session.GetString("Username");

        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        ViewBag.Username = username;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, IFormFile? image, string? webcamImage)
    {
        if (string.IsNullOrEmpty(username))
        {
            ViewBag.Error = "请提供用户名";
            return View();
        }

        string? base64Image = null;

        if (!string.IsNullOrEmpty(webcamImage))
        {
            base64Image = webcamImage;
        }
        else if (image == null || image.Length == 0)
        {
            ViewBag.Error = "请提供照片";
            return View();
        }
        else
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                base64Image = Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"图片处理失败: {ex.Message}";
                return View();
            }
        }

        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { username, image = base64Image }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{PythonApiBase}/register", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (result.GetProperty("status").GetString() == "success")
            {
                ViewBag.Success = "注册成功！请登录";
                return View("Login");
            }
            else
            {
                var message = result.GetProperty("message").GetString();
                ViewBag.Error = message ?? "注册失败";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"连接失败: {ex.Message}";
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(IFormFile? image, string? webcamImage)
    {
        string? base64Image = null;

        if (!string.IsNullOrEmpty(webcamImage))
        {
            base64Image = webcamImage;
        }
        else if (image == null || image.Length == 0)
        {
            ViewBag.Error = "请提供照片";
            return View();
        }
        else
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                base64Image = Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"图片处理失败: {ex.Message}";
                return View();
            }
        }

        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { image = base64Image }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{PythonApiBase}/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (result.GetProperty("status").GetString() == "success")
            {
                var userId = result.GetProperty("user_id").GetInt32();
                var username = result.GetProperty("username").GetString();

                HttpContext.Session.SetInt32("UserId", userId);
                HttpContext.Session.SetString("Username", username ?? "");

                return RedirectToAction("Users");
            }
            else
            {
                var message = result.GetProperty("message").GetString();
                ViewBag.Error = message ?? "登录失败";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"连接失败: {ex.Message}";
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{PythonApiBase}/users");
            var responseBody = await response.Content.ReadAsStringAsync();
            return Content(responseBody, "application/json");
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
