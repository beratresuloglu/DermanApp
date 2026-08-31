using Derman.Api.DTOs;
using Derman.Api.Identity;
using Derman.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Derman.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtService _jwtService;
    private readonly OtpService _otpService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtService jwtService,
        OtpService otpService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _otpService = otpService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        _otpService.GenerateCode(request.Email);

        return Ok(new { message = "Kayıt başarılı, e-postanıza gönderilen kodu doğrulayın." });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
    {
        if (!_otpService.ValidateCode(request.Email, request.Code))
            return BadRequest(new { message = "Kod geçersiz veya süresi dolmuş." });

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return NotFound();

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        return Ok(new { message = "Hesap doğrulandı, giriş yapabilirsiniz." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { message = "E-posta veya şifre hatalı." });

        if (!user.EmailConfirmed)
            return Unauthorized(new { message = "Hesap henüz doğrulanmamış." });

        if (user.IsBlocked)
            return Unauthorized(new { message = "Hesabınız kısıtlanmış." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(new { message = "E-posta veya şifre hatalı." });

        var token = _jwtService.GenerateToken(user);
        return Ok(new AuthResponse(token, user.FullName, user.Role));
    }
}