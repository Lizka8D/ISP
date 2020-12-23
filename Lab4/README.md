# Структура программы
Проект содержит следующие:

> Папка DataOptions содержит:
- класс DataOptions
- файл dataOptions.json
- файл dataOptions.xml
- файл dataOptions.xsd

> Папка ExamplesOfWork содержит:
- файл JsonFile.json
- файл XMLFile.xml

> Папка FileWatcher содержит:
- все классы и файлы из Lab3

> Папка ServiceLib содержит:
  - папку Parser, которая содержит:
    - класс ConfigurationManager
    - интерфейс IParser
    - класс ParserJson
    - класс ParserXML
- класс DataIO
- класс FileTransfer
- класс XmlGenerator

Также проект содержит классы:
- Program
- Service1
- Service1.Designer
- Installer1
- Installer1.Designer

# Описание программы
Данная лабораторная работа представляет собой две службы работающие параллельно. Первая служба - служба FileWatcher, разработанная в лаборатрной работе №3. Вторая служба DataManager. Ее разработка и была задачей данной лабораторной. При разработке данной службы использовалась база данных AdwantureWorks. Давайте подробнее рассмотрим алгоритм работы.
