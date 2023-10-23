using System;
using System.Drawing;
using System.Windows.Forms;
namespace Fijalkowskim_MedianFilter
{
    public class Controller
    {
        public DataManager dataManager { get; private set; }
        public MainMenu mainMenu { get; private set; }

        public Controller() 
        { 
            dataManager = new DataManager(this);
            mainMenu = new MainMenu(this);
        }
        public Bitmap GetFunctionResult(Bitmap bitmap, DllType dllType)
        {
            Bitmap result = dataManager.UseMedianFilter(dllType);
            mainMenu.SetExecutionTime(dataManager.currentExecutionTime != TimeSpan.Zero ? dataManager.currentExecutionTime.ToString() : "", 
                dataManager.previousExecutionTime != TimeSpan.Zero ? dataManager.previousExecutionTime.ToString() : "");
            return result;
        }
    }
}
