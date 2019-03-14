﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using tbfContentManager.Classes;
using WhiteCode.Network;

namespace tbfContentManager
{
    public partial class MainContentWindow
    {
        string sUserName;
        int iUserID;
        string sTrennzeichen = ";";
        DataTable table;
        SimpleNetwork_Client TCPClient;

        RoomManager roomManager;
        WorkoutManager workoutManager;
        ExerciseManager exerciseManager;
        TraineeManager traineeManager;

        enum tabs { Status = 1, Raum= 2, Workout = 3, Exercise = 4, Trainee = 5 };
        public int aktuellerTab;                              
        

        public MainContentWindow(ref SimpleNetwork_Client TCPClient, string sUserName, int iUserID)
        {
            InitializeComponent();

            roomManager = new RoomManager(ref TCPClient, this, iUserID);
            workoutManager = new WorkoutManager(ref TCPClient, this, roomManager);
            exerciseManager = new ExerciseManager(ref TCPClient, this, roomManager, workoutManager);
            traineeManager = new TraineeManager(ref TCPClient, this, iUserID);

            //StartTab
            aktuellerTab = (int)tabs.Trainee;
            traineeManagerTab.IsSelected = true;
            //While start get room list
            TCPClient.changeProtocolFunction(traineeManager.Server_response_traineeManager);
            roomManager.GetAllRoomSend();


            this.sUserName = sUserName;
            this.iUserID = iUserID;
            this.TCPClient = TCPClient;

            lblWelcomeMessage.Content = "Willkommen " + sUserName;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            MainWindow newLogin = new MainWindow();
            newLogin.Show();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // ----------------------------------------------- Raum Manager -------------------------------------------------------- //

        private void Btn_addRoom_Click(object sender, RoutedEventArgs e)
        {
            roomManager.AddRoomClick();
        }

        private void Btn_saveRoom_Click(object sender, RoutedEventArgs e)
        {
            roomManager.AddRoomSend(iUserID, sTrennzeichen, txt_beschreibung_room.Text, txt_url_pic_room.Text,
                (bool) b_isPrivate_room.IsChecked, txt_name_room.Text, "0");
            Thread.Sleep(2000);
            roomManager.GetAllRoomSend();
        }

        private void B_url_pic_room_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".txt";
            //dlg.Filter = "Bildformat(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            dlg.Filter = "Bildformat(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            
            //spater fuer video
            /*
             dlg.Filter = "Videoformat (...) |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso;" + 
             "*.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4;" + 
             "*.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm";
             */

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                //Upload File via FTP
                //string source = @"FilePath and FileName of Local File to Upload";
                //string destination = @"SFTP Server File Destination Folder";
                //string host = "SFTP Host";
                //string username = "User Name";
                //string password = "password";
                //int port = 22;  //Port 22 is defaulted for SFTP upload
                try
                {
                    Upload.UploadSFTPFile(this, "tbf.spdns.de", "contentmanager", "TBF123", filename, "./", 13002);

                    txt_url_pic_room.Text = filename;
                }
                catch (Exception)
                {
                    MessageBox.Show("Fehler beim Hochladen der Datei!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
            //System.Drawing.Image img = System.Drawing.Image.FromFile(@ + filename);
            //MessageBox.Show("Width: " + .Width + ", Height: " + img.Height);

        }

        private void Btn_cancel_room_Click(object sender, RoutedEventArgs e)
        {
            roomManager.ClearAllTxtFields();
        }

        private void Btn_Delete_room_Click(object sender, RoutedEventArgs e)
        {
            roomManager.DeleteRoom();
            Thread.Sleep(2000);
            roomManager.GetAllRoomSend();
        }

        private void TiRoomManager_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (aktuellerTab != (int)tabs.Raum)
            {
                aktuellerTab = (int)tabs.Raum;

                TCPClient.changeProtocolFunction(roomManager.Server_response_roomManager);

                //gb_workoutInfos.Visibility = Visibility.Hidden;
                roomManager.GetAllRoomSend();
                Thread.Sleep(100);
            }
        }

        private void btn_saveChangeRoom_Click(object sender, RoutedEventArgs e)
        {
            roomManager.ChangeRoomSend(iUserID, sTrennzeichen, txt_beschreibung_room.Text, txt_url_pic_room.Text, (bool)b_isPrivate_room.IsChecked, txt_name_room.Text);
        }

        // --------------------------------------------------------------- Workout Manager ---------------------------------------------------- //
        
        private void btn_addWorkout_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.AddWorkoutClick();

        }

