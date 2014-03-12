//#define DEBUG
//#define DEBUG_UPDATES
//#define DEBUG_GUI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using KSP;

namespace IoncrossKerbal_Generator
{
    class IonModuleGenerator : PartModule
    {
        public const string GENERATOR_NAME = "generatorName";
        public const string GENERATOR_GUI_NAME = "generatorGUIName";
        public const string ALWAYS_ON = "alwaysOn";
        public const string START_ON = "startOn";
        public const string IS_ACTIVE = "active";
        public const string OUTPUT_LEVEL = "outputLevel";
        public const string OUTPUT_LEVEL_STEP = "outputLevel_step";
        public const string OUTPUT_LEVEL_MIN = "outputLevel_min";
        public const string OUTPUT_LEVEL_MAX = "outputLevel_max";
        public const string HIDE_STATUS = "hide_Status";
        public const string HIDE_STATUSL2 = "hide_StatusL2";
        public const string HIDE_EFFICIENCY = "hide_Efficiency";
        public const string HIDE_OUTPUT_SETTING = "hide_OutputSetting";
        public const string HIDE_OUTPUT_ACTUAL = "hide_OutputActual";
        public const string HIDE_OUTPUT_CONTROLS = "hide_OutputControls";
        public const string HIDE_ACTIVATE_CONTROLS = "hide_ActivateControls";
        public const string HIDE_GUI_CONTROLS = "hide_GUIControls";
        public const string HIDE_RESOURCE_RATES = "hide_ResourceRates";

        public const string NODE_INPUT = "INPUT_RESOURCE";
        public const string NODE_OUTPUT = "OUTPUT_RESOURCE";
        public const string NODE_CATALYST = "CATALYST_RESOURCE";

        public List<ConfigNode> list_resouceNodes;
        public List<IonResourceData_Generator> list_inputs;
        public List<IonResourceData_Generator> list_outputs;
        public List<IonResourceData_Generator> list_catalysts;

        public IonGUIGenerator guiController;

        private IonResourceData_Generator _limitingResource;
        private float _limitFactor;
        public IonResourceData_Generator limitngResource { get { return _limitingResource; } }
        public float limitFactor { get { return _limitFactor; } }

        private float _inputModifier;
        private float _outputModifier;

        public bool isActive;
        public float outputLevel;
        public virtual bool isAble { get { return true; } }

        public string generatorName;
        public string generatorGUIName;

        public bool alwaysOn;
        public float outputLevel_step;
        public float outputLevel_min;
        public float outputLevel_max;

        public bool hide_Status;
        public bool hide_StatusL2;
        public bool hide_Efficiency;
        public bool hide_OutputSetting;
        public bool hide_OutputActual;
        public bool hide_OutputControls;
        public bool hide_ActivateControls;
        public bool hide_GUIControls;
        public bool hide_ResourceRates;

        [KSPField(guiActive = true, guiName = "Generator Status", isPersistant = false)]
        public string display_Status;

        [KSPField(guiActive = true, guiName = " ", isPersistant = false)]
        public string display_StatusL2;

        [KSPField(guiActive = true, guiName = "Set Output", guiUnits = "%", guiFormat = "F2", isPersistant = false)]
        public float display_OutputSetting;

        [KSPField(guiActive = true, guiName = "Actual Output", guiUnits = "%", guiFormat = "F2", isPersistant = false)]
        public float display_OutputActual;

        [KSPField(guiActive = true, guiName = "Efficency", guiUnits = "%", guiFormat = "F2", isPersistant = false)]
        public float display_Efficiency;


        /*****************************\
         * Activate/Deactive Buttons *
        \*****************************/
        [KSPEvent(guiActive = true, guiName = "Activate Generator")]
        public void ActivateButton()
        {
            SetGeneratorState(true);
        }

        [KSPEvent(guiActive = true, guiName = "Shutdown Generator")]
        public void ShutdownButton()
        {
            SetGeneratorState(false);
        }

        [KSPEvent(guiActive = true, guiName = "Increase Output")]
        public void IncreaseButton()
        {
            ChangeGeneratorLevel(outputLevel_step);
        }

        [KSPEvent(guiActive = true, guiName = "Decrease Output")]
        public void DecreaseButton()
        {
            ChangeGeneratorLevel(-outputLevel_step);
        }

