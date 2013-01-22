using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SmartDevice.Connectivity;

namespace IsoStoreSpy.ViewModels
{
    public class RemoteApplicationViewModel : BaseViewModel
    {
        /// <summary>
        /// Nom de la propriété Application
        /// </summary>

        public const string ApplicationPropertyName = "Application";

        /// <summary>
        /// propriété Application :  
        /// </summary>

        public RemoteApplication Application
        {
            get
            {
                return this._Application;
            }

            set
            {
                if (this._Application != value)
                {
                    this._Application = value;
                    this.RaisePropertyChanged(ApplicationPropertyName);
                }
            }
        }

        private RemoteApplication _Application = null;

        /// <summary>
        /// Nom de la propriété Title
        /// </summary>

        public const string TitlePropertyName = "Title";

        /// <summary>
        /// propriété Title :  
        /// </summary>

        public string Title
        {
            get
            {
                return this._Title;
            }

            set
            {
                if (this._Title != value)
                {
                    this._Title = value;
                    this.RaisePropertyChanged(TitlePropertyName);
                }
            }
        }

        private string _Title = null;
    }
}
