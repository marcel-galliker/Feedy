using Feedy.Services;
using Feedy.Views.JobView;
using Feedy.Views.Main;

namespace Feedy.Models
{
    public class FdGolbals
    {
        public static FdArgs            FdArgs      = new FdArgs();
        public static Dir               Dir         = new Dir();
        public static Language          Language    = new Language();
        public static Events            Events      = new Events();
        public static User              User        = new User();
        public static CVersion          Version     = new CVersion();

        //--- views ----------------------------------------------
        public static MainWindow        MainWindow;
        public static Menu              Menu;
        public static JobView           JobView;

        public static FdInterface       FdInterface = new FdInterface();

        //--- Data --------------------------------------
        public static Settings          Settings    = new Settings();
        public static MotorSettingsList MotorSettings = new MotorSettingsList(4);
        public static FeedyStatus       FeedyStatus = new FeedyStatus();
        public static FdConfig          Cfg         = new FdConfig();
        public static Job               Job         = new Job();
        public static JobSettings       JobSettings = new JobSettings();

        public static JogInfo           JogInfo = new JogInfo();
    }
}
