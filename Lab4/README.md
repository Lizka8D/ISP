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
Данная лабораторная работа представляет собой две службы работающие параллельно. Первая служба - служба, разработанная в лаборатрной работе №3. Разработка второй службы была задачей данной лабораторной. При разработке данной службы использовалась база данных AdwantureWorks. 

Для более удобного использования класс ConfigurationManager, а также парсеры XML и JSON были перенесены в отдельное пространство имен ServiceLib.

Работа с базой данных осуществляется с помощью класса DataOptions.

Также для работы с базами данных были разработаны классы DataIo (в данном классе используются хранимые процедуры для работы с базами данных ApplicationInsights и AdwentureWorks. В нем осуществляется добавление, удаление и чтение данных из БД), FileTransfer (с помощью данного класса осуществляется передача XML файла на FTP сервер), а также класс XmlGenerator (генерирует Xml файл на основе полученных данных. Данный процесс происходит с использованием DataSet. Затем создается Xsd файл, который валидирует наш Xml файл)

# Алгоритм работы программы

1. Запускаем службу, разработанную в лабораторной работе №3
2. Запускаем службу, разработанную в данной лабораторной работе
3. Службы получают необходимые настройки из Xml и Json файлов
4. Происходит извлечение данных из базы данных AdwantureWorks
5. На основе этих данных генерируются Xml и Xsd файлы
6. Эти файлы отправляются в папку SourceDirectory
7. С помощью службы, разработанную в лабораторной работе №3, полученные файлы отправляются в TargetDirectory
8. Все действия, в том числе и исключения логируются и записываются в разработанную мной базу данных ApplicationInsights, также дополнительно исключения записываются в файл Exception.txt
9. Завершение работы двух служб
