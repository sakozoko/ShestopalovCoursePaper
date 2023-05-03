using System.ComponentModel;
using System.Windows;
using CourseWork.Models;
using CourseWork.Services;

namespace CourseWork.Views;

public partial class SecuredResourceWindow : Window
{
    private readonly AuthenticationService _authenticationService;
    private readonly ILogger _logger;
    private readonly AuthorizationService _authorizationService;

    public SecuredResourceWindow(AuthenticationService authenticationService,
     AuthorizationService authorizationService,
     ILogger logger)
    {
        _logger = logger;
        _authorizationService = authorizationService;
        _authenticationService = authenticationService;
        InitializeComponent();
        Init();
    }

    private void Init()
    {
        if(!_authorizationService.Authorize(Client.Token))
        {
            SecuredResource.Text = "Ви не маєте доступу до цього ресурсу";
            return;
        }
        var user = _authenticationService.GetUser(Client.Token);
        if (user is null)
            SecuredResource.Text = "Ви не маєте доступу до цього ресурсу";
        else{
            SecuredResource.Text = $"Привіт, {user.Username}! Ваш рівень доступу {user.AccessLevel}";
                TextBlock.Visibility = Visibility.Visible;
        }
             App.Worker.ProgressChanged += WorkerProgress;
        Closing += (o, e) => App.Worker.ProgressChanged -= WorkerProgress;
        
    }

       private void WorkerProgress(object? o, ProgressChangedEventArgs e)
    {
        var workerState = (WorkerState)e.UserState!;
        var window=this;
            Dispatcher.Invoke(() =>
            {
                switch (workerState.WorkerResult)
                {
                    case WorkerResult.CannotRenewed:
                        MessageBox.Show("Час роботи токену вичерпано, оновіть сесію", "Час вийшов",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        SecuredResource.Text = "Анонім";
                        TextBlock.Visibility = Visibility.Collapsed;
                        break;
                    case WorkerResult.TokenRenewed:
                        SecuredResource.Text = $"Привіт, {workerState.User?.Username}! Ваш рівень доступу {workerState.User?.AccessLevel}";
                        TextBlock.Visibility = Visibility.Visible;
                        break;
                    case WorkerResult.TokenExpired:
                        SecuredResource.Text = "Анонім";
                        TextBlock.Visibility = Visibility.Collapsed;
                        break;
                }
            });
    }


    public void Logout_Click(object sender, RoutedEventArgs e)
    {
        _logger.Log($"User {_authenticationService.GetUser(Client.Token)?.Username} logged out");
        _authenticationService.Logout();
        var mainWindow = new MainWindow(_authenticationService, _authorizationService, _logger);
        mainWindow.Show();
        Close();
    }

    public void Back_Click(object sender, RoutedEventArgs e)
    {
        _logger.Log($"User {_authenticationService.GetUser(Client.Token)?.Username} went back to main window");
        var mainWindow = new MainWindow(_authenticationService  , _authorizationService, _logger);
        mainWindow.Show();
        Close();
    }
}