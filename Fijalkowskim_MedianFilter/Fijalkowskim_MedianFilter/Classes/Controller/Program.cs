using System;
using System.Windows.Forms;

// Mateusz Fija³kowski
// Median Filter v1 - 14.01.2024
// Silesian University of Technology 2023/24

namespace Fijalkowskim_MedianFilter
{
    //Main class for running application
    static class Program
    {   
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Controller controller = new Controller();
            Application.Run(controller.mainMenu);
        }
    }
}
