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

    }
}