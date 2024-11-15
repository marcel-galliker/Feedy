using Feedy.Services;
using Feedy.Views.JobView;
using Feedy.Views.Main;
using System;

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
        public static FeedyStatus       FeedyStatus = new FeedyStatus();
        public static Config            Cfg         = new Config();
        public static Job               Job         = new Job();
        public static JobSettings       JobSettings = new JobSettings();
    }
}
