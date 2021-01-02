# Структура программы
Проект содержит следующие:

> Папка DataManager содержит:
- все классы и файлы из Lab4

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

# Описание программы
В данной лабораторной работе был проведен рефакторинг кода для добавления асинхоенного поведения. Используются два сервиса, созданные в ранних лабораторных работах, FileWatcher(Lab 3) и DataManager(Lab 4). Для выполнения условия был переработан код в следующих файлах:
- ServiceLib\DataIO.cs
- ServiceLib\FileTrancser.cs
- ServiceLib\XmlGenerator.cs

- FileWatcher\Service1.cs
- FileWatcher\Logger.cs
- FileWatcher\Program.cs

- DataManager\Service1.cs
- DataManager\Program.cs
