using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sales.BLL;
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
        public ActionResult Details(int id)
        {
            var product = productService.GetProductByIds(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product); // 确保返回的是单个产品对象
        }


        // POST: Product/Buy
        //原来的
        //[HttpPost]
        //public ActionResult Buy(string goods_id)
        //{
        //    if (string.IsNullOrEmpty(goods_id))
        //    {
        //        ModelState.AddModelError("", "请选择一项商品，再点击购买");
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        // 执行购买逻辑
        //        return RedirectToAction("Shopping"); // 假设重定向到购物页面
        //    }
        //}


        // GET: Product/Buy/5
        // 新
        // GET: Product/Buy/5
        public ActionResult Buy(int id)
        {
            var product = productService.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult Buy(int PurchaseQuantity, string goods_id, string goods_name, string unit_price, string stock_quantiy, string Username)
        {
            try
            {
                var result = productService.Purchase(PurchaseQuantity, stock_quantiy, unit_price, goods_name, Username, goods_id);
                return RedirectToAction("Shopping", new { id = result });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var product = productService.GetProductById(int.Parse(goods_id));
                return View(product);
            }
        }

        public ActionResult Shopping(string id)
        {
            ViewBag.ProductName = id;
            return View();
        }
        //[HttpGet]
        //public ActionResult Index()
        //{
        //    var products = productService.GetProducts(null, null, true);
        //    return View(products);
        //}

    }
}