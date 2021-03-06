using ServiceLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public partial class Service1 : ServiceBase
    {
        readonly DataIO appInsights;
        readonly DataOptions dataOptions;

        public Service1(DataOptions dataOptions, DataIO appInsights)
        {
            InitializeComponent();
            this.dataOptions = dataOptions;
            this.appInsights = appInsights;
        }

        protected override void OnStart(string[] args)
        {
            DataIO reader = new DataIO(dataOptions.ConnectionString);
            FileTransfer fileTransfer = new FileTransfer(dataOptions.TargetFolder, dataOptions.SourcePath);

            string customersFileName = "person";

            await reader.GetCustomers(dataOptions.TargetFolder, appInsights, customersFileName);

            await fileTransfer.SendFileToFtp($"{customersFileName}.xml");
            await fileTransfer.SendFileToFtp($"{customersFileName}.xsd");

            await appInsights.InsertInsight("Files were sent to FTP successfully");
        }

        protected override void OnStop()
        {
            await appInsights.InsertInsightAsync("Stop");
            await appInsights.WriteInsightsToXmlAsync(dataOptions.OutputFolder);
        }
    }
}