        [KSPEvent(guiActive = true, guiName = "Open GUI")]
        public void OpenGUIButton()
        {
            SetGeneratorGUI(true);
        }

        [KSPEvent(guiActive = true, guiName = "Close GUI")]
        public void CloseGUIButton()
        {
            SetGeneratorGUI(false);
        }


        /*****************************\
         * Activate/Deactive Actions *
        \*****************************/
        [KSPAction("Activate Generator")]
        public void ActivateAction(KSPActionParam param)
        {
            SetGeneratorState(true);
        }

        [KSPAction("Shutdown Generator")]
        public void ShutdownAction(KSPActionParam param)
        {
            SetGeneratorState(false);
        }

        [KSPAction("Toggle Generator")]
        public void ToggleAction(KSPActionParam param)
        {
            SetGeneratorState(!isActive);
        }

        [KSPAction("Increase Output")]
        public void IncreaseAction(KSPActionParam param)
        {
            ChangeGeneratorLevel(outputLevel_step);
        }

        [KSPAction("Decrease Output")]
        public void DecreaseAction(KSPActionParam param)
        {
            ChangeGeneratorLevel(-outputLevel_step);
        }

        

        /********************************************\
         * Set Generator Status and Level Functions *
         * Used by above actions and buttons.       *
        \********************************************/
        public void SetGeneratorState(bool generatorState)
        {
            if (!alwaysOn)
            {
                isActive = generatorState;
                display_Status = isActive ? "Active" : "Inactive";

                if (!hide_ActivateControls)
                {
                    Events["ActivateButton"].active = !isActive;
                    Events["ShutdownButton"].active = isActive;
                }
            }
            else
                display_Status = "Active";

            //Reset curRateRequested and curRateReturned to 0 for all resouces
            //Reset _limitingResource and _limitFactor
            if (!isActive)
                ResetRates();
        }

        public void ChangeGeneratorLevel(float outputIncrement)
        {
            SetGeneratorLevel(outputLevel + outputIncrement);
        }

        public void SetGeneratorLevel(float newLevel)
        {
            outputLevel = newLevel;

            if (outputLevel > outputLevel_max)
                outputLevel = outputLevel_max;
            else if (outputLevel < outputLevel_min)
                outputLevel = outputLevel_min;

            display_OutputSetting = outputLevel * 100.0f;
            guiController.strOutputLevel = outputLevel * 100;
        }

