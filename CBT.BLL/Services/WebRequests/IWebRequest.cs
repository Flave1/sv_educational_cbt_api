using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.WebRequests
{
    public interface IWebRequest
    {
        Task<T> PostAsync<T, Y>(string url, Y data) where Y : class;
        Task<T> GetAsync<T>(string url);
    }
}
