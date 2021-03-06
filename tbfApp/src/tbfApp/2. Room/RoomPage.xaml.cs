﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace tbfApp
{
    public partial class RoomPage : ContentPage
    {
        int clickTotal;
        Label label;
        public RoomPage()
        {
            InitializeComponent();

            var scroll = new ScrollView();
            Content = scroll;

            //Content.BackgroundColor = Color.Aqua;
            
            label = new Label
            {
                Text = "0 button clicks",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // Accomodate iPhone status bar.
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Build the page.        
            var stack = new StackLayout();
            scroll.Content = stack;
            //stack.Children.Add(grid);
            
            stack.Children.Add(new RoomButton("TSG Fitness", Navigation, this, "Der virtuelle Fitnessraum für die Profis der TSG.", "roomID HERE ToDo", "imageURL HERE"));
            stack.Children.Add(new RoomButton("Raum 2", Navigation, this, "Room description here!", "roomID HERE ToDo", "imageURL HERE"));
            stack.Children.Add(new RoomButton("Raum 3", Navigation, this, "Room description here!", "roomID HERE ToDo", "imageURL HERE"));
            stack.Children.Add(new RoomButton("Raum 4", Navigation, this, "Room description here!", "roomID HERE ToDo", "imageURL HERE"));
            stack.Children.Add(new RoomButton("Raum 5", Navigation, this, "Room description here!", "roomID HERE ToDo", "imageURL HERE"));
            stack.Children.Add(label);
        }
    }
}