        public void SetGeneratorGUI(bool guiOpen)
        {
            guiController.isActive = guiOpen;

            if (!hide_GUIControls)
            {
                Events["OpenGUIButton"].active = !guiController.isActive;
                Events["CloseGUIButton"].active = guiController.isActive;
            }
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * OnAwake function                                                     *
         *                                                                      *
        \************************************************************************/
        public override void OnAwake()
        {
            base.OnAwake();
#if DEBUG
            Debug.Log("IonModuleGenerator.OnAwake() " + this.part.name + " " + generatorName);
#endif
        }

        /************************************************************************\
         * IonModuleGenerator class                                             *
         * OnLoad function                                                      *
         *                                                                      *
        \************************************************************************/
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
#if DEBUG
            Debug.Log("IonModuleGenerator.OnLoad() " + this.part.name + " " + generatorName);
            Debug.Log("IonModuleGenerator.OnLoad(): node\n" + node.ToString());
#endif
            list_resouceNodes = new List<ConfigNode>();
            list_inputs = null;
            list_outputs = null;

            //Name variables
            if (node.HasValue(GENERATOR_NAME))
                generatorName = node.GetValue(GENERATOR_NAME);
            else
                generatorName = "";

            if (node.HasValue(GENERATOR_GUI_NAME))
                generatorGUIName = node.GetValue(GENERATOR_GUI_NAME);
            else
                generatorGUIName = generatorName;


            //Active variables
            alwaysOn = false;
            ParseHelper.ReadValue(node, ALWAYS_ON, ref alwaysOn);

            isActive = alwaysOn;
            if (!alwaysOn)
                ParseHelper.ReadValue(node, new string[] {IS_ACTIVE, START_ON}, ref isActive);


            //Output level variables
            outputLevel = 1.0f;
            outputLevel_step = 0.1f;
            outputLevel_min = 0.0f;
            outputLevel_max = 1.0f;

            ParseHelper.ReadValue(node, OUTPUT_LEVEL, ref outputLevel);
            ParseHelper.ReadValue(node, OUTPUT_LEVEL_STEP, ref outputLevel_step);
            ParseHelper.ReadValue(node, OUTPUT_LEVEL_MIN, ref outputLevel_min);
            ParseHelper.ReadValue(node, OUTPUT_LEVEL_MAX, ref outputLevel_max);


            //Hide variables
            hide_Status = false;
            hide_StatusL2 = false;
            hide_Efficiency = false;
            hide_OutputSetting = false;
            hide_OutputActual = false;
            hide_OutputControls = false;
            hide_ActivateControls = false;
            hide_GUIControls = false;
            hide_ResourceRates = false;

            ParseHelper.ReadValue(node, HIDE_STATUS, ref hide_Status);
            ParseHelper.ReadValue(node, HIDE_STATUSL2, ref hide_StatusL2);
            ParseHelper.ReadValue(node, HIDE_EFFICIENCY, ref hide_Efficiency);
            ParseHelper.ReadValue(node, HIDE_OUTPUT_SETTING, ref hide_OutputSetting);
            ParseHelper.ReadValue(node, HIDE_OUTPUT_ACTUAL, ref hide_OutputActual);
            ParseHelper.ReadValue(node, HIDE_OUTPUT_CONTROLS, ref hide_OutputControls);
            ParseHelper.ReadValue(node, HIDE_ACTIVATE_CONTROLS, ref hide_ActivateControls);
            ParseHelper.ReadValue(node, HIDE_GUI_CONTROLS, ref hide_GUIControls);
            ParseHelper.ReadValue(node, HIDE_RESOURCE_RATES, ref hide_ResourceRates);

            //Resource nodes
            //Resource data is saved to list_resouceNodes because Kerbal will not copy lists of custom classes over when cloning object (I don't know why built in classes such as ConfigNode work and my custom classes like IonResourceData don't)
            //Resource data is therefore left as ConfigNodes untill OnStart is called.
            foreach(ConfigNode subNode in node.nodes)
            {
                if (IsValidResource(subNode))
               {
                   list_resouceNodes.Add(subNode);
               }
            }
        }

        /************************************************************************\
         * IonModuleGenerator class                                             *
         * OnSave function                                                      *
         *                                                                      *
        \************************************************************************/
        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
#if DEBUG
            Debug.Log("IonModuleGenerator.OnSave() " + this.part.name + " " + generatorName);
#endif
            //name variables
            node.AddValue(GENERATOR_NAME, generatorName);
            node.AddValue(GENERATOR_GUI_NAME, generatorGUIName);

            //Active variables
            if (alwaysOn)
                node.AddValue(ALWAYS_ON, alwaysOn);
            else
                node.AddValue(IS_ACTIVE, isActive);
            
            //Output level variables
            node.AddValue(OUTPUT_LEVEL, outputLevel);
            node.AddValue(OUTPUT_LEVEL_STEP, outputLevel_step);
            node.AddValue(OUTPUT_LEVEL_MIN, outputLevel_min);
            node.AddValue(OUTPUT_LEVEL_MAX, outputLevel_max);

            //Hide variables
            if (hide_Status)
                node.AddValue(HIDE_STATUS, hide_Status);
            if (hide_StatusL2)
                node.AddValue(HIDE_STATUSL2, hide_StatusL2);
            if (hide_Efficiency)
                node.AddValue(HIDE_EFFICIENCY, hide_Efficiency);
            if (hide_OutputSetting)
                node.AddValue(HIDE_OUTPUT_SETTING, hide_OutputSetting);
            if (hide_OutputActual)
                node.AddValue(HIDE_OUTPUT_ACTUAL, hide_OutputActual);
            if (hide_OutputControls)
                node.AddValue(HIDE_OUTPUT_CONTROLS, hide_OutputControls);
            if (hide_ActivateControls)
                node.AddValue(HIDE_ACTIVATE_CONTROLS, hide_ActivateControls);
            if (hide_GUIControls)
                node.AddValue(HIDE_GUI_CONTROLS, hide_GUIControls);
            if (hide_ResourceRates)
                node.AddValue(HIDE_RESOURCE_RATES, hide_ResourceRates);

