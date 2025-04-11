using Microsoft.Extensions.DependencyInjection;

namespace Hash.Extensions;

public static class ViewExtensions
{
    public static void AddViews(this IServiceCollection serviceCollection)
    {
        /// Views.MainView
        serviceCollection.AddSingleton(sp => new Views.MainView()
        {
            DataContext = sp.GetRequiredService<ViewModels.MainViewModel>()
        });
        serviceCollection.AddSingleton<ViewModels.MainViewModel>();
    }
}
