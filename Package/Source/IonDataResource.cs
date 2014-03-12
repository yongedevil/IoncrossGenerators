//#define DEBUG
//#define DEBUG_UPDATES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using KSP;

namespace IoncrossKerbal
{
    /*------------------------------------------------------*\
     * IonResourceData Class                                *
     * This classes stores information related to resources *
    \*------------------------------------------------------*/
    public class IonResourceData : IConfigNode, IEquatable<IonResourceData>, IEquatable<string>
    {
        private string name;
        public string Name { get { return name; } }
        private string guiName;
        public string GUIName { get { return guiName; } }

        private int id;
        public int ID { get { return id; } }

        protected double rateBase;
        public double RateBase { get { return rateBase; } }
        protected double ratePerKerbal;
        public double RatePerKerbal { get { return ratePerKerbal; } }
        protected double ratePerCapacity;
        public double RatePerCapacity { get { return ratePerCapacity; } }

        protected double rateBaseMod;
        public double RateBaseMod { get { return rateBase; } }
        protected double ratePerKerbalMod;
        public double RatePerKerbalMod { get { return ratePerKerbalMod; } }
        protected double ratePerCapacityMod;
        public double RatePerCapacityMod { get { return ratePerCapacityMod; } }

        private IonModuleDisplay displayModule;
        public IonModuleDisplay DisplayModule { get { return displayModule; } set { displayModule = value; } }


        /************************************************************************\
         * IonResourceData class                                                *
         * Equals function                                                      *
        \************************************************************************/
        public bool Equals(IonResourceData other)
        {
            return Equals(other.Name);
        }
        /************************************************************************\
         * IonResourceData class                                                *
         * Equals function                                                      *
        \************************************************************************/
        public bool Equals(string resourceName)
        {
            return Name == resourceName;
        }

        /************************************************************************\
         * IonResourceData class                                                *
         * Load function                                                        *
        \************************************************************************/
        public virtual void Load(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonResourceData.Load()");
            Debug.Log("IonResourceData.Load(): node\n" + node.ToString());
#endif
            name = node.GetValue("name");
            id = name.GetHashCode();

            if (node.HasValue("guiName"))
                guiName = node.GetValue("guiName");

            if (node.HasValue("rateBase"))
                rateBase = Convert.ToDouble(node.GetValue("rateBase"));
            else if (node.HasValue("rate"))
                rateBase = Convert.ToDouble(node.GetValue("rate"));
            if (node.HasValue("ratePerKerbal"))
                ratePerKerbal = Convert.ToDouble(node.GetValue("ratePerKerbal"));
            if (node.HasValue("ratePerCapacity"))
                ratePerCapacity = Convert.ToDouble(node.GetValue("ratePerCapacity"));
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
            name = node.GetValue("name");
            id = name.GetHashCode();

            if (node.HasValue("guiName"))
                guiName = node.GetValue("guiName");

            if (node.HasValue("rateBaseMod"))
                rateBaseMod = Convert.ToDouble(node.GetValue("rateBaseMod"));
            else if (node.HasValue("rateMod"))
                rateBaseMod = Convert.ToDouble(node.GetValue("rateMod"));
            if (node.HasValue("ratePerKerbalMod"))
                ratePerKerbalMod = Convert.ToDouble(node.GetValue("ratePerKerbalMod"));
            if (node.HasValue("ratePerCapacityMod"))
                ratePerCapacityMod = Convert.ToDouble(node.GetValue("ratePerCapacityMod"));
        }

        /************************************************************************\
         * IonResourceData class                                                *
         * Save function                                                        *
        \************************************************************************/
        public virtual void Save(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonResourceData.Save()");
#endif
            node.AddValue("name", name);
            if (null != guiName)
                node.AddValue("guiName", guiName);

            node.AddValue("rateBase", rateBase);
            node.AddValue("ratePerKerbal", ratePerKerbal);
            node.AddValue("ratePerCapacity", ratePerCapacity);
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
            node.AddValue("name", name);
            if (null != guiName)
                node.AddValue("guiName", guiName);

            node.AddValue("rateBaseMod", rateBaseMod);
            node.AddValue("ratePerKerbalMod", ratePerKerbalMod);
            node.AddValue("ratePerCapacityMod", ratePerCapacityMod);
#if DEBUG
            Debug.Log("IonResourceData.SaveLocal(): node\n" + node.ToString());
#endif
        }
    }

