using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telerik.Windows.Controls;

namespace UrbanHelp
{
    public class CustomLocalizationManager : LocalizationManager
    {
        public override string GetStringOverride(string key)
        {
            switch (key)
            {
                case "GridViewAlwaysVisibleNewRow":
                    return "Нажмите для добавления новой строки";

            }
            return base.GetStringOverride(key);
        }
    }
}
