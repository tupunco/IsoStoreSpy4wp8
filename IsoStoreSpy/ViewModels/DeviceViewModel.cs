using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using IsoStoreSpy.Tools;

namespace IsoStoreSpy.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        
        /// <summary>
        /// Nom de la propriété DeviceType
        /// </summary>

        public const string DeviceTypePropertyName = "DeviceType";

        /// <summary>
        /// propriété DeviceType :  
        /// </summary>

        public DeviceTypes DeviceType
        {
            get
            {
                return this._DeviceType;
            }

            set
            {
                if (this._DeviceType != value)
                {
                    this._DeviceType = value;
                    this.RaisePropertyChanged(DeviceTypePropertyName);
                }
            }
        }

        private DeviceTypes _DeviceType = DeviceTypes.None;

        
        /// <summary>
        /// Nom de la propriété Name
        /// </summary>

        public const string NamePropertyName = "Name";

        /// <summary>
        /// propriété Name :  
        /// </summary>

        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                    this.RaisePropertyChanged(NamePropertyName);
                }
            }
        }

        private string _Name = null;

        /// <summary>
        /// Tous les Devices possibles
        /// </summary>

        public static ObservableCollection<DeviceViewModel> Devices
        {
            get
            {
                if (_Devices == null)
                {
                    _Devices = CreateDeviceViewModels();
                }

                return _Devices;
            }
        }

        private static ObservableCollection<DeviceViewModel> _Devices = null;

        /// <summary>
        /// Trouver un type de device
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>

        public static DeviceViewModel FindDevice(DeviceTypes deviceType)
        {
            return Devices.FirstOrDefault((d) => d.DeviceType == deviceType);
        }

        /// <summary>
        /// Creation des Devices
        /// </summary>
        /// <returns></returns>

        private static ObservableCollection<DeviceViewModel> CreateDeviceViewModels()
        {
            ObservableCollection<DeviceViewModel> result = new ObservableCollection<DeviceViewModel>();
            Array values = Enum.GetValues(typeof(DeviceTypes));

            foreach (DeviceTypes value in values)
            {
                result.Add(new DeviceViewModel()
                {
                    _DeviceType = value,
                    Name = value.ToString()
                }
                );
            }

            return result;
        }
    }
}