    /*------------------------------------------------------*\
     * IonResourceDataGlobal Class                          *
     * This classes stores global information related to a  *
     * resource used by kerbals or their pods in the crew   *
     * support system.                                      *
    \*------------------------------------------------------*/
    public class IonResourceData_Global : IonResourceData
    {
        private double evaAmount;
        public double EVAamount { get { return evaAmount; } }
        private double evaMaxAmount;
        public double EVAMaxAmount { get { return evaMaxAmount; } }

        //Effect upon being unable to fulfill request for the resource
        //Note: causeLock will only apply to parts with a ModuleCommand attached
        private bool causeLock;
        public bool CauseLock { get { return causeLock; } }
        private bool causeDeath;
        public bool CauseDeath { get { return causeDeath; } }

        //If causeDeath is true then every timeKillRollInterval each kerbal onboard has killChance to be killed (provided at least the minimum framesWithoutResource has passed)
        private double killRollInterval;
        public double KillRollInterval { get { return killRollInterval; } }
        private float killChance;
        public float KillChance { get { return killChance; } }


        /************************************************************************\
         * IonResourceDataGlobal class                                          *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
            base.Load(node);
#if DEBUG
            Debug.Log("IonResourceDataGlobal.Load()");
            Debug.Log("IonResourceDataGlobal.Load(): node\n" + node.ToString());
#endif
            if (node.HasValue("evaAmount"))
                evaAmount = Convert.ToDouble(node.GetValue("evaAmount"));
            if (node.HasValue("evaMaxAmount"))
                evaMaxAmount = Convert.ToDouble(node.GetValue("evaMaxAmount"));

            if (node.HasValue("causeLock"))
                causeLock = "True" == node.GetValue("causeLock") || "true" == node.GetValue("causeLock") || "TRUE" == node.GetValue("causeLock");
           
            if (node.HasValue("causeDeath"))
                causeDeath = "True" == node.GetValue("causeDeath") || "true" == node.GetValue("causeDeath") || "TRUE" == node.GetValue("causeDeath");
           
            if (causeDeath)
            {
                if (node.HasValue("killRollInterval"))
                    killRollInterval = Convert.ToDouble(node.GetValue("killRollInterval"));
                if (node.HasValue("killChance"))
                    killChance = Convert.ToSingle(node.GetValue("killChance"));
            }
        }

        /************************************************************************\
         * IonResourceDataGlobal class                                          *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
            base.Save(node);
#if DEBUG
            Debug.Log("IonResourceDataGlobal.Save()");
#endif
            node.AddValue("evaAmount", evaAmount);
            node.AddValue("evaMaxAmount", evaMaxAmount);

            node.AddValue("causeLock", causeLock);
            node.AddValue("causeDeath", causeDeath);

            if (causeDeath)
            {
                node.AddValue("killRollInterval", killRollInterval);
                node.AddValue("killChance", killChance);
            }
#if DEBUG
            Debug.Log("IonResourceDataGlobal.Save(): node\n" + node.ToString());
#endif
        }

    }//end of class
    //==========================================================================================================
    // END of IonResourceDataGlobal Class
    //==========================================================================================================
    
    /*------------------------------------------------------*\
     * IonSupportResourceDataLocal Class                    *
     * This classes stores local information related to a   *
     * resource used by kerbals or their pods in the crew   *
     * support system.                                      *
    \*------------------------------------------------------*/
    public class IonResourceData_Local : IonResourceData
    {
        public IonResourceData_Global globalData;

        public new double RateBase { get { return (null != globalData ? globalData.RateBase : 0); } }
        public new double RatePerKerbal { get { return (null != globalData ? globalData.RatePerKerbal : 0); } }
        public new double RatePerCapacity { get { return (null != globalData ? globalData.RatePerCapacity : 0); } }

        private double timeSinceLastKillRoll;
        public double TimeSinceLastKillRoll { get { return timeSinceLastKillRoll; } set { timeSinceLastKillRoll = value; } }
        private int framesWithoutResource;
        public int FramesWithoutResource { get { return framesWithoutResource; } set { framesWithoutResource = value; } }

        public bool CauseLock { get { return (null != globalData ? globalData.CauseLock : false); } }
        public bool CauseDeath { get { return (null != globalData ? globalData.CauseDeath : false); } }

        public double KillRollInterval { get { return (null != globalData ? globalData.KillRollInterval : 0); } }
        public float KillChance { get { return (null != globalData ? globalData.KillChance : 0); } }

