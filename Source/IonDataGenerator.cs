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
     * IonGeneratorData Class                               *
     * This classes stores the information related to       *
     * default generators placed in all parts with an       *
     * IonCrewSupportModule attached.                       *
    \*------------------------------------------------------*/
    public class IonGeneratorData : IonData, IConfigNode
    {
        private string _generatorName;
        public string generatorName { get { return _generatorName; } }
        private string _generatorGUIName;
        public string generatorGUIName { get { return _generatorGUIName; } }

        private bool _startOn;
        public bool startOn { get { return _startOn; } }
        private bool _alwaysOn;
        public bool alwaysOn { get { return _alwaysOn; } }

        private float _outputLevelStep;
        public float outputLevelStep { get { return _outputLevelStep; } }
        private float _outputLevelMin;
        public float outputLevelMin { get { return _outputLevelMin; } }
        private float _outputLevelMax;
        public float outputLevelMax { get { return _outputLevelMax; } }

        private bool _hideStatus;
        public bool hideStatus { get { return _hideStatus; } }
        private bool _hideStatusL2;
        public bool hideStatusL2 { get { return _hideStatusL2; } }
        private bool _hideEfficency;
        public bool hideEfficency { get { return _hideEfficency; } }
        private bool _hideOutputControls;
        public bool hideOutputControls { get { return _hideOutputControls; } }
        private bool _hideActivateControls;
        public bool hideActivateControls { get { return _hideActivateControls; } }

        private List<IonResourceData_Generator> _listInputs;
        public List<IonResourceData_Generator> listInputs { get { return _listInputs; } }
        private List<IonResourceData_Generator> _listOutputs;
        public List<IonResourceData_Generator> listOutputs { get { return _listOutputs; } }


        /************************************************************************\
         * IonGeneratorData class                                               *
         * Constructors                                                         *
         * Initilizers                                                          *
        \************************************************************************/        
        protected override void InitilizeValues()
        {
#if DEBUG
            Debug.Log("IonGeneratorData.InitilizeValues()");
#endif
            _generatorName = "";
            _generatorGUIName = "";

            _startOn = false;
            _alwaysOn = false;

            _outputLevelStep = 0.1f;
            _outputLevelMin = 0;
            _outputLevelMax = 1;

            _hideStatus = false;
            _hideStatusL2 = false;
            _hideEfficency = false;
            _hideOutputControls = false;
            _hideActivateControls = false;

            _listInputs = null;
            _listOutputs = null;
        }


        /************************************************************************\
         * IonGeneratorData class                                               *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonGeneratorData.Load()");
            Debug.Log("IonGeneratorData.Load(): node\n" + node.ToString());
#endif
            //Read variables from node
            if (node.HasValue("generatorName"))
                _generatorName = node.GetValue("generatorName");
            else if (node.HasValue("name"))
                _generatorName = node.GetValue("name");

            if (node.HasValue("generatorGUIName"))
                _generatorGUIName = node.GetValue("generatorGUIName");

            if (node.HasValue("startOn"))
                _startOn = "True" == node.GetValue("startOn") || "true" == node.GetValue("startOn") || "TRUE" == node.GetValue("startOn");

            if (node.HasValue("alwaysOn"))
                _alwaysOn = "True" == node.GetValue("alwaysOn") || "true" == node.GetValue("alwaysOn") || "TRUE" == node.GetValue("alwaysOn");

            //Read output level variables
            if (node.HasValue("outputLevelStep"))
            {
                _outputLevelStep = Convert.ToSingle(node.GetValue("outputLevelStep"));
                _outputLevelStep = Math.Min(outputLevelStep, 1.0f);
                _outputLevelStep = Math.Max(outputLevelStep, 0.0f);
            }

            if (node.HasValue("outputLevelMin"))
            {
                _outputLevelMin = Convert.ToSingle(node.GetValue("outputLevelMin"));
                _outputLevelMin = Math.Min(outputLevelMin, 1.0f);
                _outputLevelMin = Math.Max(outputLevelMin, 0.0f);
            }

            if (node.HasValue("outputLevelMax"))
            {
                _outputLevelMax = Convert.ToSingle(node.GetValue("outputLevelMax"));
                _outputLevelMax = Math.Min(outputLevelMax, 1.0f);
                _outputLevelMax = Math.Max(outputLevelMax, 0.0f);
            }

            //Read hide variables
            if (node.HasValue("hideStatus"))
                _hideStatus = "True" == node.GetValue("hideStatus") || "true" == node.GetValue("hideStatus") || "TRUE" == node.GetValue("hideStatus");
            if (node.HasValue("hideStatusL2"))
                _hideStatusL2 = "True" == node.GetValue("hideStatusL2") || "true" == node.GetValue("hideStatusL2") || "TRUE" == node.GetValue("hideStatusL2");
            if (node.HasValue("hideEfficency"))
                _hideEfficency = "True" == node.GetValue("hideEfficency") || "true" == node.GetValue("hideEfficency") || "TRUE" == node.GetValue("hideEfficency");
            if (node.HasValue("hideOutputControls"))
                _hideOutputControls = "True" == node.GetValue("hideOutputControls") || "true" == node.GetValue("hideOutputControls") || "TRUE" == node.GetValue("hideOutputControls");
            if (node.HasValue("hideActivateControls"))
                _hideActivateControls = "True" == node.GetValue("hideActivateControls") || "true" == node.GetValue("hideActivateControls") || "TRUE" == node.GetValue("hideActivateControls");


            //Create lists
            _listInputs = new List<IonResourceData_Generator>();
            _listOutputs = new List<IonResourceData_Generator>();

            //Traverse through subNodes and add to lists
            foreach (ConfigNode subNode in node.nodes)
            {
#if DEBUG
                Debug.Log("IonGeneratorData.Load(): subnode " + subNode.ToString());
#endif
                if ("INPUT_RESOURCE" == subNode.name)
                {
                    IonResourceData_Generator inputResource = new IonResourceData_Generator(subNode);
                    _listInputs.Add(inputResource);
                }
                else if ("OUTPUT_RESOURCE" == subNode.name)
                {
                    IonResourceData_Generator outputResource = new IonResourceData_Generator(subNode);
                    _listOutputs.Add(outputResource);
                }
            }
        }

        /************************************************************************\
         * IonGeneratorData class                                               *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonGeneratorData.Save()");
#endif
            node.AddValue("generatorName", generatorName);
            node.AddValue("generatorGUIName", generatorGUIName);

            node.AddValue("startOn", startOn);
            node.AddValue("alwaysOn", alwaysOn);

            node.AddValue("outputLevelStep", outputLevelStep);
            node.AddValue("outputLevelMin", outputLevelMin);
            node.AddValue("outputLevelMax", outputLevelMax);

            node.AddValue("hideStatus", hideStatus);
            node.AddValue("hideStatusL2", hideStatusL2);
            node.AddValue("hideEfficency", hideEfficency);
            node.AddValue("hideOutputControls", hideOutputControls);
            node.AddValue("hideActivateControls", hideActivateControls);

            if (null != listInputs)
            {
                foreach (IonResourceData_Generator inputResource in listInputs)
                {
                    ConfigNode inputNode = new ConfigNode("INPUT_RESOURCE");
                    inputResource.Save(inputNode);
                }
            }

            if (null != listOutputs)
            {
                foreach (IonResourceData_Generator outputResource in listOutputs)
                {
                    ConfigNode outputNode = new ConfigNode("OUTPUT_RESOURCE");
                    outputResource.Save(outputNode);
                }
            }
#if DEBUG
            Debug.Log("IonGeneratorData.Save(): node\n" + node.ToString());
#endif
        }

    }//end of class
    //==========================================================================================================
    // END of IonGeneratorData Class
    //==========================================================================================================


    /*------------------------------------------------------*\
     * IonCollectorData Class                                *
     * This classes stores the information related to       *
     * default collectors placed in all parts with an       *
     * IonCrewSupportModule attached.                       *
    \*------------------------------------------------------*/
    [Serializable]
    public class IonCollectorData : IonGeneratorData
    {
        public float _minAtmosphere;

        public bool _isAutomaticOxygen;
        public bool _isAutomaticNoOxygen;

        public bool _hideAtmoContents;

        public List<IonResourceData_Generator> listOutputs_oxygen;
        public List<IonResourceData_Generator> listOutputs_noOxygen;

        /************************************************************************\
         * IonCollectorData class                                               *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
            base.Load(node);
#if DEBUG
            Debug.Log("IonSupportCollector.Load()");
            Debug.Log("IonSupportCollector.Load(): node\n" + node.ToString());
#endif
            //Read variables from node
            if (node.HasValue("minAtmosphere"))
                _minAtmosphere = Convert.ToSingle(node.GetValue("minAtmosphere"));

            if (node.HasValue("isAutomaticOxygen"))
                _isAutomaticOxygen = "True" == node.GetValue("isAutomaticOxygen") || "true" == node.GetValue("isAutomaticOxygen") || "TRUE" == node.GetValue("isAutomaticOxygen");

            if (node.HasValue("isAutomaticNoOxygen"))
                _isAutomaticNoOxygen = "True" == node.GetValue("isAutomaticNoOxygen") || "true" == node.GetValue("isAutomaticNoOxygen") || "TRUE" == node.GetValue("isAutomaticNoOxygen");

            if (node.HasValue("hideAtmoContents"))
                _hideAtmoContents = "True" == node.GetValue("hideAtmoContents") || "true" == node.GetValue("hideAtmoContents") || "TRUE" == node.GetValue("hideAtmoContents");


            //Create lists
            listOutputs_oxygen = new List<IonResourceData_Generator>();
            listOutputs_noOxygen = new List<IonResourceData_Generator>();

            //Traverse through subNodes and add to lists
            foreach (ConfigNode subNode in node.nodes)
            {
#if DEBUG
                Debug.Log("IonSupportCollector.Load(): subnode " + subNode.ToString());
#endif
                if ("OUTPUT_RESOURCE_OXYGEN" == subNode.name)
                {
                    IonResourceData_Generator outputResource = new IonResourceData_Generator(subNode);
                    listOutputs_oxygen.Add(outputResource);
                }
                else if ("OUTPUT_RESOURCE_NO_OXYGEN" == subNode.name)
                {
                    IonResourceData_Generator outputResource = new IonResourceData_Generator(subNode);
                    listOutputs_noOxygen.Add(outputResource);
                }
            }
        }

        /************************************************************************\
         * IonCollectorData class                                               *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
            base.Save(node);
#if DEBUG
            Debug.Log("IonSupportCollector.Save()");
#endif
            node.AddValue("minAtmosphere", _minAtmosphere);
            node.AddValue("isAutomaticOxygen", _isAutomaticOxygen);
            node.AddValue("isAutomaticNoOxygen", _isAutomaticNoOxygen);
            node.AddValue("hideAtmoContents", _hideAtmoContents);

            if (null != listOutputs_oxygen)
            {
                foreach (IonResourceData_Generator outputResource in listOutputs_oxygen)
                {
                    ConfigNode outputNode = new ConfigNode("OUTPUT_RESOURCE_OXYGEN");
                    outputResource.Save(outputNode);
                }
            }

            if (null != listOutputs_noOxygen)
            {
                foreach (IonResourceData_Generator outputResource in listOutputs_noOxygen)
                {
                    ConfigNode outputNode = new ConfigNode("OUTPUT_RESOURCE_NO_OXYGEN");
                    outputResource.Save(outputNode);
                }
            }
#if DEBUG
            Debug.Log("IonSupportCollector.Save(): node\n" + node.ToString());
#endif
        }

    }//end of class
    //==========================================================================================================
    // END of IonCollectorData Class
    //==========================================================================================================        
}
