using sales.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace csncpmvc.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product0
        //public ActionResult Index()
        //{
        //    return View();
        //}
        private GreenFoodSalesModel1 db = new GreenFoodSalesModel1();

        // GET: Product
        public ActionResult Index(string searchString, int? goodsId, bool? showAll)
        {
            if (showAll.HasValue && showAll.Value)
            {
                // 重置参数
                searchString = null;
                goodsId = null;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentGoodsId = goodsId;

            var products = from p in db.产品表
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.goods_name.Contains(searchString));
            }

            if (goodsId.HasValue)
            {
                products = products.Where(p => p.goods_id == goodsId.Value);
            }

            // 获取所有的goods_id供下拉框使用
            ViewBag.GoodsIdList = new SelectList(db.产品表, "goods_id", "goods_id");

            return View(products.ToList());
        }

        // POST: Product/SelectedProduct
        [HttpPost]
        public ActionResult SelectedProduct(int selectedProductId)
        {
            var selectedProduct = db.产品表.Find(selectedProductId);
            if (selectedProduct == null)
            {
                return HttpNotFound();
            }

            // 处理选定的产品逻辑，例如重定向到详细信息页面或显示详细信息
            // 这里假设重定向到详细信息页面
            return RedirectToAction("Details", new { id = selectedProductId });
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            var product = db.产品表.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        public ActionResult Buy(string goods_id)
        {
            if (string.IsNullOrEmpty(goods_id))
            {
                ModelState.AddModelError("", "请选择一项商品，再点击购买");
                return RedirectToAction("Index");
            }
            else
            {
                // 执行购买逻辑
                return RedirectToAction("Shopping"); // 假设重定向到购物页面
            }
        }
        // GET: Product/Shopping
        public ActionResult Shopping()
        {
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
    }
}