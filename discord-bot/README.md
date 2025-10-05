# Discord Bot Project

A feature-rich Discord bot built with Discord.js v14, featuring slash commands for testing bot functionality and issuing friendly challenges between users.

## 🎯 Features

### 📋 Commands

#### `/test` - Bot Test Command
Tests if the bot is working correctly and displays bot status information.

**Options:**
- `message` (optional): An optional message to echo back

**Example Usage:**
```
/test
/test message:Hello World!
```

**Features:**
- ✅ Verifies bot is online and responsive
- 👤 Displays user information
- ⏰ Shows timestamp of the request
- 💬 Echoes back optional messages

#### `/challenge` - Challenge Command
Challenge another user to a friendly competition with interactive accept/decline buttons.

**Options:**
- `opponent` (required): The user you want to challenge
- `type` (optional): Type of challenge (Gaming, Trivia, Creative, Fitness, Random)

**Example Usage:**
```
/challenge opponent:@Username
/challenge opponent:@Username type:Gaming
```

**Features:**
- ⚔️ Interactive challenge system with Accept/Decline buttons
- 🎲 Multiple challenge types to choose from
- ⏱️ 60-second timeout for responses
- 🚫 Validation (cannot challenge yourself or bots)
- 📊 Active challenge tracking

## 🚀 Setup Instructions

