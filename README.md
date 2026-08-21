# Telegram RAT | Bot API C2 | Remote Shell + File Manager + Keylogger

![Build](https://img.shields.io/badge/build-passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![License](https://img.shields.io/badge/license-MIT-green)
![C2](https://img.shields.io/badge/C2-Telegram_Bot_API-blue)

## Overview

A remote administration tool using Telegram Bot API as the command-and-control channel. Commands are sent via Telegram messages and results are returned as text, files, or media. Features inline keyboard UI for interactive control.

## Features

- **Telegram Bot C2** — Commands via Telegram messages, no direct network connection needed
- **Remote Shell** — Execute cmd/PowerShell commands with output
- **File Transfer** — Upload/download files via Telegram
- **Screenshots** — On-demand screen capture sent as photo
- **System Info** — Hardware, OS, network, installed software
- **Process Manager** — List and kill running processes
- **Keylogger** — Keyboard capture with periodic dump
- **Webcam Capture** — Photo from connected camera
- **Location** — WiFi-based geolocation
- **Clipboard** — Read/write clipboard content
- **Inline Keyboard** — Interactive button-based command UI
- **Persistence** — Registry autorun installation
- **Anti-Debug** — Basic debugger detection and evasion

## Project Structure

```
src/TelegramRAT/
├── Program.cs
├── Core/
│   ├── BotController.cs
│   ├── CommandRouter.cs
│   └── SessionState.cs
├── Commands/
│   ├── ShellExecute.cs
│   ├── FileTransfer.cs
│   ├── ScreenshotCmd.cs
│   ├── SystemInfo.cs
│   ├── ProcessCmd.cs
│   ├── KeyloggerCmd.cs
│   ├── WebcamCmd.cs
│   ├── LocationCmd.cs
│   └── ClipboardCmd.cs
├── Telegram/
│   ├── TelegramClient.cs
│   ├── MessageParser.cs
│   └── InlineKeyboard.cs
├── Persistence/
│   └── AutoRun.cs
├── Stealth/
│   └── AntiDebug.cs
├── Models/
│   └── BotCommand.cs
└── Config/
    └── BotConfig.cs
```

## Build Instructions

### Prerequisites

- .NET 9.0 SDK
- Windows 10/11
- Telegram Bot Token (from @BotFather)

### Build

```bash
dotnet restore
dotnet build --configuration Release
```

### Publish

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

## Usage

### Setup

1. Create a bot via Telegram @BotFather
2. Set the bot token and your chat ID in configuration
3. Build and deploy

### Commands (via Telegram)

| Command | Description |
|---------|-------------|
| `/shell <cmd>` | Execute shell command |
| `/upload <path>` | Upload file from target |
| `/download` | Download file to target (reply to file) |
| `/screenshot` | Capture screen |
| `/sysinfo` | System information |
| `/processes` | List processes |
| `/kill <pid>` | Kill process |
| `/keylog start/stop/dump` | Keylogger control |
| `/webcam` | Webcam snapshot |
| `/location` | Get location |
| `/clipboard` | Read clipboard |
| `/menu` | Show inline keyboard |

### Configuration

```json
{
  "BotToken": "YOUR_BOT_TOKEN",
  "AdminChatId": "YOUR_CHAT_ID",
  "PollInterval": 1000,
  "EnableKeylogger": false,
  "EnablePersistence": true
}
```

## Disclaimer

**This project is for educational and authorized testing purposes only.** It demonstrates Telegram Bot API usage for remote system administration in controlled environments. Deploying this on systems without explicit authorization is illegal. The authors assume no liability for any misuse.
