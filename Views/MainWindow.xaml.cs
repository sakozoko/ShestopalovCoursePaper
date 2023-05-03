using System.ComponentModel;
using System.Threading;
using System.Windows;
using CourseWork.Models;
using CourseWork.Services;

namespace CourseWork.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly AuthorizationService _authorizationService;
    private readonly ILogger _logger;
    private readonly AuthenticationService _authenticationService;

    public MainWindow(AuthenticationService authenticationService,
                      AuthorizationService authorizationService,
                      ILogger logger)
    {
        _logger = logger;
        _authenticationService = authenticationService;
        _authorizationService = authorizationService;        
        InitializeComponent();
        Closing += (o, e) => App.Worker.ProgressChanged-=WorkerProgress;
        if(Client.Token is not null){
            var user = _authenticationService.GetUser(Client.Token);
                Username.Text = "Авторизовано як " + user?.Username;
                if (user?.Id == 1)
                    Register.IsEnabled = true;
        }
        else
            Username.Text = "Анонім";
        App.Worker.ProgressChanged+=WorkerProgress;
    }
    private void WorkerProgress(object? o, ProgressChangedEventArgs e)
    {
        var workerState = (WorkerState)e.UserState!;
            Dispatcher.Invoke(() =>
            {
                switch (workerState.WorkerResult)
                {
                    case WorkerResult.CannotRenewed:
                        MessageBox.Show("Час дії авторизації вичерпано, обновіть сесію", "Час вийшов",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        Username.Text = "Анонім";
                        Register.IsEnabled = false;
                        break;
                    case WorkerResult.TokenRenewed:
                        Username.Text = "Авторизовано як " + workerState.User?.Username;
                        if (workerState.User?.Id == 1)
                            Register.IsEnabled = true;
                        break;
                    case WorkerResult.TokenExpired:
                        Username.Text = "Анонім";
                        Register.IsEnabled = false;
                        break;
                }
            });
    }

    

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        var username = _authenticationService.GetUser(Client.Token)?.Username;
        if(username is not null)
            _logger.Log($"{username} clicked to register button");
        else
            _logger.Log("User clicked to register button");
        var registerWindow = new RegisterWindow(_authenticationService, _authorizationService, _logger);
        registerWindow.Show();
        Close();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        var username = _authenticationService.GetUser(Client.Token)?.Username;
        if(username is not null)
            _logger.Log($"{username} clicked to login button");
        else
            _logger.Log("User clicked to login button");
        var loginWindow = new LoginWindow(_authenticationService, _authorizationService, _logger);
        loginWindow.Show();
        Close();
    }

    private void Secured_Click(object sender, RoutedEventArgs e)
    {
        var username = _authenticationService.GetUser(Client.Token)?.Username;
        if(username is not null)
            _logger.Log($"{username} clicked to secured resource button");
        else
            _logger.Log("User clicked to secured resource button");
        var securedResourceWindow = new SecuredResourceWindow(_authenticationService, _authorizationService, _logger);
        securedResourceWindow.Show();
        Close();
    }
}