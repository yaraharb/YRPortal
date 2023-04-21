using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace YRPortal.Controllers
{
    public class ProgramController : Controller
    {
        // GET: Program
        public ActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> GetRecipes()
        {
            var options = new RestClientOptions("https://api.usbeefme.com.192-185-6-199.hgws3.hgwin.temp.domains")
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);

            var request = new RestRequest("/api/Recipes/GetRecipes", Method.Post);

            request.AddHeader("Authorization", "Bearer TBoLIS0frPn51cLUleenUnkldKIBpQVRvgWs6BZjbQeTQbpjxvhZqkkJQm4hpfytNg_gnvhCe7nPDoSU7vLItA6EyMzIb1D0deJuGLTfR0QeIfqW-LrB_dKc6tLqhvPLPvti_buJVn68tu1foONznVvlbSk4rR09IlE1l-LDS5w6YK19sofDcHuGvz-GWFtxiIKxb84s8lFishISwJt3gG5SRJ1EGuBsvqsnU9U5EDG-TGKKzhLUQGwR4TX8i5sCheibep4AmaHzvlTKwPADJMnZbTUjvE9643YBgyub4XG0YImsVdTajZ0ihQCtzyuWDi-5cmFp4P1uXTEhqeS6TJkUdBYrhAqCPImSGVd-yuZJE06QylrJL36f-hKgb9WDOhxVaFqgjLUbh7Y3yNdZsDzvbTQDgaf2LZSYdzozfqSxDbGjNDWY007iRSZWEbW4Jjfx9odGt-o62Qhg3LcuLPNqWYaf0uMSYZRcv7NS29bhubAvWZfuxahVHEco5Zo8NTX9IA");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                offset = 1,
                pageSize = 10,
                categoryID = "3",
            };

            request.AddJsonBody(body);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var content = response.Content;
                // Do something with the response content
            }
            else
            {
                // Handle the error response
            }

            return View();
        }

    }
}