using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MoviesApi.Services
{
    public class WriteToFileHostedService : IHostedService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string fileName = "File1.txt";
        private Timer timer;


        public WriteToFileHostedService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        //زمان اجرا شدن
        public Task StartAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process Started");

            //میگم طبق این زمان بیا این متد را صدا بزن
            timer = new Timer(callback: DoWork, state: null, dueTime: TimeSpan.Zero, period: TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        //زمان ثبت توقف 
        //منظور همان توقف IIS  هست
        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process Stoped");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //برو زمان را توی این بخش ثبت کن و لاگ بزن
            WriteToFile("Process Ongoing : " + DateTime.Now.ToString("yy-MM-dd hh:mm:ss"));
        }

        private void WriteToFile(string message)
        {
            //ContentRootPath : مسیر پیش فرض پروژه

            var path = $@"{_environment.ContentRootPath}\wwwroot\{fileName}";
            using (var writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine(message);
            }

        }
    }
}