        private void btn_saveWorkout_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.AddWorkoutSend(iUserID, sTrennzeichen, txt_beschreibung_workout.Text, txt_url_pic_workout.Text, txt_name_workout.Text, "0");
        }

        private void B_url_pic_workout_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Bildformat (*.JPG; *.PNG; )|*.JPG, *.PNG";

            //spater fuer video
            
             //dlg.Filter = "Videoformat (...) |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso;" + 
             //"*.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4;" + 
             //"*.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm";
             

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                txt_url_pic_workout.Text = filename;
            }
            //System.Drawing.Image img = System.Drawing.Image.FromFile(@ + filename);
            //MessageBox.Show("Width: " + .Width + ", Height: " + img.Height);

        }

        private void btn_saveChangeWorkout_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.ChangeWorkoutSend(iUserID, sTrennzeichen, txt_beschreibung_workout.Text, txt_url_pic_workout.Text, txt_name_workout.Text);

        }

        private void Btn_cancel_workout_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.ClearAllTxtFields();
        }

        private void Btn_Delete_workout_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.DeleteWorkout();
        }

        private void TiWorkoutManager_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(aktuellerTab != (int)tabs.Workout)
            {
                aktuellerTab = (int)tabs.Workout;

                TCPClient.changeProtocolFunction(this.workout_room_Delegate);
                roomManager.GetAllRoomSend();
            }

        }

        private void workout_room_Delegate(string message)
        {
            roomManager.Server_response_roomManager(message);

            TCPClient.changeProtocolFunction(workoutManager.Server_response_workoutManager);
            workoutManager.ShowAllRooms();
        }

        //------------------------------- Exercise Manager -------------------------------------------------//

        private void btn_addExercise_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.AddWorkoutClick();
        }

        private void btn_saveExercise_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.AddWorkoutSend(iUserID, sTrennzeichen, txt_beschreibung_workout.Text, txt_url_pic_workout.Text, txt_name_workout.Text, "0");
        }

        private void B_url_pic_Exercise_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Bildformat (*.JPG; *.PNG; )|*.JPG, *.PNG";

            //spater fuer video

            //dlg.Filter = "Videoformat (...) |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso;" + 
            //"*.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4;" + 
            //"*.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm";


            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                txt_url_pic_Exercise.Text = filename;
            }
            //System.Drawing.Image img = System.Drawing.Image.FromFile(@ + filename);
            //MessageBox.Show("Width: " + .Width + ", Height: " + img.Height);

        }

        private void btn_saveChangeExercise_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.ChangeWorkoutSend(iUserID, sTrennzeichen, txt_beschreibung_workout.Text, txt_url_pic_workout.Text, txt_name_workout.Text);

        }

        private void Btn_cancel_Exercise_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.ClearAllTxtFields();
        }

        private void Btn_Delete_Exercise_Click(object sender, RoutedEventArgs e)
        {
            workoutManager.DeleteWorkout();
        }

        private void TiExerciseManager_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (aktuellerTab != (int)tabs.Exercise)
            {
                aktuellerTab = (int)tabs.Exercise;

                TCPClient.changeProtocolFunction(roomManager.Server_response_roomManager);
                roomManager.GetAllRoomSend();
                Thread.Sleep(400);

                TCPClient.changeProtocolFunction(exerciseManager.Server_response_exerciseManager);
                gb_ExerciseInfos.Visibility = Visibility.Hidden;
                exerciseManager.ShowAllRooms();
            }
        }

        private void TiTraineeManager_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (aktuellerTab != (int)tabs.Trainee)
            {
                aktuellerTab = (int)tabs.Trainee;

                TCPClient.changeProtocolFunction(traineeManager.Server_response_traineeManager);

                roomManager.GetAllRoomSend();
                Thread.Sleep(100);
            }
        }
    }
 }
