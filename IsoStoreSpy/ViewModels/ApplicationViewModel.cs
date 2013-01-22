using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.SmartDevice.Connectivity;
using IsoStoreSpy.Tools;

namespace IsoStoreSpy.ViewModels
{
    public class ApplicationViewModel : BaseViewModel
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="dropBoxViexModel"></param>
        
        public ApplicationViewModel()
        {
        }

        
        /// <summary>
        /// Nom de la propriété Devices
        /// </summary>

        public const string DevicesPropertyName = "Devices";

        /// <summary>
        /// propriété Devices :  
        /// </summary>

        public ObservableCollection<DeviceViewModel> Devices
        {
            get
            {
                return DeviceViewModel.Devices;
            }
        }

        /// <summary>
        /// Nom de la propriété SelectedDevice
        /// </summary>

        public const string SelectedDevicePropertyName = "SelectedDevice";

        /// <summary>
        /// propriété SelectedDevice :  
        /// </summary>

        public DeviceViewModel SelectedDevice
        {
            get
            {
                return this._SelectedDevice;
            }

            set
            {
                if (value == null)
                {
                    value = DeviceViewModel.FindDevice( DeviceTypes.None ); // le device par defaut
                }

                if (this._SelectedDevice != value)
                {
                    this._SelectedDevice = value;
                    this.RaisePropertyChanged(SelectedDevicePropertyName);

                    /*
                    this.SelectedRemoteApplication = null;

                    try
                    {
                        this.RefreshRemoteApplications();
                    }
                    catch (Exception ex)
                    {
                        this.RemoteApplications = null;
                        this.SelectedRemoteApplication = null;

                        App.ShowError(ex.Message);
                    }*/
                }
            }
        }

        private DeviceViewModel _SelectedDevice = null;

        /// <summary>
        /// Obtenir toutes les applications
        /// </summary>

        public void RefreshRemoteApplications()
        {
            try
            {
                ObservableCollection<RemoteApplicationViewModel> result = new ObservableCollection<RemoteApplicationViewModel>();

                if (this.SelectedDevice != null)
                {
                    List<RemoteApplication> applications = RemoteIsolatedStoreTools.GetAllApplications(this.SelectedDevice.DeviceType);

                    foreach (RemoteApplication application in applications)
                    {
                        result.Add(new RemoteApplicationViewModel()
                            {
                                Application = application
                            }
                        );
                    }
                }

                this.RemoteApplications = result;
            }
            catch
            {
                this.RemoteApplications = null;
                this.SelectedRemoteApplication = null;

                throw;
            }
        }

        
        /// <summary>
        /// Nom de la propriété SelectedRemoteApplication
        /// </summary>

        public const string SelectedRemoteApplicationPropertyName = "SelectedRemoteApplication";

        /// <summary>
        /// propriété SelectedRemoteApplication :  
        /// </summary>

        public RemoteApplicationViewModel SelectedRemoteApplication
        {
            get
            {
                return this._SelectedRemoteApplication;
            }

            set
            {
                if (this._SelectedRemoteApplication != value)
                {
                    this._SelectedRemoteApplication = value;
                    this.RaisePropertyChanged(SelectedRemoteApplicationPropertyName);
                }
            }
        }

        private RemoteApplicationViewModel _SelectedRemoteApplication = null;

        /// <summary>
        /// Nom de la propriété RemoteApplications
        /// </summary>

        public const string RemoteApplicationsPropertyName = "RemoteApplications";

        /// <summary>
        /// propriété RemoteApplications :  
        /// </summary>

        public ObservableCollection<RemoteApplicationViewModel> RemoteApplications
        {
            get
            {
                return this._RemoteApplications;
            }

            set
            {
                if (this._RemoteApplications != value)
                {
                    this._RemoteApplications = value;
                    this.RaisePropertyChanged(RemoteApplicationsPropertyName);
                }
            }
        }

        private ObservableCollection<RemoteApplicationViewModel> _RemoteApplications = new ObservableCollection<RemoteApplicationViewModel>();
    }
}
