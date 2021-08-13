using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JdaTools.Studio.ViewModels
{
    public abstract class ViewModelBase : PropertyChangedBase
    {
        protected bool SetProperty<T>(ref T Storage, T Value, [CallerMemberName] string Propertname = null)
        {
            if (EqualityComparer<T>.Default.Equals(Storage, Value)) return false;
            Storage = Value;
            NotifyOfPropertyChange(Propertname);
            return true;
        }
    }
}
