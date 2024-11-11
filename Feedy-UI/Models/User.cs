using GE_Utilities;
using MahApps.Metro.IconPacks;
using System.Windows;

namespace Feedy.Models
{
	public enum EN_UserType
    {
		USER_undef,			// 0
        USER_operator,      // 1
        USER_supervisor,    // 2
        USER_service,       // 3
  //      USER_gt,            // 4
    };

    public class User : GeBindable
    {
		public delegate void UserChagedDelegate();
        public UserChagedDelegate UserChanged;

		public User()
		{
			#if DEBUG
				Type = EN_UserType.USER_service;
			#else
				Type = EN_UserType.USER_operator;
			#endif
			FdGolbals.Language.LanguageChanged   += _Language;
		}

		//--- _Language ----------------------------------------------
        private void _Language()
        {
            Name = GetName(_Type);
        }

		//--- Property UserType ---------------------------------------
		private EN_UserType _Type;
		public EN_UserType Type
		{
			get { return _Type; }
			set { 
					Name = GetName(value);
					Icon = GetIcon(value);
					Enabled = (value>EN_UserType.USER_operator);
					Visible = Enabled? Visibility.Visible:Visibility.Collapsed;
					if (SetProperty(ref _Type, value))
					{
						if (UserChanged!=null) UserChanged();
					}
				}
		}

		//--- Property Icon ---------------------------------------
		private PackIconMaterialKind _Icon;
		public PackIconMaterialKind Icon
		{
			get { return _Icon; }
			set { SetProperty(ref _Icon, value); }
		}

		//--- Property Name ---------------------------------------
		private string _Name;
		public string Name
		{
			get { return _Name; }
			set { SetProperty(ref _Name, value); }
		}

		//--- Property Visible ---------------------------------------
		private Visibility _Visible;
		public Visibility Visible
		{
			get { return _Visible; }
			set { SetProperty(ref _Visible, value); }
		}

		//--- Property Enabled ---------------------------------------
		private bool _Enabled;
		public bool Enabled
		{
			get { return _Enabled; }
			set { SetProperty(ref _Enabled, value); }
		}

		//--- GetName --------------------------------
		public static string GetName(EN_UserType type)
		{
			return FdGolbals.Language.GetText(type.ToString());
		}

		//--- GetIcon --------------------------------
		public static PackIconMaterialKind GetIcon(EN_UserType type)
		{
			switch (type)
			{
				case EN_UserType.USER_supervisor:   return MahApps.Metro.IconPacks.PackIconMaterialKind.AccountEditOutline; 
				case EN_UserType.USER_service:      return MahApps.Metro.IconPacks.PackIconMaterialKind.AccountCogOutline; 
				default:                            return MahApps.Metro.IconPacks.PackIconMaterialKind.AccountOutline;   
			}
		}

		//--- Login -----------------------------------------
		public static bool Login(EN_UserType type, string pwd)
		{
			switch (type)
			{
				case EN_UserType.USER_supervisor:	if (pwd=="user") 
													{
														FdGolbals.User.Type=type; 
														return true;
													}
													return false;

				case EN_UserType.USER_service:		if (pwd=="Paro")  
													{
														FdGolbals.User.Type=type; 
														return true;
													}
													return false;

				case EN_UserType.USER_operator:		FdGolbals.User.Type=type; return true;
			}
			return false;
		}
	}
}
