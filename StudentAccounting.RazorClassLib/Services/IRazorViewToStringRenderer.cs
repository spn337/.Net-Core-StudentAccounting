using System.Threading.Tasks;

namespace StudentAccounting.RazorClassLib.Services
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}
