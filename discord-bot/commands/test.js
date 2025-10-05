import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';

export default {
  data: new SlashCommandBuilder()
    .setName('test')
    .setDescription('Test command to verify the bot is working correctly')
    .addStringOption(option =>
      option
        .setName('message')
        .setDescription('Optional message to echo back')
        .setRequired(false)
    ),

  async execute(interaction) {
    const message = interaction.options.getString('message');
    const user = interaction.user;
    
    // Create an embed for a nice response
    const embed = new EmbedBuilder()
      .setColor('#00ff00')
      .setTitle('✅ Bot Test Successful!')
      .setDescription('The bot is working correctly!')
      .addFields(
        { name: '👤 Requested by', value: user.tag, inline: true },
        { name: '🆔 User ID', value: user.id, inline: true },
        { name: '⏰ Timestamp', value: `<t:${Math.floor(Date.now() / 1000)}:F>`, inline: false }
      )
      .setThumbnail(user.displayAvatarURL())
      .setTimestamp()
      .setFooter({ text: 'Discord Bot by Samuel Terry' });

    // If a message was provided, add it to the embed
    if (message) {
      embed.addFields({ name: '💬 Your Message', value: message, inline: false });
    }

    await interaction.reply({ embeds: [embed] });
  },
};
