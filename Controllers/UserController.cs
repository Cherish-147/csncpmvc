using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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



    }
}