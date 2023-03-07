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

```
# CSGOChatGPT config file

# Whether the bot should connect to the game at startup.
ConnectAtStartup=False

# Your current Steam nickname. Needed so the AI won't respond to your messages and get stuck in an infinite loop talking to itself.
PersonalNickname=your_current_nickname

# Your ChatGPT API key. Get one here: https://platform.openai.com/account/api-keys
APIKey=your_chatgpt_api_key

# The port you set in the games launch options with the '-netconport' parameter.
TelnetPort=12350

# The maximum length of a chat message in the game. If a message is longer than this, the messages will be split into multiple chunks. Your username length will affect this.
GameChatMessageMaxLength=100

# Whether the AI should respond to chat messages automatically.
RespondToChatAutomatically=True

# The string that separates the nickname from the message. Notice the hidden character in the default value: '[U+200E] : '.
RespondToChatSplitter=‎ : 

# The string that appears in fron of your name when you're dead.
RespondToChatDeadSplitter=*DEAD*

# Whether the AI should respond to any console message that contains the splitter. Needed for other games than CS:GO or for some community servers. Example: chat message starts with 'ai: ', the bot will respond.
RespondToAnyConsoleMessage=True

# The splitter that the AI will use to respond to console messages.
RespondToAnyConsoleMessageSplitter= ai: 

# Whether the AI should respond to using the game chat. Set to false to receive AI responses only in the game console. Additionally set the automatic responses from players to false and you can use the console as a personal ChatGPT interface without interference from other players.
RespondUsingGameChat=True

# The AI instruction. You can experiment with this. Example: 'You are roleplaying as a very angry gamer.' or 'You are a helpful assistant.'
AIInstruction=You are a helpful assistant.

# How many messages the AI should remember. The more messages, the more accurate the AI will be. But it will also take longer to respond.
MessageHistoryLength=6

# Whether the AI should ignore messages that it sent itself. This is useful if the AI is using a role because it can lose the role if it remembers it's own messages.
IgnoreAIMessagesInHistory=False

# The length of the AI responses. 1 token ~ 4 characters.
ResponseMaxTokens=0

```

### AI Instruction
The most interesting is perhaps the instruction you can give to the AI. You can define the type of AI you want to talk to. For example, you can make the AI a very angry gamer or a helpful assistant.

Try to describe the wanted behaviour of the bot here. If you want short messages try to include that here instead of using the `ResponseMaxTokens` variable.

It's pretty hard to find interesting values for this. Few examples that seemed to work pretty well:

```
You are a helpful assistant who responds with a few sentences.
You are a chat bost who responds with funny jokes using a few sentences.
You are roleplaying a CS:GO player. You respond with a few sentences.
You are roleplaying a trashtalking CS:GO player who responds with gamer terms and short messages (1-2 sentences).
You are roleplaying Yoda from Star Wars. You respond with one or two sentences.
```

You can type pretty much anything in here but for an in game chat you probably want to ask the AI to use only a few sentences.


## Can this get me VAC banned?

No. The bot doesn't modify any game files or inject any code into the game. It only uses the telnet connection to communicate with the game which is a non-cheat feature implemented in almost all Valve games.

I would not use this on Faceit or ESEA servers though. I don't know if they allow telnet connections to the game console.

## Will this work with other games?

This will work with most Valve games. However the direct chat response only works in CS:GO. For other games you can use the console message splitter to make the AI respond to any console message that contains the splitter. Example: chat message starts with 'ai: ', the bot will respond.

## Donate if you really enjoy this!

Paypal: https://paypal.me/porrasm

# Example use cases

## English-Spanish translator

### Usage

- Send message in Spanish to chat `echo chatbot_chat My English message`
- Translate receiveed message into console `echo chatbot_chat_private La respuesta española`

### Config file

```
RespondToChatAutomatically=False
RespondToAnyConsoleMessage=False
RespondUsingGameChat=True
AIInstruction=You are an English-Spanish translator.
MessageHistoryLength=1
ResponseMaxTokens=0
```
