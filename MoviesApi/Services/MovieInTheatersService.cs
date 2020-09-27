using System;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesApi.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace MoviesApi.Services
{
    public class MovieInTheatersService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        public MovieInTheatersService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            //dueTime :  موعد مقرر یا سروقت
            _timer = new Timer(DoWork, null, dueTime: TimeSpan.Zero, period: TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var today = DateTime.Today;
            var movies = await context.Movies.Where(x => x.ReleaseDate == today).ToListAsync();
            if (!movies.Any()) return;
            {
                movies.ForEach(x => x.InTheaters = true);
                await context.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Infinit : نامحددود
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public async void Dispose()
        {
            await _timer.DisposeAsync();
        }
    }
}