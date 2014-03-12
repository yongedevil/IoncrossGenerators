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
    /*------------------------------------------------------*\
     * IonResourceData_Generator Class                      *
     * This classes stores information related to a         *
     * generator resource                                   *
    \*------------------------------------------------------*/
    public class IonResourceData_Generator : IonResourceData
    {
        public const string EFFECT_ON_EFFICIENCY = "effectOnEfficiency";
        public const string REQUIRED_RESOURCE = "requiredResource";
        public const string CUTOFF_MARGIN = "cutoffMargin";


        private double _curAvailable;
        public double curAvailable { get { return _curAvailable; } set { _curAvailable = value; } }
        private double _curFreeAmount;
        public double curFreeAmount { get { return _curFreeAmount; } set { _curFreeAmount = value; } }
        //private double _curRequest;
        //public double curRequest { get { return _curRequest; } set { _curRequest = value; } }
        //private double _curReturned;
        //public double curReturned { get { return _curReturned; } set { _curReturned = value; } }
        private double _curRateRequested;
        public double curRateRequested { get { return _curRateRequested; } set { _curRateRequested = value; } }
        private double _curRateReturned;
        public double curRateReturned { get { return _curRateReturned; } set { _curRateReturned = value; } }

        private float _effectOnEfficiency;
        public float effectOnEfficiency { get { return _effectOnEfficiency; } }
        private float _cutoffMargin;
        public float cutoffMargin { get { return _cutoffMargin; } }


        /************************************************************************\
         * IonResourceData_Generator class                                      *
         * Constructors                                                         *
         * Initilizers                                                          *
        \************************************************************************/
        public IonResourceData_Generator() : base()
        {

        }
        public IonResourceData_Generator(ConfigNode node) : base (node)
        {

        }

        protected override void InitilizeValues()
        {
            base.InitilizeValues();
#if DEBUG
            Debug.Log("IonResourceData_Generator.InitilizeValues()");
#endif
            _curAvailable = 0;
            _curFreeAmount = 0;
            _curRateRequested = 0;
            _curRateReturned = 0;

            _effectOnEfficiency = 0;
            _cutoffMargin = 0;
        }

        /************************************************************************\
         * IonResourceData_Generator class                                      *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
            base.Load(node);
#if DEBUG
            Debug.Log("IonResourceData_Generator.Load()");
            Debug.Log("IonResourceData_Generator.Load(): node\n" + node.ToString());
#endif

            _effectOnEfficiency = 0;
            _cutoffMargin = 0;

            if (ParseHelper.ReadValue(node, EFFECT_ON_EFFICIENCY, ref _effectOnEfficiency))
            {
                if (_effectOnEfficiency > 1)
                    _effectOnEfficiency = 1;
                else if (_effectOnEfficiency < 0)
                    _effectOnEfficiency = 0;
            }
            else if (node.HasValue(REQUIRED_RESOURCE))
            {
                if ("TRUE" == node.GetValue(REQUIRED_RESOURCE).ToUpper())
                    _effectOnEfficiency = 1;
                else
                    _effectOnEfficiency = 0;
            }

            if (ParseHelper.ReadValue(node, CUTOFF_MARGIN, ref _cutoffMargin))
            {
                if (_cutoffMargin > 1)
                    _cutoffMargin = 1;
                else if (_cutoffMargin < 0)
                    _cutoffMargin = 0;
            }
        }
        /************************************************************************\
         * IonResourceData_Generator class                                      *
         * LoadLocal function                                                   *
        \************************************************************************/
        public override void LoadLocal(ConfigNode node)
        {
            base.LoadLocal(node);
#if DEBUG
            Debug.Log("IonResourceData_Generator.LoadLocal()");
            Debug.Log("IonResourceData_Generator.LoadLocal(): node\n" + node.ToString());
#endif
        }

        /************************************************************************\
         * IonResourceData_Generator class                                      *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
            base.Save(node);
#if DEBUG
            Debug.Log("IonResourceData_Generator.Save()");
#endif
            node.AddValue(EFFECT_ON_EFFICIENCY, _effectOnEfficiency);
            node.AddValue(CUTOFF_MARGIN, _cutoffMargin);
#if DEBUG
            Debug.Log("IonResourceData_Generator.Save(): node\n" + node.ToString());
#endif
        }
        /************************************************************************\
         * IonResourceData_Generator class                                      *
         * SaveLocal function                                                   *
        \************************************************************************/
        public override void SaveLocal(ConfigNode node)
        {
            base.SaveLocal(node);
#if DEBUG
            Debug.Log("IonResourceData_Generator.SaveLocal()");
#endif
#if DEBUG
            Debug.Log("IonResourceData_Generator.SaveLocal(): node\n" + node.ToString());
#endif
        }
    }//end of class
    //==========================================================================================================
    // END of IonResourceData_Generator Class
    //==========================================================================================================
    
    /*------------------------------------------------------*\
     * IonResourceData_GeneratorCatalyst Class              *
     * This classes stores information related to a         *
     * generator catalyst.                                  *
    \*------------------------------------------------------*/
    public class IonResourceData_GeneratorCatalyst : IonResourceData_Generator
    {
        public const string EFFECT_ON_EFFICIENCY_INPUTS = "effectOnInputEfficiency";
        public const string EFFECT_ON_EFFICIENCY_OUTPUTS = "effectOnOutputEfficiency";

        private float _effectOnEfficiency_inputs;
        private float _effectOnEfficiency_outputs;
        public float effectOnEfficiency_inputs { get { return _effectOnEfficiency_inputs; } }
        public float effectOnEfficiency_outputs { get { return _effectOnEfficiency_outputs; } }


        /************************************************************************\
         * IonResourceData_GeneratorCatalyst class                              *
         * Constructors                                                         *
         * Initilizers                                                          *
        \************************************************************************/
        public IonResourceData_GeneratorCatalyst() : base()
        {

        }
        public IonResourceData_GeneratorCatalyst(ConfigNode node) : base(node)
        {

        }

        protected override void InitilizeValues()
        {
            base.InitilizeValues();
#if DEBUG
            Debug.Log("IonResourceData_GeneratorCatalyst.InitilizeValues()");
#endif
            _effectOnEfficiency_inputs = 0;
            _effectOnEfficiency_outputs = 0;
        }

        /************************************************************************\
         * IonResourceData_GeneratorCatalyst class                              *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
            base.Load(node);
#if DEBUG
            Debug.Log("IonResourceData_GeneratorCatalyst.Load()");
            Debug.Log("IonResourceData_GeneratorCatalyst.Load(): node\n" + node.ToString());
#endif

            _effectOnEfficiency_inputs = 0.0f;
            _effectOnEfficiency_outputs = 0.0f;

            if (ParseHelper.ReadValue(node, EFFECT_ON_EFFICIENCY_INPUTS, ref _effectOnEfficiency_inputs))
            {
                if (_effectOnEfficiency_inputs > 1)
                    _effectOnEfficiency_inputs = 1;
                else if (_effectOnEfficiency_inputs < 0)
                    _effectOnEfficiency_inputs = 0;
            }

            if (ParseHelper.ReadValue(node, EFFECT_ON_EFFICIENCY_OUTPUTS, ref _effectOnEfficiency_outputs))
            {
                if (_effectOnEfficiency_outputs > 1)
                    _effectOnEfficiency_outputs = 1;
                else if (_effectOnEfficiency_outputs < 0)
                    _effectOnEfficiency_outputs = 0;
            }
        }

        /************************************************************************\
         * IonResourceData_GeneratorCatalyst class                              *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
            base.Save(node);
#if DEBUG
            Debug.Log("IonResourceData_GeneratorCatalyst.Save()");
#endif
            node.AddValue(EFFECT_ON_EFFICIENCY_INPUTS, _effectOnEfficiency_inputs);
            node.AddValue(EFFECT_ON_EFFICIENCY_OUTPUTS, _effectOnEfficiency_outputs);
#if DEBUG
            Debug.Log("IonResourceData_GeneratorCatalyst.Save(): node\n" + node.ToString());
#endif
        }
    }//end of class
    //==========================================================================================================
    // END of IonResourceData_GeneratorCatalyst Class
    //==========================================================================================================
}
