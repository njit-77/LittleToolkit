using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Hash.Extensions;
using Hash.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hash;

public partial class App : Application
{
    public static new App Current => (App)Application.Current;

    private IServiceProvider _serviceProvider;

    private Log.ILogger _logger => GetService<Log.ILogger>();

    private IMessageBoxService _messageBoxService => GetService<IMessageBoxService>();

    App()
    {
        _serviceProvider = ConfigureServices();
    }

    #region override

    protected override void OnStartup(StartupEventArgs e)
    {
        AppStartUp();

        MainWindow = GetService<Views.MainView>();
        MainWindow.Visibility = Visibility.Visible;

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        mutex.Dispose();
        mutex = null;

        base.OnExit(e);
    }

    #endregion


    #region Method

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        /// View
        services.AddViews();

        /// ILogger
        services.AddLogger();

        /// IServiceCollection
        services.AddSingleton<IServiceCollection>(services);

        /// WeakReferenceMessenger
        services.AddSingleton<WeakReferenceMessenger>();
        services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
            provider.GetRequiredService<WeakReferenceMessenger>()
        );

        /// Dispatcher
        services.AddSingleton(_ => Current.Dispatcher);

        /// Service
        services.AddSingleton<IMessageBoxService, MessageBoxService>();

        return services.BuildServiceProvider();
    }

    private void AppStartUp()
    {
        if (!EnsureAssemblySingletion())
        {
            _messageBoxService?.ShowMessage(
                $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name} 服务实例已在运行中。",
                MessageLevel.Information
            );

            System.Windows.Application.Current.Shutdown();

            return;
        }

        /// 设置UI线程发生异常时处理函数
        System.Windows.Application.Current.DispatcherUnhandledException +=
            App_DispatcherUnhandledException;

        /// 设置非UI线程发生异常时处理函数
        AppDomain.CurrentDomain.UnhandledException += App_UnhandledException;

        /// 设置托管代码异步线程发生异常时处理函数
        TaskScheduler.UnobservedTaskException += App_UnobservedTaskException;

        /// 设置非托管代码发生异常时处理函数
        callBack = new Unhandled_CallBack(Unhandled_ExceptionFilter);
        SetUnhandledExceptionFilter(callBack);
    }

    public T? GetService<T>()
        where T : class
    {
        return _serviceProvider.GetService(typeof(T)) as T;
    }

    #endregion


    #region Singletion App

    /// <summary>
    /// 必须定义此变量
    /// </summary>
    /// <remarks>
    /// <para>
    /// 当EnsureAssemblySingletion()函数内部定义局部Mutex时，如果先启动软件再调试运行，此时判断单例模式失效。
    /// </para>
    /// </remarks>
    private System.Threading.Mutex mutex;

    private const string AssemblyGUID = "5763f94e-1c86-45d6-9f84-877c700f2367";

    private bool EnsureAssemblySingletion()
    {
        _logger.Info("This is a message from EnsureAssemblySingletion.");

        mutex = new System.Threading.Mutex(
            true,
            $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name} - {AssemblyGUID}",
            out bool ret
        );

        if (ret)
        {
            mutex.ReleaseMutex();
        }

        return ret;
    }

    #endregion


    #region Try catch Exception

    private void App_DispatcherUnhandledException(
        object sender,
        System.Windows.Threading.DispatcherUnhandledExceptionEventArgs exception
    )
    {
        _logger?.Error("[UI线程]异常：{ErrorException}.", exception.Exception);

        _messageBoxService?.ShowMessage(exception.Exception.ToString(), MessageLevel.Error);

        exception.Handled = true;
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs exception)
    {
        _logger?.Fatal("[非UI线程]异常：{ErrorException}.", exception);

        _messageBoxService?.ShowMessage("软件出现不可恢复错误，即将关闭。", MessageLevel.Error);

        System.Windows.Application.Current.Shutdown();
    }

    private void App_UnobservedTaskException(
        object sender,
        UnobservedTaskExceptionEventArgs exception
    )
    {
        _logger?.Fatal("Fatal - [Task]异常 Exception = {ErrorException}.", exception.Exception);

        exception.SetObserved();
    }

    [System.Runtime.InteropServices.DllImport("kernel32")]
    private static extern Int32 SetUnhandledExceptionFilter(Unhandled_CallBack cb);

    private delegate int Unhandled_CallBack(ref long a);

    private Unhandled_CallBack callBack;

    private int Unhandled_ExceptionFilter(ref long a)
    {
        _logger?.Fatal("[非托管代码]异常：{FatalStackTrace}.", Environment.StackTrace);

        return 1;
    }

    #endregion
}
