using AuthenticationProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;

namespace AuthenticationProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string connStr = _configuration["DbContext:ConnectionString"];
                var userInfo = new LoginInfo(); 

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT * FROM [dbo].[User] WHERE UserId=@Username AND Password=@Password";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", model.Username);
                        cmd.Parameters.AddWithValue("@Password", model.Password);

                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 資料庫有對應的使用者資訊，驗證登入通過
                                userInfo.Username = reader.GetString(reader.GetOrdinal("UserId"));
                                userInfo.Password = reader.GetString(reader.GetOrdinal("Password"));
                                // 其他需要的使用者資訊，可以在這裡設定

                                // 將使用者資訊存入 Session 或其他認證機制中
                                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, userInfo.Username)
                            };
                                var identity = new ClaimsIdentity(claims, "MyCookieAuthenticationScheme");

                                await HttpContext.SignInAsync("MyCookieAuthenticationScheme", new ClaimsPrincipal(identity));

                                // 跳轉到登入成功後的頁面
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                // 資料庫無對應的使用者資訊，驗證登入失敗
                                ModelState.AddModelError("", "登入失敗，請檢查使用者名稱和密碼");
                                TempData["ErrorMessage"] = "登入失敗，請檢查使用者名稱和密碼";
                            }
                        }
                        //Debug.WriteLine("Found matching user record in the database.");
                    }
                }
            }

            // 驗證失敗，將使用者導回登入頁面
            return View("Login", model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // 呼叫 ASP.NET Core 的登出方法
            await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");

            // 重定向到登出後的頁面
            return RedirectToAction("Index", "Home");
        }
    }
}
