using System.Windows;
using CourseWork.Models;
using CourseWork.Services;

namespace CourseWork.Views;

public partial class LoginWindow : Window
{
    private readonly ILogger _logger;
    private readonly AuthenticationService _authenticationService;
    private readonly AuthorizationService _authorizationService;

    public LoginWindow(AuthenticationService authenticationService,
                       AuthorizationService authorizationService,
                       ILogger logger)
    {
        _logger = logger;
        _authenticationService = authenticationService;
        _authorizationService = authorizationService;
        InitializeComponent();
    }

    public void Login_Click(object sender, RoutedEventArgs e)
    {
        var authenticatedUsername = _authenticationService.GetUser(Client.Token)?.Username;
        if(authenticatedUsername is not null)
            _logger.Log($"User {authenticatedUsername} tried to login by {Username.Text}");
        else
            _logger.Log($"User {Username.Text} tried to login");
        var result = _authenticationService.Login(Username.Text, Password.Password);
        if (result)
        {
            var token = _authenticationService.GetAccessToken(Username.Text, Password.Password);
            Client.Token = token;
            MessageBox.Show("Успішно авторизовано!");
            var mainWindow = new MainWindow(_authenticationService, _authorizationService, _logger);
            mainWindow.Show();
            Close();
        }
        else
        {
            MessageBox.Show( "Невірний логін або пароль!");
        }
    }

    public void Back_Click(object sender, RoutedEventArgs e)
    {
        var username = _authenticationService.GetUser(Client.Token)?.Username;
        if(username is not null)
            _logger.Log($"User {username} returned to main window");
        else
            _logger.Log("User returned to main window");
        var mainWindow = new MainWindow(_authenticationService, _authorizationService, _logger);
        mainWindow.Show();
        Close();
    }
}