            //Resource lists
            OnSaveList(node, list_catalysts, NODE_CATALYST);
            OnSaveList(node, list_inputs, NODE_INPUT);
            OnSaveList(node, list_outputs, NODE_OUTPUT);

            if (null != list_resouceNodes)
            {
                foreach (ConfigNode subnode in list_resouceNodes)
                {
                    node.AddNode(subnode);
                }
            }
#if DEBUG
            Debug.Log("IonModuleGenerator.OnSave(): node\n" + node.ToString());
#endif
        }

        /************************************************************************\
         * IonModuleGenerator class                                             *
         * OnSaveList function                                                  *
         *                                                                      *
         * Helper function for OnSave.                                          *
        \************************************************************************/
        protected void OnSaveList(ConfigNode node, List<IonResourceData_Generator> list_resources, string nodeType)
        {
            if (null != list_resources)
            {
                foreach (IonResourceData_Generator resource in list_resources)
                {
                    ConfigNode subnode = new ConfigNode(nodeType);
                    resource.Save(subnode);
                    node.AddNode(subnode);
                }
            }
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * OnStart function                                                     *
         *                                                                      *
        \************************************************************************/
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
#if DEBUG
            Debug.Log("IonModuleGenerator.OnStart() " + this.part.name + " " + generatorName);
            Debug.Log("IonModuleGenerator.OnStart(): state " + state.ToString());
#endif
            guiController = new IonGUIGenerator(this);

            SetGeneratorLevel(outputLevel);
            SetGeneratorState(isActive);
            SetGeneratorGUI(false);

            if (null != list_resouceNodes)
            {
                list_inputs = new List<IonResourceData_Generator>();
                list_outputs = new List<IonResourceData_Generator>();
                list_catalysts = new List<IonResourceData_Generator>();

                //add nodes to _inputs and _outputs
                foreach (ConfigNode node in list_resouceNodes)
                {
                    List<IonResourceData_Generator> resourceList = GetList(node);

                    if (null != resourceList)
                    {
                        IonResourceData_Generator resource = new IonResourceData_Generator(node);
                        if(!hide_ResourceRates)
                            resource.displayModule = IonModuleDisplay.FindCreateDisplayModule(this.part, resource.resourceName);

                        resourceList.Add(resource);
                    }
                }

                list_resouceNodes = null;
            }

            
            //Set hidden fields and controls
            if (hide_Status)
                Fields["display_Status"].guiActive = false;
            if (hide_StatusL2)
                Fields["display_StatusL2"].guiActive = false;
            if (hide_Efficiency)
                Fields["display_Efficiency"].guiActive = false;
            if(hide_OutputSetting)
                Fields["display_OutputSetting"].guiActive = false;
            if (hide_OutputActual)
                Fields["display_OutputActual"].guiActive = false;

            if(hide_OutputControls)
            {
                Events["IncreaseButton"].active = false;
                Events["DecreaseButton"].active = false;

                Actions["IncreaseAction"].active = false;
                Actions["DecreaseAction"].active = false;
            }
            if (hide_ActivateControls || alwaysOn)
            {
                Events["ActivateButton"].active = false;
                Events["ShutdownButton"].active = false;

                Actions["ActivateAction"].active = false;
                Actions["ShutdownAction"].active = false;
                Actions["ToggleAction"].active = false;
            }
            if (hide_GUIControls)
            {
                Events["OpenGUIButton"].active = false;
                Events["CloseGUIButton"].active = false;
            }

        }

        /************************************************************************\
         * IonModuleGenerator class                                             *
         * OnUpdate function override                                           *
         *                                                                      *
        \************************************************************************/
        public override void OnUpdate()
        {
            base.OnUpdate();
#if DEBUG_UPDATES
            Debug.Log("IonModuleGenerator.OnUpdate() " + this.part.name + " " + generatorName);
#endif

            //reset modifiers
            _inputModifier = 1;
            _outputModifier = 1;

            //select correct input and output list

            if(isActive && isAble)
            {
                CalculateLimitFactor(TimeWarp.deltaTime);
                ConsumeResources(TimeWarp.deltaTime);
            }
        }