        /************************************************************************\
         * IonResourceDataLocal class                                           *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
            base.LoadLocal(node);
#if DEBUG
            Debug.Log("IonResourceDataLocal.Load()");
            Debug.Log("IonResourceDataLocal.Load(): node\n" + node.ToString());
#endif
            if (node.HasValue("timeSinceLastKillRoll"))
                timeSinceLastKillRoll = Convert.ToDouble(node.GetValue("timeSinceLastKillRoll"));
            if (node.HasValue("framesWithoutResource"))
                framesWithoutResource = Convert.ToInt32(node.GetValue("framesWithoutResource"));
        }

        /************************************************************************\
         * IonResourceDataLocal class                                           *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
            base.SaveLocal(node);
#if DEBUG
            Debug.Log("IonResourceDataLocal.Save()");
#endif
            node.AddValue("timeSinceLastKillRoll", timeSinceLastKillRoll);
            node.AddValue("framesWithoutResource", framesWithoutResource);
#if DEBUG
            Debug.Log("IonResourceDataLocal.Save(): node\n" + node.ToString());
#endif
        }
    }//end of class
    //==========================================================================================================
    // END of IonResourceDataLocal Class
    //==========================================================================================================

    /*------------------------------------------------------*\
     * IonResourceDataGenerator Class                       *
     * This classes stores information related to a         *
     * generator resource for a part.                       *
    \*------------------------------------------------------*/
    public class IonResourceData_Generator : IonResourceData
    {
        private double curAvailable;
        public double CurAvailable { get { return curAvailable; } set { curAvailable = value; } }
        private double curFreeAmount;
        public double CurFreeAmount { get { return curFreeAmount; } set { curFreeAmount = value; } }
        private double curRequest;
        public double CurRequest { get { return curRequest; } set { curRequest = value; } }

        private float effectOnEfficency;
        public float EffectOnEfficency { get { return effectOnEfficency; } }
        private float cutoffMargin;
        public float CutoffMargin { get { return cutoffMargin; } }


        /************************************************************************\
         * IonResourceDataGenerator class                                       *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
            base.Load(node);
#if DEBUG
            Debug.Log("IonResourceDataGenerator.Load()");
            Debug.Log("IonResourceDataGenerator.Load(): node\n" + node.ToString());
#endif
            if (node.HasValue("effectOnEfficency"))
            {
                effectOnEfficency = Convert.ToSingle(node.GetValue("effectOnEfficency"));
                if (effectOnEfficency > 1)
                    effectOnEfficency = 1;
                else if (effectOnEfficency < 0)
                    effectOnEfficency = 0;
            }

            if (node.HasValue("cutoffMargin"))
            {
                cutoffMargin = Convert.ToSingle(node.GetValue("cutoffMargin"));
                if (cutoffMargin > 1)
                    cutoffMargin = 1;
                else if (cutoffMargin < 0)
                    cutoffMargin = 0;
            }
        }
        /************************************************************************\
         * IonResourceDataGenerator class                                       *
         * LoadLocal function                                                   *
        \************************************************************************/
        public override void LoadLocal(ConfigNode node)
        {
            base.LoadLocal(node);
#if DEBUG
            Debug.Log("IonResourceDataGenerator.LoadLocal()");
            Debug.Log("IonResourceDataGenerator.LoadLocal(): node\n" + node.ToString());
#endif
        }

        /************************************************************************\
         * IonResourceDataGenerator class                                       *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
            base.Save(node);
#if DEBUG
            Debug.Log("IonResourceDataGenerator.Save()");
#endif
            node.AddValue("effectOnEfficency", effectOnEfficency);
            node.AddValue("cutoffMargin", cutoffMargin);
#if DEBUG
            Debug.Log("IonResourceDataGenerator.Save(): node\n" + node.ToString());
#endif
        }
        /************************************************************************\
         * IonResourceDataGenerator class                                       *
         * SaveLocal function                                                   *
        \************************************************************************/
        public override void SaveLocal(ConfigNode node)
        {
            base.SaveLocal(node);
#if DEBUG
            Debug.Log("IonResourceDataGenerator.SaveLocal()");
#endif
#if DEBUG
            Debug.Log("IonResourceDataGenerator.SaveLocal(): node\n" + node.ToString());
#endif
        }
    }//end of class
    //==========================================================================================================
    // END of IonResourceDataGenerator Class
    //==========================================================================================================

}
