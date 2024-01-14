using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

// Mateusz Fijałkowski
// Median Filter v1 - 14.01.2024
// Silesian University of Technology 2023/24

namespace Fijalkowskim_MedianFilter
{
    //Controller class contains only instances of dataManager and mainMenu and allows for communication between them. (MVC model)
    public class Controller
    {
        public DataManager dataManager { get; private set; }
        public MainMenu mainMenu { get; private set; }

        public Controller() 
        { 
            dataManager = new DataManager(this);
            mainMenu = new MainMenu(this);
        }
    }
}
