using System;

namespace Lab3
{
    class ConfigurationManager : IParser
    {
        private readonly IParser parser;
        public ConfigurationManager(string path, Type mainType)
        {
            if (path.EndsWith(".xml"))
            {
                parser = new ParserXML(path, mainType);
            }
            else if (path.EndsWith(".json"))
            {
                parser = new ParserJson(path, mainType);
            }
            else
            {
                throw new ArgumentNullException($"invalid extension");
            }
        }

        public T GetOptions<T>() => parser.GetOptions<T>();
    }
}
© 2021 GitHub, Inc.
