# TelegramBotMinecraft

![GitHub release](https://img.shields.io/github/v/release/TRONMAXS/TelegramBotMinecraft?include_prereleases&label=release)
![GitHub repo size](https://img.shields.io/github/repo-size/TRONMAXS/TelegramBotMinecraft)
![GitHub issues](https://img.shields.io/github/issues/TRONMAXS/TelegramBotMinecraft)


**Telegram-бот для удалённого управления Minecraft-серверами через Telegram с удобной графической панелью.**  
Позволяет включать, выключать, проверять статус серверов и выполнять команды через RCON. Начиная с версии `v1.0.0-alpha.1`, проект полностью переписан с Windows Forms на современный кроссплатформенный движок **Avalonia UI** на базе **.NET 10** и паттерна **MVVM**.

---

## Технологический стек

* **Платформа:** .NET 10 (C#)
* **UI-Движок:** Avalonia UI (v12.1.0)
* **Стилизация:** Semi.Avalonia
* **Архитектура UI:** CommunityToolkit.Mvvm (MVVM)
* **База данных:** SQLite

---

## Быстрый старт

### Требования
* Операционная система Windows 10 / 11 (x64).
* Токен Telegram-бота (можно получить у [@BotFather](https://t.me/@BotFather).

### Запуск готовой сборки
1. Перейдите в раздел [Releases](https://github.com/TRONMAXS/TelegramBotMinecraft/releases) и скачайте актуальную версию.
2. Распакуйте архив в отдельную папку.
3. Запустите файл `TelegramBotMinecraft.exe`.

---

## Сборка из исходников

Если вы хотите скомпилировать проект самостоятельно, клонируйте репозиторий и выполните команду публикации в терминале папки `TelegramBotMinecraft.Avalonia`:

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishReadyToRun=true
```

После успешной сборки готовая папка с исполняемым файлом и нативными библиотеками появится по пути: `bin/Release/net10.0/win-x64/publish/`.

---

## 📄 Лицензия

Проект разрабатывается как Open-Source решение. Вы можете свободно использовать и модифицировать код под свои нужды.
