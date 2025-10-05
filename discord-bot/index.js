import { Client, GatewayIntentBits, REST, Routes, SlashCommandBuilder } from 'discord.js';
import dotenv from 'dotenv';
import testCommand from './commands/test.js';
import challengeCommand from './commands/challenge.js';

// Load environment variables
dotenv.config();

// Create Discord client
const client = new Client({
  intents: [
    GatewayIntentBits.Guilds,
    GatewayIntentBits.GuildMessages,
    GatewayIntentBits.MessageContent,
  ],
});

// Command collection
const commands = [
  testCommand,
  challengeCommand,
];

// Ready event
client.once('ready', () => {
  console.log(`✅ Bot is online as ${client.user.tag}`);
  console.log(`📊 Connected to ${client.guilds.cache.size} server(s)`);
});

// Interaction handler
client.on('interactionCreate', async (interaction) => {
  if (!interaction.isChatInputCommand()) return;

  const command = commands.find(cmd => cmd.data.name === interaction.commandName);
  
  if (!command) {
    console.error(`❌ Command not found: ${interaction.commandName}`);
    return;
  }

  try {
    await command.execute(interaction);
  } catch (error) {
    console.error('❌ Error executing command:', error);
    const errorMessage = 'There was an error executing this command!';
    
    if (interaction.replied || interaction.deferred) {
      await interaction.followUp({ content: errorMessage, ephemeral: true });
    } else {
      await interaction.reply({ content: errorMessage, ephemeral: true });
    }
  }
});

// Register slash commands
async function registerCommands() {
  const token = process.env.DISCORD_BOT_TOKEN;
  const clientId = process.env.DISCORD_CLIENT_ID;
  
  if (!token || !clientId) {
    console.error('❌ Missing DISCORD_BOT_TOKEN or DISCORD_CLIENT_ID in .env file');
    process.exit(1);
  }

  const rest = new REST({ version: '10' }).setToken(token);

  try {
    console.log('🔄 Registering slash commands...');
    
    const commandData = commands.map(cmd => cmd.data.toJSON());
    
    await rest.put(
      Routes.applicationCommands(clientId),
      { body: commandData }
    );

    console.log('✅ Successfully registered slash commands globally');
  } catch (error) {
    console.error('❌ Error registering commands:', error);
    process.exit(1);
  }
}

// Login and start bot
async function startBot() {
  const token = process.env.DISCORD_BOT_TOKEN;
  
  if (!token) {
    console.error('❌ DISCORD_BOT_TOKEN is not set in .env file');
    console.error('📝 Please create a .env file with your bot token');
    process.exit(1);
  }

  try {
    await registerCommands();
    await client.login(token);
  } catch (error) {
    console.error('❌ Failed to start bot:', error);
    process.exit(1);
  }
}

// Handle process termination
process.on('SIGINT', () => {
  console.log('\n👋 Shutting down bot...');
  client.destroy();
  process.exit(0);
});

process.on('SIGTERM', () => {
  console.log('\n👋 Shutting down bot...');
  client.destroy();
  process.exit(0);
});

// Start the bot
startBot();
