using System.Web.Mvc;
public class AdminController : Controller
{
    public ActionResult Users() => View();

    public ActionResult Index() => View();
    public ActionResult Agencies() => View();
    public ActionResult Bookings() => View();
    public ActionResult Payment() => View();
    public ActionResult Feedback() => View();
    public ActionResult Settings() => View();

}
