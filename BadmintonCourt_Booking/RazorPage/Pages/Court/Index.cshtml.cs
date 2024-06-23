using Microsoft.AspNetCore.Mvc;
using BadmintonCourtBusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace RazorPage.Pages.Court
{
    public class IndexModel : PageModel
    {

		private readonly HttpClient httpClient = null;
		public IndexModel()
		{
			httpClient = new HttpClient();
		}

		public IList<BadmintonCourtBusinessObjects.Entities.Court> list { get; set; } = default;

		public async Task OnGetAsync()
		{
			HttpResponseMessage respone = await httpClient.GetAsync("https://localhost:7233/Court/GetAll");
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var data = await respone.Content.ReadAsStringAsync();
			list = JsonSerializer.Deserialize<List<BadmintonCourtBusinessObjects.Entities.Court>>(data, options);


		}
	}
}