### Prerequisites
- **Node.js**: Version 16.9.0 or higher ([Download](https://nodejs.org/))
- **npm**: Comes with Node.js
- **Discord Account**: To create and configure the bot

### 1. Create a Discord Bot

1. Go to [Discord Developer Portal](https://discord.com/developers/applications)
2. Click "New Application" and give it a name
3. Navigate to the "Bot" section in the left sidebar
4. Click "Add Bot" and confirm
5. Under the bot's username, click "Reset Token" and copy the token (you'll need this later)
6. Scroll down and enable these **Privileged Gateway Intents**:
   - ✅ Message Content Intent
   - ✅ Server Members Intent (optional)
7. Navigate to "OAuth2" > "URL Generator"
8. Select these scopes:
   - ✅ `bot`
   - ✅ `applications.commands`
9. Select these bot permissions:
   - ✅ Send Messages
   - ✅ Embed Links
   - ✅ Read Message History
   - ✅ Use Slash Commands
10. Copy the generated URL at the bottom and open it in your browser
11. Select a server to add the bot to and authorize

### 2. Configure the Bot

1. Navigate to the `discord-bot` directory:
   ```bash
   cd discord-bot
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create a `.env` file by copying the example:
   ```bash
   cp .env.example .env
   ```

4. Edit the `.env` file and add your credentials:
   ```env
   DISCORD_BOT_TOKEN=your_actual_bot_token_here
   DISCORD_CLIENT_ID=your_application_client_id_here
   ```
   
   - Get `DISCORD_BOT_TOKEN` from the Bot section in Discord Developer Portal
   - Get `DISCORD_CLIENT_ID` from the OAuth2 section (Application ID)

### 3. Run the Bot

Start the bot with:
```bash
npm start
```

For development with auto-restart on file changes (Node.js 18+):
```bash
npm run dev
```

You should see:
```
🔄 Registering slash commands...
✅ Successfully registered slash commands globally
✅ Bot is online as YourBotName#1234
📊 Connected to 1 server(s)
```

## 📚 Project Structure

```
discord-bot/
├── commands/           # Command modules
│   ├── test.js        # Test command implementation
│   └── challenge.js   # Challenge command implementation
├── index.js           # Main bot file
├── package.json       # Node.js dependencies and scripts
├── .env.example       # Example environment variables
├── .env               # Your actual credentials (gitignored)
└── README.md          # This file
```

## 🎮 Usage Guide

### Testing the Bot

1. In any Discord channel where the bot is present, type `/test`
2. The bot will respond with an embed showing:
   - Confirmation that the bot is working
   - Your user information
   - Current timestamp
3. Try with a message: `/test message:Bot is awesome!`

### Issuing Challenges

1. Type `/challenge` and select an opponent user
2. Optionally select a challenge type
3. The opponent will see the challenge with Accept/Decline buttons
4. Only the challenged user can respond to the buttons
5. Challenge expires after 60 seconds if no response

**Challenge Types:**
- 🎮 **Gaming**: Video game competitions
- 🧩 **Trivia**: Knowledge-based challenges
- 🎨 **Creative**: Art, music, or writing challenges
- 💪 **Fitness**: Physical activity challenges
- 🎲 **Random**: Any type of challenge

## 🔧 Development

### Adding New Commands

1. Create a new file in the `commands/` directory (e.g., `mycommand.js`)
2. Export a command object with `data` and `execute` properties:

```javascript
import { SlashCommandBuilder } from 'discord.js';

export default {
  data: new SlashCommandBuilder()
    .setName('mycommand')
    .setDescription('Description of my command'),

  async execute(interaction) {
    await interaction.reply('Hello from my command!');
  },
};
```

3. Import the command in `index.js`:
```javascript
import myCommand from './commands/mycommand.js';
```

4. Add it to the commands array:
```javascript
const commands = [
  testCommand,
  challengeCommand,
  myCommand, // Add your command here
];
```

5. Restart the bot to register the new command

### Debugging

The bot includes comprehensive error handling and logging:
- ✅ Successful operations are logged with green checkmarks
- ❌ Errors are logged with red X marks
- 🔄 Process updates are shown with spinning arrows

Check the console output for detailed information about bot operations.

## 🛡️ Security Best Practices

1. **Never commit your `.env` file** - It contains sensitive credentials
2. **Keep your bot token secret** - Anyone with the token can control your bot
3. **Regenerate your token immediately** if it's exposed
4. **Use environment variables** for all sensitive data
5. **Review bot permissions** regularly and only grant what's necessary

## 📝 Common Issues

### Bot doesn't respond to commands
- ✅ Ensure the bot is online (check console for "Bot is online" message)
- ✅ Verify bot has proper permissions in the Discord server
- ✅ Make sure slash commands are registered (check console output)
- ✅ Wait a few minutes after inviting the bot for commands to sync

### "Invalid Token" error
- ✅ Check that your token in `.env` is correct
- ✅ Ensure there are no extra spaces in the `.env` file
- ✅ Regenerate the token in Discord Developer Portal if needed

### Commands not showing up
- ✅ Slash commands can take up to 1 hour to sync globally
- ✅ Try kicking and re-inviting the bot to your server
- ✅ Check that `applications.commands` scope was selected when inviting

### "Missing Access" error
- ✅ Verify the bot has "Send Messages" permission
- ✅ Check that the bot's role is positioned correctly in server settings
- ✅ Ensure the bot can see the channel you're using

## 📦 Dependencies

- **discord.js** (^14.14.1) - Discord API library
- **dotenv** (^16.3.1) - Environment variable management

## 🤝 Contributing

This bot is part of Samuel Terry's portfolio. To extend or modify:

1. Follow the existing code structure and patterns
2. Maintain comprehensive error handling
3. Document new features in this README
4. Test thoroughly before deploying

## 📄 License

This project is licensed under the MIT License - see the LICENSE file in the root directory for details.

## 🔗 Related Projects

This Discord bot is part of the **Samuel Terry Unity3D Portfolio** repository:
- [Portfolio Main Page](../README.md)
- Unity3D game development projects
- Advanced C# programming examples
- Multiplayer networking systems

## 📞 Contact

**Samuel Terry**
- LinkedIn: [sameats3d](https://www.linkedin.com/in/sameats3d)
- Email: sameats3d@gmail.com

---

**Note**: This bot is designed for demonstration purposes and can be extended for production use. Always follow Discord's Terms of Service and Community Guidelines when deploying bots.
