using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdentityServer3.Core.Services;
using sales.BLL;
namespace csncpmvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }
        UserService userService = new UserService(); // 创建 UserService 实例
        // POST: Home/Login
        [HttpPost] // 指定该方法处理 POST 请求
        [ValidateAntiForgeryToken] // 验证防伪标记
        // 定义 Login 方法，处理 POST 请求，接受用户名、密码和用户类型作为参数
        public ActionResult Login(string UserName, string PassWord, int UserType)
        {
            if (ModelState.IsValid) // 检查模型状态是否有效
            {
               
                bool isAuthenticated = userService.Login(UserName,PassWord,UserType); // 调用 UserService 的 Login 方法验证用户身份

                if (isAuthenticated) // 如果验证通过
                {
                    Session["username"] = UserName;
                    // 根据用户类型重定向
                    if (UserType == 1) // 如果是普通用户
                    {
                        return RedirectToAction("Index", "User"); // 重定向到 User 控制器的 UserDashboard 动作
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin"); // 重定向到 Admin 控制器的 AdminDashboard 动作
                    }
                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码错误."); // 添加模型错误信息
                }
            }
            return View(); // 返回登录视图
            
        }

        // GET: Home/Register
        public ActionResult Register() // 定义 Register 方法，处理 GET 请求
        {
            return View(); // 返回注册视图
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string UserName, string Password, string AgainPassword)
        {
            ViewBag.UserName = UserName;
            ViewBag.Password = Password;
            ViewBag.AgainPassword = AgainPassword;
            
            sales.BLL.UserService.UserRegister(UserName,Password,AgainPassword);
            TempData["Message"] = "注册成功";
            //return RedirectToAction("Index");
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string username,string currentPassword,string newPassword,string confirmNewPassword) 
        {
                try
                {
                UserService.ChangePassword(username, currentPassword, newPassword, confirmNewPassword);

                    ViewBag.Message = "密码修改成功";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            

            return View();
        }

    }
}