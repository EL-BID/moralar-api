using Microsoft.Extensions.FileProviders;

namespace Moralar.WebApi.Settings
{
    public class HostingEnvironmentAdapter : Microsoft.AspNetCore.Hosting.IHostingEnvironment
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HostingEnvironmentAdapter(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string EnvironmentName
        {
            get => _webHostEnvironment.EnvironmentName;
            set => _webHostEnvironment.EnvironmentName = value;
        }

        public string ApplicationName
        {
            get => _webHostEnvironment.ApplicationName;
            set => _webHostEnvironment.ApplicationName = value;
        }

        public string ContentRootPath
        {
            get => _webHostEnvironment.ContentRootPath;
            set => _webHostEnvironment.ContentRootPath = value;
        }

        public IFileProvider ContentRootFileProvider
        {
            get => _webHostEnvironment.ContentRootFileProvider;
            set => _webHostEnvironment.ContentRootFileProvider = value;
        }

        public string WebRootPath
        {
            get => _webHostEnvironment.WebRootPath;
            set => _webHostEnvironment.WebRootPath = value;
        }

        public IFileProvider WebRootFileProvider
        {
            get => _webHostEnvironment.WebRootFileProvider;
            set => _webHostEnvironment.WebRootFileProvider = value;
        }
    }
}
