using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DeviceDemo.Views.Device
{
    class Led : Border
    {
        static Brush On = Brushes.Green;
        static Brush Off = Brushes.LightGray;

        public Led()
        {
            this.Width = this.Height = 30;
            this.CornerRadius = new CornerRadius(this.Width / 2);

            this.BorderThickness = new Thickness(1);
            this.Background = Off;
        }
        public int State
        {
            get => Background == On ? 1 : 0;
            set
            {
                this.Background = value == 0 ? Off : On;
            }
        }
    }
    class Demo : BaseView<Models.DeviceViewModel, MyTableLayout>
    {
        Dictionary<string, Led> _leds = new Dictionary<string, Led>();
        void SetValue(int value)
        {
            int i = 0;
            foreach (var p in _leds)
            {
                p.Value.State = value & (1 << i++);
            }
            Model.UpdateStatus(value);
        }
        void SetValue(string name, int value)
        {
            _leds[name].State = value;
        }
        protected override void RenderBody()
        {
            var gridLed = new MyTableLayout();
            var gridBtn = new MyTableLayout { 
                Margin = new Thickness(100),
            };

            MainContent.AddRow(gridLed);
            MainContent.AddRow(gridBtn);

            int i = 0;
            foreach (var p in Model.Status)
            {
                var led = new Led();
                gridLed.AddColumn();
                gridLed.Add(0, i++, led);
                _leds.Add(p.Key, led);
            }

            int rows = 4, cols = 3;
            for (i = 0; i < rows; i++)
            {
                gridBtn.AddRow();
            }
            for (i = 0; i < cols; i++)
            {
                gridBtn.AddColumn();
            }

            string s = "123456789*0#";
            for (int r = 0, k = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++, k++)
                {
                    var btn = new Button {
                        Content = s.Substring(k, 1),
                        FontSize = 14,
                        Margin = new Thickness(2),
                        Width = 40,
                        Height = 40,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                    btn.VerticalAlignment = VerticalAlignment.Stretch;

                    //btn.Click += (b, e) => {
                        char v = ((string)btn.Content)[0];
                        if (char.IsDigit(v))
                        {
                            SetValue(v & 15);
                            return;
                        }
                    };

                    gridBtn.Add(r, c, btn);
                }
            }

            gridLed.SetWidths(GridUnitType.Star, 1, 1, 1, 1);
            gridBtn.SetWidths(GridUnitType.Star, 1, 1, 1);
            gridBtn.SetHeights(GridUnitType.Star, 1, 1, 1, 1);

            MainContent.SetHeights(GridUnitType.Star, 25, 75);
        }
    }
}
