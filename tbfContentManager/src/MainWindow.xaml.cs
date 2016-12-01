﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using WhiteCode.Network;
using System.Net;
using System.Net.Sockets;



namespace tbfContentManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        simpleNetwork_Client TCPClient;
        MainContentWindow mainContentWindow = new MainContentWindow();
        public MainWindow()
        {
            InitializeComponent();
            TCPClient = new simpleNetwork_Client(server_response, "", IPAddress.Parse("62.138.6.50"), 
                                                 13001, AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void server_response(string message) {
            TCPClient.closeConnection();
            List<string> lServerData = new List<string>();
            lServerData = message.Split(';').ToList();
            switch (lServerData[0].ToString())
            {
                  case "#103":
                    //MessageBox.Show(message);
                    if (lServerData[1] == "1")
                    {
                       
                        mainContentWindow.Dispatcher.BeginInvoke((Action)(() => mainContentWindow.Show()));
                        this.Dispatcher.BeginInvoke((Action)(() => this.Hide()));
                    }
                    if (lServerData[1] == "2") {
                        MessageBox.Show("Benutzername oder Password ist falsch!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (lServerData[1] == "3")
                    {
                        MessageBox.Show("Der Server hat einen internen Fehler! Bitte kontaktieren Sie einen Administrator!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    txtUser.Dispatcher.BeginInvoke((Action)(() => txtUser.Text = ""));
                    txtPassword.Dispatcher.BeginInvoke((Action)(() => txtPassword.Password = ""));
                    break;
                default :
                    break;
            }
           

        }

        private void btnLogin_Click(object sender, System.Windows.RoutedEventArgs e){
            TCPClient = new simpleNetwork_Client(server_response, "", IPAddress.Parse("62.138.6.50"),
                                                13001, AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (txtPassword.Password.Length > 3 && txtUser.Text.Length > 3)
            {
                if (!TCPClient.connect()) {
                    MessageBox.Show("Verbindung mit dem Server konnte nicht aufgebaut werden!");
                    return;
                }
                //string loginRequest;
                TCPClient.sendMessage("#102;" + txtUser.Text +";" + txtPassword.Password, true);
                //TCPClient.closeConnection();
            }
            else
            {
                MessageBox.Show("Benutzername oder Passwort ist zu kurz! (mindestens 4 Zeichen)", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(null, null);
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
