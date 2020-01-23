using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FuncGraph
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Draw(ref Canvas canvas, Equation equation, double minX, double maxX)
        {
            int steps = 200; //liczba linii odwzorowywujących wykres funkcji
            double functionInterval = (maxX - minX) / steps; //odstep pomiedzy kolejnymi punktami funkcji - te punkty sa pozniej laczone za pomoca linii tworzac wykres

            double minY = double.MaxValue;
            double maxY = double.MinValue;
            double x;
            double y;

            for (int i = 0; i < steps; ++i) //obliczanie minuma i maksima funkcji w danym zakresie x
            {
                x = (functionInterval * i) + minX;
                y = equation.Compute(x);
                
                minY = Math.Min(y, minY);
                maxY = Math.Max(y, maxY);
            }

            Line line;
            double screenInterval = canvas.ActualWidth / steps; //odstep na ekranie wyrazony w pikselach pomiedzy kolejnymi badanymi wartosciami na osi x

            double y1;
            double y2;

            for (int i = 0; i < steps; ++i) //wyliczenie wspolrzednych punktu wykresu oraz ich wyskalowanie
            {
                line = new Line();
                line.Stroke = Brushes.DarkMagenta;

                line.X1 = screenInterval * i;
                line.X2 = screenInterval * (i + 1);

                y1 = equation.Compute((functionInterval * i) + minX);
                y1 -= minY;
                y1 /= (maxY - minY);
                y1 = 1 - y1;
                line.Y1 = canvas.ActualHeight * y1;

                y2 = equation.Compute((functionInterval * (i + 1)) + minX);
                y2 -= minY;
                y2 /= (maxY - minY);
                y2 = 1 - y2;
                line.Y2 = canvas.ActualHeight * y2;

                canvas.Children.Add(line);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Equation eq = new Equation(TextBox1.Text); //stworzenie rownania w oparciu o tresc wprowadzana przez uzytkownika
            Canvas1.Children.Clear(); //czyszczenie zawartosci kontrolki canvas odpwiedzialnej za wyswietlanie wykresu
            Draw(ref Canvas1, eq, double.Parse(TextBox2.Text), double.Parse(TextBox3.Text)); //przekazywanie referencji do kontrolki canvas celem narysowania wykresu o okreslonych parametrach
        }
    }
}