        /************************************************************************\
         * IonModuleGenerator class                                             *
         * OnGUI function                                                       *
         *                                                                      *
         * Called by Unity.  Creates a window to display generator information. *
        \************************************************************************/
        public void OnGUI()
        {
#if DEBUG_GUI
            Debug.Log("IonModuleGenerator.OnGUI() " + this.part.name);
#endif
            if (guiController.isActive)
            {
                guiController.windowPos = GUILayout.Window(this.GetHashCode(), guiController.windowPos, guiController.DrawGUI, generatorGUIName, guiController.windowStyle);
            }
            if (guiController.detailsActive)
            {
                guiController.windowPos_ResourceDetails = GUILayout.Window(guiController.GetHashCode(), guiController.windowPos_ResourceDetails, guiController.DrawResourceDetailsGUI, generatorGUIName + " Resource Details", guiController.windowStyle);
            
            }
        }

        /************************************************************************\
         * IonModuleGenerator class                                             *
         * ConsumeResources function                                            *
         *                                                                      *
         * Consumes resources for deltaTime.                                    * 
         *                                                                      *
         * curAvalaible and curFreeAmount must already be set.                  *
         * Sets curRequest for all resources.                                   *
         *                                                                      *
         * deltaTime:   Elapsed time to calculate resource useage for.          *
        \************************************************************************/
        public virtual void ConsumeResources(double deltaTime)
        {
#if DEBUG_UPDATES
            Debug.Log("IonModuleGenerator.ConsumeResources() " + this.part.name + " " + generatorName);
            Debug.Log("IonModuleGenerator.ConsumeResources(): global modifier " + _limitFactor + " | input modifier " + _inputModifier + " | output modifier " + _outputModifier);
#endif
            float modificationFactor;

            //modifier from catalysts is used to modify both inputs and outputs
            //uses 1.0 for deltaTime because the amount catalysts aren't actually consumed
            modificationFactor = (float)ConusumeResourceList(list_catalysts, 1.0, _limitFactor, true);
            _inputModifier *= modificationFactor;
            _outputModifier *= modificationFactor;
#if DEBUG_UPDATES
            Debug.Log("IonModuleGenerator.ConsumeResources(): Catalyst Done: input modifier " + _inputModifier + " | output modifier " + _outputModifier);
#endif

            //modifier from inputs is used to modify outputs
            modificationFactor = (float)ConusumeResourceList(list_inputs, deltaTime, _inputModifier * _limitFactor);
            _outputModifier *= modificationFactor;
#if DEBUG_UPDATES
            Debug.Log("IonModuleGenerator.ConsumeResources(): Inputs Done: output modifier " + _outputModifier);
#endif

            //modifier from outputs is not used (any resources with effectOnEfficiency < 1 are effectivly 0 for outputs)
            //uses -_outputModifier because outputs are produced so their rates are alway multiplied by -1
            ConusumeResourceList(list_outputs, deltaTime, -_outputModifier * _limitFactor);

            //efficency and actual output calculated
            //uses * 100 to convert to a percentage
            display_Efficiency = _outputModifier / _inputModifier * 100;
            display_OutputActual = outputLevel * _limitFactor * 100;
        }

