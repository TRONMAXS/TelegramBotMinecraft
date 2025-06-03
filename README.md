# Telegram Bot Minecraft v0.0.1
![GitHub release](https://img.shields.io/github/v/release/TRONMAXS/TelegramBotMinecraft?include_prereleases&label=release)
![GitHub repo size](https://img.shields.io/github/repo-size/TRONMAXS/TelegramBotMinecraft)
![GitHub issues](https://img.shields.io/github/issues/TRONMAXS/TelegramBotMinecraft)

**Telegram-бот для удалённого управления Minecraft-серверами через Telegram.**
Позволяет включать, выключать, проверять статус серверов и выполнять команды через RCON.

## Возможности

* Управление неограниченным числом Minecraft-серверов
* Включение/выключение серверов
* Проверка статуса серверов и количества игроков
* Выполнение команд на сервере через Telegram
* Поддержка GUI на Windows (Windows Forms)
* Поддержка любых версий Minecraft-серверов (через RCON)

## Используемые технологии

* Язык: **C#**
* UI: **Windows Forms**
* Библиотеки: [`Telegram.Bot`](https://github.com/TelegramBots/Telegram.Bot), [`CoreRCON`](https://github.com/dparparyan/CoreRCON)

## Команды Telegram-бота

| Команда                                  | Описание                                                                |
| ---------------------------------------- | ----------------------------------------------------------------------- |
| `/servers_list`                          | Показывает список доступных серверов                                    |
| `/server_enable <пароль> <номер>`        | Включает указанный сервер (по номеру в списке) — **только для админов** |
| `/bot_server_start`                      | Запускает выбранный сервер                                              |
| `/bot_servers_check`                     | Проверяет статус всех серверов                                          |
| `/bot_server_list`                       | Показывает количество игроков на выбранном сервере                      |
| `/bot_server_stop`                       | Останавливает выбранный сервер                                          |
| `/bot_server_command <пароль> <команда>` | Выполняет команду на выбранном сервере — **только для админов**         |
| `/bot_world_delete`                      | Удаляет мир на выбранном сервере                                        |
| `/help`                                  | Отображает список доступных команд                                      |

## Конфигурация

Бот использует два конфигурационных файла:

### `Settings.json`

```json
[
  {
    "Notifications": true,
    "BotToken": "123456789:ABCdefGHIjklMNOpqrSTUvwxYZ",
    "ChatIds": [
      {
        "Identifier": "646516246",
        "Name": "Admin"
      }
    ]
  }
]
```

### `Servers.json`

```json
[
  {
    "Name": "Vanilla(Survival) - 1.20.1",
    "Path": "G:\\MinecraftServers\\Vanilla(Survival) - 1.20.1",
    "Ip": "127.0.0.1",
    "RconPort": "25565",
    "RconPassword": "12345",
    "ConnectIp": "127.0.0.1",
    "Port": "25565",
    "Enabled": true
  }
]
```

## Установка и запуск

*(Инструкция появится позже)*

## Дополнительно

* Ограничений на количество серверов — **нет**
* Права администратора требуются только для двух команд: `/server_enable`, `/bot_server_command`
* **Поддержка Linux-серверов не тестировалась**
* GUI доступен (Windows Forms интерфейс)

---

## 👤 Обо мне

Привет! Меня зовут Максим — я изучаю C# и разработку игр на Unity.  
Этот проект — моя практика в разработке приложений на Windows Forms, создании Telegram-ботов и работе с Minecraft-серверами через RCON.

Если хочешь связаться — пиши на: popik.maxim@gmail.com  
[Мои проекты на GitHub](https://github.com/TRONMAXS)
