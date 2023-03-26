# twitch-shopping-network-logger

A simple Twitch bot which logs the whisper history between the bot and any users which whisper to it.

This is useful for managing chats when many users chat with you at once. The Twitch UI makes it difficult to respond to chat messages in an orderly fashion
if many users are whispering you at the same time, as it's impossible to tell when users messaged you, and the webpage sometimes crashes if you receive too
many whispers.

The intended uscase is when streaming an online shopping channel. Viewers can whisper a bot and the bot will automatically respond and log messages. This allows you
and your crew to respond to users in the order they reached out to you as you find the time to do so.

## Features

* Logs user whispers to an Excel spreadsheet. Each user will have its own row, and each new message will be added to a column.
* Responds to whispers with an automatic message the first time a user messages the bot

## Deployment and Configuration

Three ways are provided to run the bot:

1. As a command-line utility to be run locally
2. As a WinForms GUI to be run locally
3. As a REST WebAPI to be hosted

Regardless of the method, you'll need to provide a `Config.json` in the directory the application is running.

The config should have the following format:

```json
{
    "TwitchClientKey": String, // The access token to enable the bot to interact with Twitch (see "Generating an Access Token" below)
    "AuthorizedUsers": [String], // The list of users authorized to log messages
    "AutoRespondEnabled": Boolean,  // If true, will respond with an auto-generated message the first time a user whispers the bot
    "FirstWhisperResponse": String, // The message to respond with the first time if AutoRespondEnabled is true
    "ExcelDirectory": String", // The directory to save the Excel file in
}
```

### Generating an Access Token

You can generate an access token here: https://twitchtokengenerator.com/

You will need the following roles:
* chat:read
* chat:edit
* whisper:read
* whisper:edit
