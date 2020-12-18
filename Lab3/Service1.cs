using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Xml.Schema;
using System.Xml.Linq;

namespace Lab3
{
    public partial class Service1 : ServiceBase
    {
        Logger logger;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ConfigurationManager configManager;

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xml")) &&
               File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xsd")))
            {
                XmlSchemaSet schema = new XmlSchemaSet();

                schema.Add(string.Empty, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xsd"));

                XDocument xdoc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xml"));

                xdoc.Validate(schema, ValidationEventHandler);

                configManager = new ConfigurationManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xml"), typeof(Options));
            }
            else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonFile.json")))
            {
                configManager = new ConfigurationManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonFile.json"), typeof(Options));
            }
            else
            {
                throw new ArgumentNullException($"No config was found");
            }

            Options options = configManager.GetOptions<Options>();

            logger = new Logger(options);

            Thread loggerThread = new Thread(new ThreadStart(logger.Start));

            loggerThread.Start();
        }

        protected override void OnStop()
        {
            if (!(logger is null))
            {
                logger.Stop();
            }
        }
        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (Enum.TryParse("Error", out XmlSeverityType type) && type == XmlSeverityType.Error)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
