//#define DEBUG
//#define DEBUG_UPDATES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using KSP;

namespace IoncrossKerbal_Generator
{
    public abstract class IonData : IConfigNode
    {
        public IonData()
        {
            InitilizeValues();
        }
        public IonData(ConfigNode node)
        {
            InitilizeValues();
            Load(node);
        }

        protected abstract void InitilizeValues();
        public abstract void Load(ConfigNode node);
        public abstract void Save(ConfigNode node);
    }

    /*------------------------------------------------------*\
     * IonResourceData Class                                *
     * This classes stores information related to resources *
    \*------------------------------------------------------*/
    public class IonResourceData : IonData, IEquatable<IonResourceData>, IEquatable<string>
    {
        public const string RESOURCE_NAME = "name";
        public const string RATE = "rate";
        public const string RATE_BASE = "rateBase";
        public const string RATE_PERKERBAL = "ratePerKerbal";
        public const string RATE_PERCAPACITY = "ratePerCapacity";
        public const string RATEMOD = "rateMod";
        public const string RATEMOD_BASE = "rateBaseMod";
        public const string RATEMOD_PERKERBAL = "ratePerKerbalMod";
        public const string RATEMOD_PERCAPACITY = "ratePerCapacityMod";
        

        private string _resourceName;
        public string resourceName { get { return _resourceName; } }

        private int _id;
        public int id { get { return _id; } }

        protected double _rateBase;
        public double rateBase { get { return _rateBase; } }
        protected double _ratePerKerbal;
        public double ratePerKerbal { get { return _ratePerKerbal; } }
        protected double _ratePerCapacity;
        public double ratePerCapacity { get { return _ratePerCapacity; } }

        protected double _rateBaseMod;
        public double rateBaseMod { get { return _rateBase; } }
        protected double _ratePerKerbalMod;
        public double ratePerKerbalMod { get { return _ratePerKerbalMod; } }
        protected double _ratePerCapacityMod;
        public double ratePerCapacityMod { get { return _ratePerCapacityMod; } }

        private IonModuleDisplay _displayModule;
        public IonModuleDisplay displayModule { get { return _displayModule; } set { _displayModule = value; } }


        /************************************************************************\
         * IonResourceData class                                                *
         * Constructors                                                         *
         * Initilizers                                                          *
        \************************************************************************/
        public IonResourceData() : base()
        {

        }
        public IonResourceData(ConfigNode node) : base(node)
        {

        }

        protected override void InitilizeValues()
        {
#if DEBUG
            Debug.Log("IonResourceData.InitilizeValues()");
#endif
            _resourceName = "";
            _id = 0;

            _rateBase = 0;
            _ratePerKerbal = 0;
            _ratePerCapacity = 0;

            _rateBaseMod = 1;
            _ratePerKerbalMod = 1;
            _ratePerCapacityMod = 1;

            _displayModule = null;
        }


        public void AddDisplayRate(float rate)
        {
            if (null != displayModule)
            {
                displayModule.curRate += rate;
            }
        }

        public void SetDisplayRate(float rate)
        {
            if(null != displayModule)
            {
                displayModule.curRate = rate;
            }
        }

        /************************************************************************\
         * IonResourceData class                                                *
         * Equals function                                                      *
        \************************************************************************/
        public bool Equals(IonResourceData other)
        {
            return Equals(other._resourceName);
        }
        /************************************************************************\
         * IonResourceData class                                                *
         * Equals function                                                      *
        \************************************************************************/
        public bool Equals(string resourceName)
        {
            return _resourceName == resourceName;
        }

        /************************************************************************\
         * IonResourceData class                                                *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonResourceData.Load()");
            Debug.Log("IonResourceData.Load(): node\n" + node.ToString());
#endif
            _resourceName = node.GetValue(RESOURCE_NAME);
            _id = _resourceName.GetHashCode();

            _rateBase = 0.0;
            _ratePerKerbal = 0.0;
            _ratePerCapacity = 0.0;
            ParseHelper.ReadValue(node, new string[] { RATE_BASE, RATE }, ref _rateBase);
            ParseHelper.ReadValue(node, RATE_PERKERBAL, ref _ratePerKerbal);
            ParseHelper.ReadValue(node, RATE_PERCAPACITY, ref _ratePerCapacity);
        }
        /************************************************************************\
         * IonResourceData class                                                *
         * LoadLocal function                                                   *
        \************************************************************************/
        public virtual void LoadLocal(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonResourceData.LoadLocal()");
            Debug.Log("IonResourceData.LoadLocal(): node\n" + node.ToString());
#endif
            _resourceName = node.GetValue(RESOURCE_NAME);
            _id = _resourceName.GetHashCode();

            _rateBaseMod = 1.0;
            _ratePerKerbalMod = 1.0;
            _ratePerCapacityMod = 1.0;
            ParseHelper.ReadValue(node, new string[] { RATEMOD_BASE, RATEMOD }, ref _rateBaseMod);
            ParseHelper.ReadValue(node, RATEMOD_PERKERBAL, ref _ratePerKerbalMod);
            ParseHelper.ReadValue(node, RATEMOD_PERCAPACITY, ref _ratePerCapacityMod);
        }

        /************************************************************************\
         * IonResourceData class                                                *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonResourceData.Save()");
#endif
            node.AddValue(RESOURCE_NAME, _resourceName);

            node.AddValue(RATE_BASE, _rateBase);
            node.AddValue(RATE_PERKERBAL, _ratePerKerbal);
            node.AddValue(RATE_PERCAPACITY, _ratePerCapacity);
#if DEBUG
            Debug.Log("IonResourceData.Save(): node\n" + node.ToString());
#endif
        }
        /************************************************************************\
         * IonResourceData class                                                *
         * SaveLocal function                                                   *
        \************************************************************************/
        public virtual void SaveLocal(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonResourceData.SaveLocal()");
#endif
            node.AddValue(RESOURCE_NAME, _resourceName);

            node.AddValue(RATEMOD_BASE, _rateBaseMod);
            node.AddValue(RATEMOD_PERKERBAL, _ratePerKerbalMod);
            node.AddValue(RATEMOD_PERCAPACITY, _ratePerCapacityMod);
#if DEBUG
            Debug.Log("IonResourceData.SaveLocal(): node\n" + node.ToString());
#endif
        }

    }//end of class
    //==========================================================================================================
    // END of IonResourceData Class
    //==========================================================================================================


}
