using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PickerParser.Components
{
    public class ColoredProgressBar : ProgressBar
    {
        private Color barColor = Color.Blue;
        private Color backgroundColor = Color.White;

        public ColoredProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public Color BarColor
        {
            get { return barColor; }
            set
            {
                barColor = value;
                this.Invalidate();
            }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            // Отрисовка фона
            e.Graphics.FillRectangle(new SolidBrush(BackgroundColor), rect);

            // Отрисовка полоски ProgressBar в выбранном цвете
            rect.Width = (int)(rect.Width * ((double)Value / Maximum)) - 4;
            rect.Height = rect.Height - 4;
            e.Graphics.FillRectangle(new SolidBrush(BarColor), 2, 2, rect.Width, rect.Height);
        }
    }
}
