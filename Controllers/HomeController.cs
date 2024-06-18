using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        // POST: Home/Login
        [HttpPost] // 指定该方法处理 POST 请求
        [ValidateAntiForgeryToken] // 验证防伪标记
        // 定义 Login 方法，处理 POST 请求，接受用户名、密码和用户类型作为参数
        public ActionResult Login(string username, string pwd, int userType)
        {
            if (ModelState.IsValid) // 检查模型状态是否有效
            {
                UserService userService = new UserService(); // 创建 UserService 实例
                bool isAuthenticated = userService.Login(username, pwd, userType); // 调用 UserService 的 Login 方法验证用户身份

                if (!isAuthenticated) // 如果验证通过
                {
                    // 根据用户类型重定向
                    if (userType == 1) // 如果是普通用户
                    {
                        return RedirectToAction("UserDashboard", "User"); // 重定向到 User 控制器的 UserDashboard 动作
                    }
                    else if (userType == 2) // 如果是管理员
                    {
                        return RedirectToAction("AdminDashboard", "Admin"); // 重定向到 Admin 控制器的 AdminDashboard 动作
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
    }
}