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
            dataManager = new DataManager();
            mainMenu = new MainMenu(this);
        }
        public Bitmap GetFunctionResult(Bitmap bitmap, DllType dllType, ref string executionTime, ref string previousExecutionTime)
        {
            Bitmap result = dataManager.UseMedianFilter(dllType);
            executionTime = dataManager.currentExecutionTime != TimeSpan.Zero ? dataManager.currentExecutionTime.ToString() : "";
            previousExecutionTime = dataManager.previousExecutionTime != TimeSpan.Zero ? dataManager.previousExecutionTime.ToString() : "";
            return result;
        }
    }
}
