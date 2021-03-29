## TelepuzikiChat


### Идея:
Основная цель нашего проекта - чат для общения людей.


### Основные компоненты системы:

**1. Client Application**

**2. Server Application (Console)**

**3. NetworkingLibrary**

**4. CryptingLibrary**

**5. DataBase**


### Точки расширения архитектуры:
**1. Networking:**
В архитектуре клиент-серверной части есть возможность расширения в плане 
новых способов передачи данных между клиентами и сервером, т.е. есть возможность 
добавления новых классов, наследуемых от `IPackage` и `IMessage`, где `IPackage`
является абстракцией для передачи потока байт по сети, а `IMessage` асбтракция для
вышестоящего уровня.

**2. Crypting:**

**3. DataBase:**


### Участники:
**1. Иваненко Григорий** [КН-204]

**2. Клабукова Юлия** [КН-204]

**3. Тиунов Владимир** [КН-204]

**4. Казанцев Артём** [КН-204]