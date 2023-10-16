using System;
using System.Windows.Forms;
namespace Fijalkowskim_MedianFilter
{
    public class Controller
    {
        public DataManager dataManager { get; private set; }
        public MainMenu mainMenu { get; private set; }
        public DllTestForm dllTestForm { get; private set; }

        public Controller() 
        { 
            dataManager = new DataManager();
            mainMenu = new MainMenu(this);
        }
        public int GetFunctionResult(int a, int b, DllType dllType, ref String executionTime, ref String previousExecutionTime)
        {
            int result = dataManager.GetResult(a, b, dllType);
            executionTime = dataManager.currentExecutionTime != TimeSpan.Zero ? dataManager.currentExecutionTime.ToString() : "";
            previousExecutionTime = dataManager.previousExecutionTime != TimeSpan.Zero ? dataManager.previousExecutionTime.ToString() : "";
            return result;
        }
        public void OpenTestWindow()
        {
            dllTestForm = new DllTestForm(this);
            dllTestForm.Show();
        }
    }
}
