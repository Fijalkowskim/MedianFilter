using System;
using System.Drawing;
using System.Threading.Tasks;
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

        /*public async Task<Bitmap> GetFunctionResult(DllType dllType, IProgress<ImageLoadingProgress> progress)
        {
            Bitmap result = await dataManager.UseMedianFilter(dllType, progress);
            mainMenu.SetExecutionTime(dataManager.currentExecutionTime != TimeSpan.Zero ? dataManager.currentExecutionTime.ToString() : "", 
                dataManager.previousExecutionTime != TimeSpan.Zero ? dataManager.previousExecutionTime.ToString() : "");
            return result;
        }*/
    }
}
