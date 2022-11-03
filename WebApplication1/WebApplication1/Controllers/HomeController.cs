using System.Diagnostics;
using LdapForNet;
using LdapForNet.Native;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
// using Novell.Directory.Ldap;
using NovellLdap = Novell.Directory.Ldap;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private const string baseHost = "dc=example,dc=org";
    private const string password = "admin";
    private const string searchFilter = $"(uid=billy)";
    
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public Task<IActionResult> Index()
    {

        #region ldapfornmet

        using (var cn = new LdapConnection())
        {
            // connect ldap server use hostname and port
            cn.Connect("localhost", 8389);
            // 登入驗證
            cn.Bind(Native.LdapAuthMechanism.SIMPLE, $"cn=admin,{baseHost}", password);
            // 搜尋節點, filter 條件自訂 , scope 自訂
            var nodes = cn.Search(baseHost, searchFilter, null, Native.LdapSearchScope.LDAP_SCOPE_SUB);
            // 取得節點內 屬性值
            var mail = nodes.FirstOrDefault((e => e.Dn.Contains("uid=billy"))).DirectoryAttributes.FirstOrDefault(x => x.Name == "mail").GetValue<string>();
            
            // 詳細 api
            // https://github.com/flamencist/ldap4net
        }

        #endregion

        return Task.FromResult<IActionResult>(View());
    }

    public IActionResult Privacy(string useName,string userPwd)
    {
        // const string useName = "billy";
        // const string userPwd = "0000";
        
        #region Novell.Directory.Ldap

        using (var connect = new NovellLdap.LdapConnection())
        {
            // connect ldap server use hostname and port
            connect.Connect("localhost",8389);
            // 登入驗證
            connect.Bind($"cn=admin,{baseHost}", password);
            // 可察看連線與驗證是否成功
            if(connect.Bound) Console.WriteLine("connected true");

            var filter = $"uid={useName}";
            // 搜尋節點, filter 條件自訂, scope 自訂
            var nodes  = connect.Search(baseHost, NovellLdap.LdapConnection.ScopeSub, filter, null, false);
            // 取得下一節點
            var newEntry = nodes.Next();
            // 撈選 uid 屬性
            var account = newEntry.GetAttribute("uid");
            // 撈選 mail 屬性值
            var mail = newEntry.GetAttribute("mail").StringValue;
        }

        #endregion
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}