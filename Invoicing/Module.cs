using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using Invoicing.Module.BusinessObjects;

namespace Invoicing {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    public sealed partial class InvoicingModule : ModuleBase {
        public InvoicingModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);

            ITypeInfo InvoiceTypeInfo = typesInfo.FindTypeInfo(typeof(Invoice));
            ITypeInfo CustomerTypeInfo = typesInfo.FindTypeInfo("Customers.Module.BusinessObjects.Customer");
            IMemberInfo CustomerPropertyInfo = InvoiceTypeInfo.FindMember("Customer");
            IMemberInfo InvoicesPropertyInfo = CustomerTypeInfo.FindMember("Invoices");
            if (CustomerPropertyInfo == null)
            {
                CustomerPropertyInfo = InvoiceTypeInfo.CreateMember("Customer", CustomerTypeInfo.Type);
                CustomerPropertyInfo.AddAttribute(new DevExpress.Xpo.AssociationAttribute("A", CustomerTypeInfo.Type), true);
            }
            if (InvoicesPropertyInfo == null)
            {
                Type elementType = InvoiceTypeInfo.Type;

                // Get the generic type definition for List<>
                Type listType = typeof(XPCollection<>);

                // Use MakeGenericType to create the specific type List<string>
                Type constructedListType = listType.MakeGenericType(elementType);

                InvoicesPropertyInfo = CustomerTypeInfo.CreateMember("Invoices", constructedListType);
                InvoicesPropertyInfo.AddAttribute(new DevExpress.Xpo.AssociationAttribute("A", InvoiceTypeInfo.Type), true);
                //InvoicesPropertyInfo.AddAttribute(new DevExpress.Xpo.AggregatedAttribute(), true);
            }
            ((XafMemberInfo)CustomerPropertyInfo).Refresh();
            ((XafMemberInfo)InvoicesPropertyInfo).Refresh();



        }
    }
}
