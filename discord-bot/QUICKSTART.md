# Discord Bot - Quick Start Guide

## 🚀 Get Started in 5 Minutes

### Step 1: Create Your Discord Bot (2 minutes)

1. Go to https://discord.com/developers/applications
2. Click **"New Application"** 
3. Give it a name (e.g., "My Portfolio Bot")
4. Navigate to **"Bot"** section in sidebar
5. Click **"Add Bot"** → Confirm
6. Click **"Reset Token"** → **Copy the token** (save it securely!)
7. Enable these switches under "Privileged Gateway Intents":
   - ✅ MESSAGE CONTENT INTENT
   - ✅ SERVER MEMBERS INTENT (optional)

### Step 2: Invite Bot to Your Server (1 minute)

1. Go to **"OAuth2"** → **"URL Generator"** in sidebar
2. Check these **Scopes**:
   - ✅ `bot`
   - ✅ `applications.commands`
3. Check these **Bot Permissions**:
   - ✅ Send Messages
   - ✅ Embed Links
   - ✅ Read Message History
   - ✅ Use Slash Commands
4. Copy the generated URL at the bottom
5. Open URL in browser → Select your server → Authorize

### Step 3: Configure the Bot (1 minute)

```bash
cd discord-bot
npm install
cp .env.example .env
```

Edit `.env` file:
```env
DISCORD_BOT_TOKEN=paste_your_token_here
DISCORD_CLIENT_ID=paste_your_client_id_here
```

> **Note**: Get CLIENT_ID from "OAuth2" section → "Client ID" at the top

### Step 4: Start the Bot (1 minute)

```bash
npm start
```

You should see:
```
🔄 Registering slash commands...
✅ Successfully registered slash commands globally
✅ Bot is online as YourBotName#1234
📊 Connected to 1 server(s)
```

### Step 5: Test the Commands

In any Discord channel:

1. Type `/test` and press Enter
   - Bot responds with a nice embed showing it's working!

2. Type `/challenge @username` and press Enter
   - Creates an interactive challenge with Accept/Decline buttons!

## 🎉 You're Done!

Your Discord bot is now running with:
- ✅ `/test` command for testing
- ✅ `/challenge` command for user interactions

## 📚 Next Steps

- Read [README.md](README.md) for detailed documentation
- Read [FEATURES.md](FEATURES.md) for technical details
- Customize commands in `commands/` directory
- Add your own commands following the pattern

## ⚠️ Common Issues

### "Invalid Token"
- Make sure you copied the entire token (no spaces)
- Regenerate token in Discord Developer Portal

### Commands not showing
- Wait 5-10 minutes for global commands to sync
- Try `/` in Discord and wait for autocomplete

### Bot doesn't respond
- Check console for error messages
- Verify bot has correct permissions in server
- Make sure bot is online (green dot in Discord)

## 💡 Tips

- Keep your bot token secret - never share it!
- Use `npm run dev` for auto-restart during development (Node.js 18+)
- Check console output for helpful debug information
- The `.env` file is git-ignored for security

## 🆘 Need Help?

Check the full [README.md](README.md) for:
- Detailed setup instructions
- Troubleshooting guide
- Development guidelines
- Security best practices
