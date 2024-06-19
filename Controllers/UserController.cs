using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentityServer3.Core.ViewModels;
using sales.BLL;
using sales.DAL;

namespace csncpmvc.Controllers
{
    public class UserController : Controller
    {
        //GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Product()
        {
            return View();
        }
        
        private sales.BLL.ProductMethod productService = new ProductMethod();
        [HttpGet]
        // GET: Product
        public ActionResult Index(string searchString, int? goodsId, bool? showAll)
        {
            var products = productService.GetProducts(searchString, goodsId, showAll);

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentGoodsId = goodsId;

            // 获取所有的goods_id供下拉框使用
            ViewBag.GoodsIdList = new SelectList(productService.GetAllGoodsIds());

            return View(products);
        }
        // POST: Product/SelectedProduct
        [HttpPost]
        public ActionResult SelectedProduct(int selectedProductId)
        {
            var selectedProduct = productService.GetProductByIds(selectedProductId);
            if (selectedProduct == null)
            {
                return HttpNotFound();
            }

            // 处理选定的产品逻辑，例如重定向到详细信息页面或显示详细信息
            // 这里假设重定向到详细信息页面
            return RedirectToAction("Details", new { id = selectedProductId });
            //return View();
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "The id parameter is required.");
            }

            var product = productService.GetProductByIds(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product); // 确保返回的是单个产品对象
        }



        
        public ActionResult ToBuySelectedProduct(int selectedProductId)
        {
            var selectedProduct = productService.GetProductByIds(selectedProductId);
            if (selectedProduct == null)
            {
                return HttpNotFound();
            }

            // 处理选定的产品逻辑，例如重定向到详细信息页面或显示详细信息
            // 这里假设重定向到详细信息页面
            return RedirectToAction("Shopping", new { id = selectedProductId });
            
        }

       
        //// GET: Product/Shopping
        public ActionResult Shopping(int?id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "The id parameter is required.");
            }

            var product = productService.GetProductByIds(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product); // 确保返回的是单个产品对象
        }


        /// <summary>
        /// 处理两个按钮是去下单还是查看详细
        /// </summary>
        /// <param name="selectedProductId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProcessSelection(string selectedProductId, string action)
        {
            if (action == "Details")
            {
                return RedirectToAction("Details", new { id = selectedProductId });
            }
            else if (action == "Buy")
            {
                // 处理购买逻辑
                return RedirectToAction("Shopping", new { id = selectedProductId });
            }

            return RedirectToAction("Index");
        }

        public ActionResult SummitPurchase()
        {
            return View();
        }

        // 处理提交购买数量的请求
        [HttpPost]
        public ActionResult SummitPurchase(int purchaseQuantity, int goods_id, string goods_name, string unit_price, string stock_quantiy, string username)
        {
            try
            {
                username = Session["username"].ToString(); // 测试用
                Purchase(purchaseQuantity, stock_quantiy, unit_price, goods_name, username, goods_id.ToString());
                // 设置 ViewBag 变量(新增三行)
                ViewBag.GoodsName = goods_name;
                ViewBag.PurchaseQuantity = purchaseQuantity;
                ViewBag.UnitPrice = decimal.Parse(unit_price);
                ViewBag.Message = "购买成功";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }
        public string Purchase(int number, string inventory, string price, string goodsname, string Username, string goodsid)
        {
            int Goods = int.Parse(goodsid);
            if (number == 0)
            {
                throw new Exception("请输入购买数量");
            }
            else
            {
                if (number > int.Parse(inventory))
                {
                    throw new Exception("购买数量不能大于库存");
                }
                else
                {
                    using (var db = new GreenFoodSalesModel1())
                    {
                        // 修改产品表中的库存
                        var product = db.产品表.SingleOrDefault(p => p.goods_id == Goods);

                        if (product != null)
                        {
                            product.stock_quantiy -= number; // 更新库存数量
                            db.SaveChanges(); // 保存更改到数据库
                        }
                        else
                        {
                            throw new Exception("未找到指定商品");
                        }

                        // 判断该商品是否存在购物车中。如果存在就修改购买数量，否则增加一条新的购买记录
                        var cartItem = db.购物车表.SingleOrDefault(r => r.username == Username && r.goods_id == goodsid);

                        if (cartItem != null)
                        {
                            cartItem.Qty += number;
                        }
                        else
                        {
                            var newCartItem = new sales.DAL.购物车表
                            {
                                username = Username,
                                goods_id = goodsid,
                                Proname = goodsname,
                                ListPrice = decimal.Parse(price),
                                Qty = number
                            };

                            db.购物车表.Add(newCartItem); // 插入新项
                        }

                        db.SaveChanges(); // 保存更改到数据库
                    }
                }
            }

            return goodsid.ToString();
        }

        private GreenFoodSalesModel1 db = new GreenFoodSalesModel1();
        //查看购物车
        public ActionResult CartView() { 
            string username = Session["username"].ToString();
            var look=from l in db.购物车表 where(l.username == username) select l;
            return View(look);
        }


        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 验证用户登录信息
                var user = db.用户表.SingleOrDefault(u => u.usename == model.Username );
                if (user != null)
                {
                    Session["username"] = user.usename;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码错误");
                }
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult ProcessSelections(string selectedProductId, string action)
        {
            Session["Cid"] = selectedProductId;
            if (action == "Checkout")
            {
                return RedirectToAction("Checkout", new { id = selectedProductId });
            }
            else if (action == "Detele")
            {
                // 处理购买逻辑
                return RedirectToAction("Detele", new { id = selectedProductId });
            }

            return RedirectToAction("Index");
        }

        public ActionResult Detele(string id)
        {
            
            ProductMethod.CartDelete(id);

            return View();
        }
        //结算
        public ActionResult Checkout()
        {
            
            return View();  
        }
        private ProductMethod OrderService = new ProductMethod();
        [HttpPost]
        public ActionResult Finnally(string username,string province,string city,string address,string zipcode, string phone)
        {
            username=Session["username"].ToString();
            string Cid = Session["Cid"].ToString();
            OrderService.AddOrder(username, province, city, address, zipcode, phone);
            ProductMethod.DeleteCart(username);
            return View();
        }
        public ActionResult Finnally()
        { 
            return View(); 
        }
        //public ActionResult CartView(string username)
        //{

        //}
        //购物车

    }
}