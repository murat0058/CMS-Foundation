﻿using System;
using System.Xml.Linq;
using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Functions;


namespace Composite.C1Console.Trees
{
    /// <summary>    
    /// </summary>
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public sealed class ConfirmActionNode : ActionNode
    {
        /// <exclude />
        public string ConfirmTitle { get; internal set; }                           // Requried

        /// <exclude />
        public string ConfirmMessage { get; internal set; }                         // Requried

        /// <exclude />
        public XElement FunctionMarkup { get; internal set; }                       // Requried

        /// <exclude />
        public bool RefreshTree { get; internal set; }                              // Optional


        // Cached values
        /// <exclude />
        public DynamicValuesHelper ConfirmTitleDynamicValuesHelper { get; set; }

        /// <exclude />
        public DynamicValuesHelper ConfirmMessageDynamicValuesHelper { get; set; }

        /// <exclude />
        public AttributeDynamicValuesHelper FunctionMarkupDynamicValuesHelper { get; private set; }


        /// <exclude />
        protected override void OnAddAction(Action<ElementAction> actionAdder, EntityToken entityToken, TreeNodeDynamicContext dynamicContext, DynamicValuesHelperReplaceContext dynamicValuesHelperReplaceContext)
        {
            WorkflowActionToken actionToken = new WorkflowActionToken(
                WorkflowFacade.GetWorkflowType("Composite.C1Console.Trees.Workflows.ConfirmActionWorkflow"),
                this.PermissionTypes)
            {
                Payload = this.Serialize(),
                ExtraPayload = PiggybagSerializer.Serialize(dynamicContext.Piggybag.PreparePiggybag(this.OwnerNode, dynamicContext.CurrentEntityToken)),
                DoIgnoreEntityTokenLocking = true
            };


            actionAdder(new ElementAction(new ActionHandle(actionToken))
            {
                VisualData = CreateActionVisualizedData(dynamicValuesHelperReplaceContext)
            });
        }


        /// <exclude />
        protected override void OnInitialize()
        {
            try
            {
                FunctionTreeBuilder.Build(this.FunctionMarkup);
            }
            catch
            {
                AddValidationError("TreeValidationError.Common.WrongFunctionMarkup");
                return;
            }


            this.FunctionMarkupDynamicValuesHelper = new AttributeDynamicValuesHelper(this.FunctionMarkup);
            this.FunctionMarkupDynamicValuesHelper.Initialize(this.OwnerNode);

            this.ConfirmTitleDynamicValuesHelper = new DynamicValuesHelper(this.ConfirmTitle);
            this.ConfirmTitleDynamicValuesHelper.Initialize(this.OwnerNode);

            this.ConfirmMessageDynamicValuesHelper = new DynamicValuesHelper(this.ConfirmMessage);
            this.ConfirmMessageDynamicValuesHelper.Initialize(this.OwnerNode);
        }
    }
}
