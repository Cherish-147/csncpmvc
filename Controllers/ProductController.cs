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

        public ActionResult Buy()
        {
            return View();
        }

    }
}