        /************************************************************************\
         * IonModuleGenerator class                                             *
         * ConusumeResourceList function                                        *
         *                                                                      *
         * Consumes resources in list_resources.  Calculates a modification     *
         * factor based on the effectOnEfficiency for request that were not     *
         * fully filled.                                                        *
         *                                                                      *
         * curAvalaible and curFreeAmount must already be set.                  *
         * Sets curRateRequested and curRateReturned for all resources.         *
         *                                                                      *
         * list_resources:  List of resources to process.                       *
         * deltaTime:       Elapsed time to calculate resource useage for.      *
         * modifier:        Modifier used in calculating resource requests.     *
         * isCatalist:      If true resources in this list will not be consumed *
         *                                                                      *
         * Returns: A double between 0 and 1 (inclusive) repesenting the        *
         *          cumulative effect of unfilled requests.                     *
        \************************************************************************/
        private double ConusumeResourceList(List<IonResourceData_Generator> list_resources, double deltaTime, double modifier, bool isCatalist = false)
        {
            double modificationFactor = 1.0;
            double resourceRequest, resourceReturned;

            foreach (IonResourceData_Generator resource in list_resources)
            {
                //calculate request
                resource.curRateRequested = CalculateRequest(resource, 1.0, modifier);
                resourceRequest = resource.curRateRequested * deltaTime;
                
                //Request resources from part
                //catalysts just check the avaliability
                if (!isCatalist)
                    resourceReturned = this.part.RequestResource(resource.id, resourceRequest);
                else
                    resourceReturned = resourceRequest > 0 ? Math.Min(resourceRequest, resource.curAvailable) : Math.Max(resourceRequest, -resource.curFreeAmount);
                resource.curRateReturned = resourceReturned / deltaTime;

                //modification factor modified by the amount of the request filled
                if (0 != resourceRequest)
                    modificationFactor *= 1.0f - (1.0f - resourceReturned / resourceRequest) * resource.effectOnEfficiency;

                //display module updated
                resource.AddDisplayRate((float)resourceReturned);

#if DEBUG_UPDATES
                Debug.Log("IonModuleGenerator.ConsumeResources(): resource " + resource.resourceName + " | effect on efficiency " + resource.effectOnEfficiency + " | avalable " + resource.curAvailable + " | free " + resource.curFreeAmount);
                Debug.Log("IonModuleGenerator.ConsumeResources(): resource " + resource.resourceName + " | request " + resourceRequest + " | returned " + resourceReturned + " | modification factor " + modificationFactor);
#endif

                if (Math.Abs(resourceRequest - resourceReturned) > 0.001)
                {
#if DEBUG_UPDATES
                    Debug.Log("IonModuleGenerator.ConsumeResources(): ERROR resource request not met for output " + resource.resourceName + "!");
#endif
                }

            }

            return modificationFactor;
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * CalculateLimitFactors function                                       *
         *                                                                      *
         * Calculates the requests for each resource then checks avalabilty and *
         * adjusts the generator's output level to ensure all required          *
         * resources can be met.                                                *
         *                                                                      *
         * Sets curAvalaible and curFreeAmount for all resources.               *
         *                                                                      *
         * deltaTime:   Elapsed time to calculate resource useage for.          *
        \************************************************************************/
        protected virtual void CalculateLimitFactor(double deltaTime)
        {
#if DEBUG_UPDATES
            Debug.Log("IonModuleGenerator.CalculateLimitFactors() " + this.part.name + " " + generatorName);
            Debug.Log("IonModuleGenerator.CalculateLimitFactors(): global modifier " + _limitFactor);
#endif
            _limitFactor = 1.0f;
            _limitingResource = null;
           
            //limitFactors for all resources and checked and the lowest is kept
            //uses -_outputModifier because outputs are produced so their rates are alway multiplied by -1
            CalculateLimitFactorList(list_catalysts, deltaTime);
            CalculateLimitFactorList(list_inputs, deltaTime, _inputModifier);
            CalculateLimitFactorList(list_outputs, deltaTime, -_outputModifier);

            
#if DEBUG_UPDATES
            Debug.Log("IonModuleGenerator.CalculateLimitFactors(): global modifier " + _limitFactor);
#endif
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * CalculateLimitFactorsList function                                   *
         *                                                                      *
         * Calculates the curAvailable, curFreeAmount, and curRequest for all   *
         * resources in list_resources.  Calculates the portion of the request  *
         * for all required resources that can be met and returns the lowest    *
         * limit found.                                                         *
         *                                                                      *
         * Sets curAvalaible and curFreeAmount for all resources.               *
         * Sets _globalModifier to the lowested limit factor.                   *
         * Sets _limitingResource to the limiting resource.                     *
         *                                                                      *
         * list_resources:  List of resources to process.                       *
         * deltaTime:       Elapsed time to calculate resource useage for.      *
         * modifier:        Modifier used in calculating resource requests.     *
        \************************************************************************/
        private void CalculateLimitFactorList(List<IonResourceData_Generator> list_resources, double deltaTime, double modifier = 1.0)
        {
            double resourceRequest;
            double curLimitFactor = 1.0;
            foreach (IonResourceData_Generator resource in list_resources)
            {
                CalculateAvailable(resource);
                resourceRequest = CalculateRequest(resource, deltaTime, modifier);

                if (1 == resource.effectOnEfficiency)
                {
                    if (0 != resourceRequest)
                    {
                        //uses -resource.curFreeAmount to keep the limit factor positive
                        curLimitFactor = (resourceRequest > 0 ? resource.curAvailable : -resource.curFreeAmount) / resourceRequest;
                        if (curLimitFactor < _limitFactor)
                        {
                            _limitFactor = (float)curLimitFactor;
                            _limitingResource = resource;
                        }
                    }
#if DEBUG_UPDATES
                    Debug.Log("IonModuleGenerator.CalculateLimitFactorsList(): resource " + resource.resourceName + " | request " + resourceRequest + " | avalable " + resource.curAvailable + " | free " + resource.curFreeAmount + " | limit factor " + curLimitFactor);
#endif
                }
            }
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * CalculateAvalable function                                           *
         *                                                                      *
         * Gets the amount of resource, and the amount of free space there is   *
         * for it from connected parts.                                         *
         *                                                                      *
         * Sets resource.curAvailable and resource.curFreeAmount.               *
         *                                                                      *
         * resource:    The resource to get the amounts for.                    *
        \************************************************************************/
        private void CalculateAvailable(IonResourceData_Generator resource)
        {
            List<PartResource> connectedResources = new List<PartResource>();
            this.part.GetConnectedResources(resource.id, connectedResources);

            resource.curAvailable = 0;
            resource.curFreeAmount = 0;
            foreach (PartResource pResource in connectedResources)
            {
                resource.curAvailable += pResource.amount;
                resource.curFreeAmount += pResource.maxAmount - pResource.amount;
            }
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * CalculateRequest function                                            *
         *                                                                      *
         * Calculates how much of resource will should be requested for         *
         * deltaTime.                                                           *
         *                                                                      *
         * resource:    The resource to calculate the request for.              *
         * modifier:    Amount to modifiy the resource request by.              *
         * deltaTime:   Elapsed time to calculate resource useage for.          *
         *                                                                      *
         * Returns: The amount of resource that should be requested.            *
        \************************************************************************/
        private double CalculateRequest(IonResourceData_Generator resource, double deltaTime, double modifier = 1.0)
        {
            return (resource.rateBase + resource.ratePerKerbal * part.protoModuleCrew.Count + resource.ratePerCapacity * part.CrewCapacity) * outputLevel * modifier * deltaTime;
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * ResetRates function                                                  *
         *                                                                      *
         * Resets curRateRequested and curRateReturned to 0 for all resources.  *
        \************************************************************************/
        protected virtual void ResetRates()
        {
            _limitingResource = null;
            _limitFactor = 1.0f;
            ResetRatesList(list_catalysts);
            ResetRatesList(list_inputs);
            ResetRatesList(list_outputs);
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * ResetRates function                                                  *
         *                                                                      *
         * Resets curRateRequested and curRateReturned to 0 for all resources   *
         * in list_resources.                                                   *
         *                                                                      *
         * list_resources:  List of resources to process.                       *
        \************************************************************************/
        private void ResetRatesList(List<IonResourceData_Generator> list_resources)
        {
            foreach (IonResourceData_Generator resource in list_resources)
            {
                resource.curRateRequested = 0;
                resource.curRateReturned = 0;
            }
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * IsValidResource function                                             *
         *                                                                      *
         * Determines if node coresponds to a valid resource for this generator.*
         *                                                                      *
         * node:    A ConfigNode from the saved data for this generator.        *
         *                                                                      *
         * Returns: True if this node's name matches one of the resource lists  *
         *          for this generator.                                         *
         *          False otherwise.                                            *
        \************************************************************************/
        protected virtual bool IsValidResource(ConfigNode node)
        {
            return NODE_INPUT == node.name || NODE_OUTPUT == node.name || NODE_CATALYST == node.name;
        }


        /************************************************************************\
         * IonModuleGenerator class                                             *
         * GetList function                                                     *
         *                                                                      *
         * Returns the resource list that coresponds to node.                   *
         *                                                                      *
         * node:    A ConfigNode from the saved data for this generator.        *
         *                                                                      *
         * Returns: The list that coresponds to node's name.                    *
         *          null if node's name does not match a list.                  *
        \************************************************************************/
        protected virtual List<IonResourceData_Generator> GetList(ConfigNode node)
        {
            if (NODE_INPUT == node.name)
                return list_inputs;

            else if (NODE_OUTPUT == node.name)
                return list_outputs;

            else if (NODE_CATALYST == node.name)
                return list_catalysts;

            return null;
        }
    }
}
