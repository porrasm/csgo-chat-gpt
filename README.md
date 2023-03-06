# CS:GO ChatGPT integration
ChatGPT integrated into CS:GO chat. I was bored one day and decided to make this.

[Youtube demonstration!](https://youtu.be/YURCXX9IC3E)

## What does it do?
It allows you to talk to the AI in CS:GO chat. The AI will respond to your messages in all chat. You can specify your username in the config file so the AI won't respond to your messages.

You can make the AI trashtalk, roleplay a character or respond only in rhymes etc. See the `AIInstruction` configuration option.

Example conversation:

```
AIInstruction: "You are roleplaying an angry gamer playing CS:GO."

player: ez game
ai    : What the hell? How is this an easy game? My teammates suck and the enemy team is all camping. Come on guys, step it up! Don't just stand there and let them kill 
player: learn to play noob
ai    : What did you just say? Learn to play? You think I'm not playing well? I'm carrying this damn team! You're the one sitting at the bottom with no kills.
```

## How to use

1. Download the latest release from the [releases page](https://github.com/porrasm/csgo-chat-gpt/releases)
2. Extract the zip file
3. Configure the telnet port in the games launch options with the `netconport` parameter. Example: `netconport 12350`
4. Get your ChatGPT API key from [here](https://platform.openai.com/account/api-keys)
5. Run the program
6. Type 'config' in the console (restart the program after saving the file)
7. Configure the bot
8. Type 'start' in the console

## How to configure

The bot offers the following configuration options:

- PersonalNickname: Your current Steam nickname. Needed so the AI won't respond to your messages and get stuck in an infinite loop talking to itself.
- APIKey: Your ChatGPT API key. Get one here: https://platform.openai.com/account/api-keys
- ResponseMaxTokens: The length of the AI responses. 1 token ~ 4 characters.
- RespondToChatAutomatically: Whether the AI should respond to chat messages automatically.
- RespondToAnyConsoleMessageWithSplitter: Whether the AI should respond to any console message that contains the splitter. Needed for other games than CS:GO or for some community servers. Example: chat message starts with 'ai: ', the bot will respond.
- ConsoleMessageSplitter: The splitter that the AI will use to respond to console messages.
- TelnetPort: The port you set in the games launch options with the '-netconport' parameter.
- MessageHistoryLength: How many messages the AI should remember. The more messages, the more accurate the AI will be. But it will also take longer to respond.
- AIInstruction: The AI instruction. You can experiment with this. Example: 'You are roleplaying as a very angry gamer.' or 'You are a helpful assistant.'

The most interesting is perhaps the AIInstruction. You can define the type of AI you want to talk to. For example, you can make the AI a very angry gamer or a helpful assistant.

## Can this get me VAC banned?

No. The bot doesn't modify any game files or inject any code into the game. It only uses the telnet connection to communicate with the game which is a non-cheat feature implemented in almost all Valve games.

I would not use this on Faceit or ESEA servers though. I don't know if they allow telnet connections to the game console.

## Will this work with other games?

This will work with most Valve games. However the direct chat response only works in CS:GO. For other games you can use the console message splitter to make the AI respond to any console message that contains the splitter. Example: chat message starts with 'ai: ', the bot will respond.

## Donate if you really enjoy this!

Paypal: https://paypal.me/porrasm
