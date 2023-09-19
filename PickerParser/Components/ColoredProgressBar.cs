using System.Drawing;
using System.Windows.Forms;

namespace PickerParser.Components
{
    public class ColoredProgressBar : ProgressBar
    {
        private Color barColor = Color.Blue;
        private Color backgroundColor = Color.White;
        private Color textColor = Color.Black;
        private string customText = "";
        private Font textFont = new Font("W3$iP", 9);
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

        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                this.Invalidate();
            }
        }

        public string Text
        {
            get { return customText; }
            set
            {
                customText = value;
                this.Invalidate();
            }
        }

        public Font TextFont
        {
            get { return textFont; }
            set
            {
                textFont = value;
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

            // Отрисовка текста
            string text = string.IsNullOrEmpty(customText) ? Value.ToString() + "%" : customText;
            SizeF textSize = e.Graphics.MeasureString(text, TextFont);
            PointF textLocation = new PointF(2, (this.Height - textSize.Height) / 2);
            e.Graphics.DrawString(text, TextFont, new SolidBrush(TextColor), textLocation);
        }
    }
}
